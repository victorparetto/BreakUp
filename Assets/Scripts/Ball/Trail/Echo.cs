using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Echo : MonoBehaviour
{
	[SerializeField] float TimeToHideObject;
	[SerializeField] bool StartLoop;
	private void Awake()
	{
		StartLoop = true;
	}

	void Start()
    {

	}

    void Update()
    {
		if (StartLoop)
		{
			StartCoroutine(HideObject(TimeToHideObject));
		}
	}

	IEnumerator HideObject(float timer)
	{
		StartLoop = false;
		yield return new WaitForSeconds(timer);
		gameObject.SetActive(false);
		StartLoop = true;
	}
}
