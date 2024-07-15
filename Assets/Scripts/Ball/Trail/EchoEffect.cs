using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoEffect : MonoBehaviour
{
	float timeBtwSpawns;
	[SerializeField] float startTimeBtwSpawns;

	[SerializeField] GameObject echo;

	[SerializeField] int echo_Amount;
	List<GameObject> echo_list = new List<GameObject>();

	[SerializeField] bool spawnMore;

	void Start()
	{
		for (int i = 0; i < echo_Amount; i++)
		{
			GameObject echo_obj = Instantiate(echo);
			echo_obj.SetActive(false);
			echo_list.Add(echo_obj);
		}
	}

	void Update()
	{
		if(timeBtwSpawns <= 0)
		{
			//Spawn echo game object
			spawnEcho();
			timeBtwSpawns = startTimeBtwSpawns;
		}
		else
		{
			timeBtwSpawns -= Time.deltaTime;
		}
	}

	GameObject getEcho()
	{
		for (int i = 0; i < echo_list.Count; i++)
		{
			if(!echo_list[i].activeInHierarchy)
			{
				return echo_list[i];
			}
		}

		if(spawnMore)
		{
			GameObject obj = Instantiate(echo);
			echo_list.Add(obj);
			return obj;
		}

		return null;
	}

	void spawnEcho()
	{
		GameObject obj = getEcho();
		if (obj == null) return;
		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;
		obj.SetActive(true);
	}
}
