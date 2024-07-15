using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpIndex : MonoBehaviour
{
    public int index;
    public GameObject goldPowerUpForSlot; //Only used by DOUBLE SIZE CHOOSE CARDS    
    public GameObject colorParticles;

    public void ActivateColorParticles(bool activate)
    {
        colorParticles.SetActive(activate);
    }
}
