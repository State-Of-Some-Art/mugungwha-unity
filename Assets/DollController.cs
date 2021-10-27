using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;

public class DollController : MonoBehaviour
{
    public void Connect()
    {
        StartCoroutine("ProcessSocket");
    }

    struct MsgPacket
    {
        public int c;
        public string[] imgs;
    }

    private float targetAngle = 0f;
    private float currAngle = 0f;
    private TcpClient client;
    private NetworkStream stream;

    private bool shouldSendReady = false;
    public float difficulty = 1.0f;
    public float speed = 10f;
    public string host = "127.0.0.1";
    public Int32 port = 65432;

    private string jsonBuffer = "";

    public Renderer eye1;
    public Renderer eye2;
    public Material glowMat;
    public Material normMat;
    public AudioSource audioPlayer;
    public Transform canvas;
    public GameObject faceIm;
    public Light directLight;

    private void ParseJson()
    {
        int idx = jsonBuffer.IndexOf("}");
        while (idx >= 0)
        {
            string packet = jsonBuffer.Substring(0, idx + 1);
            jsonBuffer = jsonBuffer.Remove(0, idx + 1);
            idx = jsonBuffer.IndexOf("}");
            HandlePacket(JsonUtility.FromJson<MsgPacket>(packet));
        }
    }

    IEnumerator ProcessSocket()
    {
        client = new TcpClient(host, port);
        stream = client.GetStream();
        Debug.Log("Connected to Python Server");

        while (true)
        {
            if (stream.DataAvailable)
            {
                byte[] data = new byte[1024];
                int bytesRead = stream.Read(data, 0, data.Length);
                if (bytesRead > 0)
                {
                    string s = Encoding.ASCII.GetString(data, 0, bytesRead);
                    jsonBuffer += s;
                }
            }
            ParseJson();
            yield return null;
        }
    }

    private void HandlePacket(MsgPacket p)
    {
        switch (p.c)
        {
            case 0:
                targetAngle = 0;
                break;
            case 1:
//                foreach (Transform t in canvas)
//                {
//                    t.gameObject.SetActive(false)
//;               }
                targetAngle = 180f;
                eye1.material = normMat;
                eye2.material = normMat;
                // Camera.main.transform.position = new Vector3(0, 0.46f, -2.5f);
                audioPlayer.pitch = difficulty;
                audioPlayer.Play();
                shouldSendReady = true;
                difficulty += 0.2f;
                directLight.color = Color.white;
                break;
            case 2:
                eye1.material = glowMat;
                eye2.material = glowMat;
                // Camera.main.transform.position = new Vector3(0, 0.46f, -2.0f);
                directLight.color = Color.black;
                break;
            case 3:
                foreach (Transform t in canvas)
                {
                    t.gameObject.SetActive(false);
                }
                for (int i = 0; i < p.imgs.Length; i++)
                {
                    if(i >= canvas.childCount)
                    {
                        break;
                    }
                    Image im = canvas.GetChild(i).GetComponent<Image>();
                    im.sprite = FromBase64(p.imgs[i]);
                    im.gameObject.SetActive(true);
                }
                break;
            default:
                Debug.Log("None");
                break;
        }
    }

    private Sprite FromBase64(string b64string)
    {
        byte[] bs = Convert.FromBase64String(b64string);
        Texture2D png = new Texture2D(400, 400);
        png.LoadImage(bs);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Rect tRect = new Rect(0, 0, png.width, png.height);
        return Sprite.Create(png, tRect, pivot);
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioPlayer.isPlaying && shouldSendReady)
        {
            shouldSendReady = false;
            byte[] data = Encoding.ASCII.GetBytes("Client is Ready");
            stream.Write(data, 0, data.Length);
        }

        transform.rotation = Quaternion.Euler(0, currAngle, 0);
        if (Math.Abs(targetAngle - currAngle) < speed * difficulty * Time.deltaTime)
        {
            currAngle = targetAngle;
            return;
        }

        if(currAngle < targetAngle)
        {
            currAngle += Time.deltaTime * speed * difficulty;
        }
        else
        {
            currAngle -= Time.deltaTime * speed * difficulty;
        }
    }
}
