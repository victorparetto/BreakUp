using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallExplosionParticlesBehav : MonoBehaviour
{
    GameManager gm;

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();    
    }

    public void OnParticleSystemStopped()
    {
        gm.mainBall.gameObject.SetActive(true);
        if (gm.ballIsResettingAfterDeath) gm.ballIsResettingAfterDeath = false;
        gameObject.SetActive(false);
        transform.parent = null;
    }
}
