using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
	[SerializeField] GameObject TopBorder;
	[SerializeField] GameObject BottomBorder;
	[SerializeField] Text text_aspectratio;

	[SerializeField] RectTransform canvas_MainPanel;
	[SerializeField] RectTransform canvas_PaddlePanel;
	[SerializeField] RectTransform canvas_TopBorder;
	[SerializeField] RectTransform canvas_BottomBorder;
	[SerializeField] List<RectTransform> canvas_ObjectsToScale = new List<RectTransform>();

	[SerializeField] bool useCanvasBorders;

	float cameraSize_Base = 7.8f;
	float panelScale_height;
	float height_base = 16;
	float height_new;
	float height_res;
	float test_res;

	float current_height;
	float current_width;

	Vector2 aspectRatio;

	private void Awake()
	{
		CalculateDesiredResolution();
	}


	void Start()
    {
        
    }


    void Update()
    {

	}

	void CalculateDesiredResolution()
	{
		//Debug.Log("Screen  Width Aspect: " + Screen.width);
		//Debug.Log("Screen Height Aspect: " + Screen.height);
		float desiredViewWidth = 9f;
		float desiredViewHeight = 16f;
		float screenRatio =  (float)Screen.width / (float)Screen.height;
		float targetRatio = desiredViewWidth / desiredViewHeight;
		//print((float)Screen.width + " - " + (float)Screen.height);
		//print(screenRatio + " - " + targetRatio);

		if(screenRatio >= targetRatio)
		{
			//print("Screen is greater or equal than Target");
			Camera.main.orthographicSize = desiredViewHeight / 2f;
			//print("Camera Size with the other formula: " + (Camera.main.pixelHeight / 2f / 100f * 0.8125f));
		}
		else
		{
			//print("Target is greater or equal than Screen");
			float differenceInSize = targetRatio / screenRatio;
			Camera.main.orthographicSize = desiredViewHeight / 2f * differenceInSize;
			//print("Camera Size with the other formula: " + (Camera.main.pixelHeight / 2f / 100f * 0.8125f));
		}
	}
}
