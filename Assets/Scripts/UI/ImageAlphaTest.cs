using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAlphaTest : MonoBehaviour
{
    Image img;

    private void Awake()
    {
        img.GetComponent<Image>();
    }

    private void Start()
    {
        img.alphaHitTestMinimumThreshold = 0.1f; //Enable read/write on sprite inspector
    }
}
