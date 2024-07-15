using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallColorChange : MonoBehaviour
{
	[SerializeField]
	Material wallMaterial;

	private void Awake()
	{
		wallMaterial.SetColor("_MainColor", Color.white * 4f);
	}

	void Start()
    {
        
    }

    void Update()
    {
		//Debug.Log(currentColor);
    }
}
