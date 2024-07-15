using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBehaviour : MonoBehaviour
{
    Rigidbody2D rb2d;
    GameManager gm;
    PowerUpManager pum;

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();    
        pum = GameObject.FindGameObjectWithTag("PowerUpManager").GetComponent<PowerUpManager>();

        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        rb2d.gravityScale = Random.Range(0.4f, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other) //Only used by Bubble PowerUps
    {
        if (other.CompareTag("BotBound"))
        {
            GameObject temp = PoolManager.current.GetPooledGameObject(PoolManager.current.bubbleExplosionParticlesPool, gm.bubbleExplosionParticle);
            temp.transform.position = transform.position;
            temp.SetActive(true);
            pum.currentActiveBubbles.Remove(gameObject);
            gameObject.SetActive(false);
        }
    }
}
