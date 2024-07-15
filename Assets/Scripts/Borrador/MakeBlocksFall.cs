using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeBlocksFall : MonoBehaviour
{
	int[,] level = { 
	//1  2  3  4  5  6  7  8  9
	{ 0, 0, 0, 0, 1, 0, 0, 0, 0},  //1
	{ 0, 0, 0, 1, 1, 1, 0, 0, 0},  //2
	{ 0, 0, 1, 1, 1, 1, 1, 0, 0},  //3
	{ 0, 1, 1, 1, 1, 1, 1, 1, 0},  //4
	{ 0, 1, 1, 1, 1, 1, 1, 1, 0},  //5
	{ 1, 1, 1, 1, 1, 1, 1, 1, 1},  //6
	{ 0, 1, 1, 1, 1, 1, 1, 1, 0},  //7
	{ 0, 1, 1, 1, 1, 1, 1, 1, 0},  //8
	{ 0, 0, 1, 1, 1, 1, 1, 0, 0},  //9
	{ 0, 0, 0, 1, 1, 1, 0, 0, 0},  //10
	{ 0, 0, 0, 0, 1, 0, 0, 0, 0}}; //11

	GameObject[] breakables;
	bool fall = true;

	private void Awake()
	{
		breakables = GameObject.FindGameObjectsWithTag("Breakable");

		for (int x = 0; x < level.Length; x++)
		{
			for (int y = 0; y < level.Length; y++)
			{
				/*if (level[x,y] == level[0,4])
				{
					Debug.Log(level[x, y]);
				}*/
			}
		}
	}

	void Start()
    {
		/*if (fall)
			StartCoroutine(PrepareLevel());*/

	}

    void Update()
    {
		//if(fall)¨StartCoroutine(PrepareLevel());
	}

	IEnumerator PrepareLevel()
	{
		for (int i = 0; i < breakables.Length; i++)
		{
			breakables[i].transform.position =
			Vector2.MoveTowards(breakables[i].transform.position, new Vector2(breakables[i].transform.position.x, breakables[i].transform.position.y - 7.1171f),
			20 * Time.deltaTime);

			yield return new WaitForSeconds(0.5f);
		}
		fall = false;
	}
}
