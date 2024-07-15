using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUpBehaviour : MonoBehaviour
{
    [SerializeField] int maxHitCounter = 1;
    [SerializeField] int currentHitCounter = 1;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Ball"))
        {
            currentHitCounter--;

            if(currentHitCounter <= 0)
            {
                currentHitCounter = maxHitCounter;
                gameObject.SetActive(false);
            }
        }
    }
}
