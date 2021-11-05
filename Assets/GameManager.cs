using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject startButton;
    public Image gameOverLogo;
    public Text timerText;
    public DollController dollController;

    private float timeLeft = 60;

    public void StartGame()
    {
        startButton.SetActive(false);
        timeLeft = 60;
        StartCoroutine("Tick");
    }

    public void EndGame()
    {
        timeLeft = 0;
        dollController.StopAllCoroutines();
        dollController.gameObject.SetActive(false);
        StartCoroutine("FadeIn");
    }

    IEnumerator FadeIn()
    {
        gameOverLogo.gameObject.SetActive(true);
        while(gameOverLogo.color.a < 1)
        {
            gameOverLogo.color += new Color(0, 0, 0, 0.1f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator Tick()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            timeLeft -= 1;
            timerText.text = timeLeft.ToString();
        }
        EndGame();
    }

}
