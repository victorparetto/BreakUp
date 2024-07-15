using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyButtonBehaviour : MonoBehaviour
{
    [SerializeField] GameObject equipButton = null;
    [SerializeField] GameObject particlesToSpawn = null;

    public void EndOfBoughtAnimation()
    {
        if(particlesToSpawn != null) 
            Instantiate(particlesToSpawn, transform.position, transform.rotation);

        equipButton.SetActive(true);
        gameObject.SetActive(false);
    }
}
