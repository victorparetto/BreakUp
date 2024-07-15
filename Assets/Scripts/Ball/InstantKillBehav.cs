using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantKillBehav : MonoBehaviour
{
    Ball ballParent;

    private void Awake()
    {
        ballParent = transform.GetComponentInParent<Ball>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Breakable"))
        {
            ballParent.currentPhasingsLeft -= 1;
            ballParent.AddBreakableScore();
        }        
    }
}
