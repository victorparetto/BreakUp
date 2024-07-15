using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleExtraBehaviour : MonoBehaviour
{
	MovePaddle mp;

	[SerializeField] GameObject[] gameObjects;
	[SerializeField] Vector3[] go_position;
	[SerializeField] Vector3[] other_go_position;

	private void Awake()
	{
		mp = GetComponent<MovePaddle>();
	}

	void Start()
    {
        
    }

    void Update()
    {
        switch(mp.currentSize)
		{
			case 0:
				gameObjects[0].transform.position = go_position[0];
				gameObjects[1].transform.position = other_go_position[0];
				break;
			case 1:
				gameObjects[0].transform.position = go_position[1];
				gameObjects[1].transform.position = other_go_position[1];
				break;
			case 2:
				gameObjects[0].transform.position = go_position[2];
				gameObjects[1].transform.position = other_go_position[2];
				break;
			case 3:
				gameObjects[0].transform.position = go_position[3];
				gameObjects[1].transform.position = other_go_position[3];
				break;
		}
    }
}
