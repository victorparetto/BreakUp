using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBulletBehav : MonoBehaviour
{
    float speed = 10f;
    Vector2 placeToHidePooled = new Vector2(-9f, 1f);

    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Breakable") || other.CompareTag("Unbreakable"))
        {
            HidePooled();
        }
        else if (other.CompareTag("TopBound"))
        {
            HidePooled();
        }
    }

    private void HidePooled()
    {
        gameObject.SetActive(false);
        transform.position = placeToHidePooled;
    }
}
