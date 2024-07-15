using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBallSprite : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb2d;

    [Range(5,20)]
    [SerializeField] float rotateSpeed = 6f;

    void Update()
    {
        if (rb2d.velocity == Vector2.zero) return;

        if(rb2d.velocity.x < 0)
        {
            transform.Rotate(Vector3.forward * rotateSpeed);
        }
        else
        {
            transform.Rotate(-Vector3.forward * rotateSpeed);
        }
    }
}
