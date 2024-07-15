using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour {

    public bool canScroll = true;
    public float scrollSpeed = 5f;

    public MeshCollider[] backgrounds;
    public float yLimit = -20f;

    private float boundWidth;

    MeshCollider mc;

    void FixedUpdate() {

        if (canScroll) {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                CreateInfinite(i);
                Move(i);
            }
        }
    }

    void Move(int i) {
        Vector3 newPos = backgrounds[i].transform.position;
        newPos.y -= scrollSpeed * Time.deltaTime;
        backgrounds[i].transform.position = newPos;
    }

    void CreateInfinite(int i) {
        if (backgrounds[i].transform.position.y < yLimit)
        {
            int valueToFind = i - 1;

            if (valueToFind < 0)
            {
                valueToFind = backgrounds.Length - 1;
            }
            boundWidth = backgrounds[i].bounds.size.y - 0.05f;

            Vector3 temp = backgrounds[i].transform.position;
            temp.y = backgrounds[valueToFind].transform.position.y + boundWidth;
            backgrounds[i].transform.position = temp;
        }
    }
}
