using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorAlphaBlink : MonoBehaviour
{
    Image imageRenderer;
    TMP_Text textRenderer;

    float timer = 0;
    bool isDescending;

    public float changePerFrame = 0.02f;
    public float minimumAlpha = 0;

    public bool isImage;
    public bool isText;

    void Awake()
    {
        if (isImage) imageRenderer = GetComponent<Image>();
        else if (isText) textRenderer = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (isImage)
        {
            if (isDescending)
            {
                timer -= changePerFrame;
                imageRenderer.color = new Color(imageRenderer.color.r, imageRenderer.color.g, imageRenderer.color.b, timer);
                if (timer <= minimumAlpha) isDescending = false;
            }
            else
            {
                timer += changePerFrame;
                imageRenderer.color = new Color(imageRenderer.color.r, imageRenderer.color.g, imageRenderer.color.b, timer);
                if (timer >= 1) isDescending = true;
            }
        }
        else if (isText)
        {
            if (isDescending)
            {
                timer -= changePerFrame;
                textRenderer.color = new Color(textRenderer.color.r, textRenderer.color.g, textRenderer.color.b, timer);
                if (timer <= minimumAlpha) isDescending = false;
            }
            else
            {
                timer += changePerFrame;
                textRenderer.color = new Color(textRenderer.color.r, textRenderer.color.g, textRenderer.color.b, timer);
                if (timer >= 1) isDescending = true;
            }
        }
    }
}
