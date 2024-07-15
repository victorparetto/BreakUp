using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailMovement : MonoBehaviour
{
    RectTransform rt;

    public float RotateSpeed = 5f;
    public float Radius = 0.1f;

    private Vector2 _centre;
    private float _angle;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        _centre = rt.localPosition;
    }

    private void Update()
    {

        _angle += RotateSpeed * Time.deltaTime;

        var offset = new Vector2(Mathf.Sin(_angle), Mathf.Cos(_angle)) * Radius;
        rt.localPosition = _centre + offset;
    }
}
