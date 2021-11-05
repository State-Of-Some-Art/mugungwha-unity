using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusableButton : MonoBehaviour
{
    public float scale;

    private Vector3 originalScale;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalScale = rect.localScale;
    }

    public void HighLight()
    {
        rect.localScale = new Vector3(scale, scale, scale);
    }

    public void DeHighLight()
    {
        rect.localScale = originalScale;
    }
}
