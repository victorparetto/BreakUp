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
#if UNITY_ANDROID
		height_res = Screen.height;
#endif
		ResolutionBase();

		if (Camera.main.orthographicSize != 7.8f)
		{
			SettingViewComposition();
		}

		aspectRatio = AspectRatio.GetAspectRatio(Screen.width, Screen.height);
	}


	void Start()
    {
        
    }


    void Update()
    {
		//Debug.Log( Camera.main.pixelRect + " " + Camera.main.aspect +" "+ Camera.main.pixelHeight + Camera.main.pixelWidth + " " + Screen.height);
		//Debug.Log(current_width);
		//Debug.Log("Current Screen Height: " + Screen.height + "   " + "Current Screen Width: " + Screen.width);
		//Debug.Log(aspectRatio);
	}

	void ResolutionBase()
	{
		//Debug.Log(Camera.main.aspect);
#if UNITY_EDITOR
		switch (Camera.main.aspect)
		{
			case (0.5621243f):
				Debug.Log(9f/16f);
				height_res = 1920;
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 16;

				break;

			case (0.5f):
				Debug.Log(9f/18f);
				height_res = 2160;
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 18;

				break;

			case (0.4864864864864865f):
				Debug.Log("9:18.5");
				height_res = 2220;
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 18.5f;

				break;

			case (0.4736842105263158f):
				Debug.Log("9:19");
				height_res = 2280;
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 19;

				break;

			case (0.4615384615384615f):
				Debug.Log("9:19.5");
				height_res = 2340;
				Camera.main.orthographicSize = (( height_res / 2f) / 100f) * 0.8125f;
				height_new = 19.5f;

				break;
		}


		/*height_res = Screen.height;
		Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;

		height_new = 18.5f;
		int divisor = 1;
		float reminder_h;
		float reminder_w;

		while(current_height <= 0 && current_width <= 0)
		{
			if(current_width % divisor == 0 && current_height % divisor == 0)
			{
				current_width = current_width / divisor;
				current_height = current_height / divisor;
			}
			divisor++;
		}
		*/
#endif

#if UNITY_ANDROID
		switch (Camera.main.aspect)
		{
			case (0.5621243f):
				Debug.Log(9f / 16f);
				height_res = 1920;
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 16;

				break;

			case (0.5f):
				Debug.Log(9f / 18f);
				height_res = 2160;
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 18;

				break;

			case (0.4864864864864865f):
				Debug.Log("9:18.5");
				height_res = 2220;
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 18.5f;

				break;

			case (0.4736842105263158f):
				Debug.Log("9:19");
				height_res = 2280;
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 19;

				break;

			case (0.4615384615384615f):
				Debug.Log("9:19.5");
				height_res = 2340;
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 19.5f;

				break;
		}
		/*switch ((float)Camera.main.pixelHeight / (float)Camera.main.pixelWidth)
		{
			case (0.5621243f):
				Debug.Log("9:16");
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 16;

				break;

			case (0.5f):
				Debug.Log(9 / 18);
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 18;

				break;

			case (0.4864864864864865f):
				Debug.Log("9:18.5");
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 18.5f;

				break;

			case (0.4736842105263158f):
				Debug.Log("9:19");
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 19;

				break;

			case (0.4615384615384615f):
				Debug.Log("9:19.5");
				Camera.main.orthographicSize = ((height_res / 2f) / 100f) * 0.8125f;
				height_new = 19.5f;

				break;
		}*/
#endif
	}

	void SettingViewComposition()
	{
		//Get current game resolution
		ResolutionBase();
		//---------------------------------------------

		//Setting Borders on Scene (GameObjects)
		TopBorder.transform.localScale = new Vector3(Camera.main.aspect * (2f * Camera.main.orthographicSize), (Camera.main.orthographicSize - 7.8f), 1);
		TopBorder.transform.position = new Vector3(0, cameraSize_Base + TopBorder.transform.localScale.y / 2f, 0);
		BottomBorder.transform.localScale = new Vector3(Camera.main.aspect * (2f * Camera.main.orthographicSize), (Camera.main.orthographicSize - 7.8f), 1);
		BottomBorder.transform.position = new Vector3(0, (-cameraSize_Base) - TopBorder.transform.localScale.y / 2f, 0);
		//---------------------------------------------

		//Camera position
		float camera_Y = (TopBorder.transform.position.y + (Mathf.Abs(BottomBorder.transform.position.y))) / 2f;
		Camera.main.transform.position = new Vector3(0, BottomBorder.transform.position.y + camera_Y, -10);
		//---------------------------------------------

		//Setting Canvas space
		panelScale_height = ((height_base * 100) / height_new) / 100; //Get the height of the current aspect ratio

		canvas_MainPanel.localScale = new Vector3(1, panelScale_height, 1); //Setting 9:16 panel 

		for (int i = 0; i < canvas_ObjectsToScale.Count; i++) //Scaling Objects inside panel
		{
			canvas_ObjectsToScale[i].localScale = new Vector3(
				canvas_ObjectsToScale[i].localScale.x,
				canvas_ObjectsToScale[i].localScale.y + Mathf.Abs(1 - panelScale_height),
				canvas_ObjectsToScale[i].localScale.z);
		}

		if (useCanvasBorders) //Setting Borders on Canvas(Panels)
		{
			canvas_TopBorder.localScale = new Vector3(1, (1f - panelScale_height) / 2f, 1);
			canvas_TopBorder.anchoredPosition = new Vector2(0, (((height_res * panelScale_height) / 2f) + ((height_res * (1f - panelScale_height)) / 2f) / 2f));
			canvas_BottomBorder.localScale = new Vector3(1, (1f - panelScale_height) / 2f, 1);
			canvas_BottomBorder.anchoredPosition = new Vector2(0, -(((height_res * panelScale_height) / 2f) + ((height_res * (1f - panelScale_height)) / 2f) / 2f));
		}
		//---------------------------------------------
	}
}
