using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVisual : MonoBehaviour
{
	private const int SAMPLE_SIZE = 1024;

	[SerializeField] float rmsValue;
	[SerializeField] float dbValue;
	[SerializeField] float pitchValue;

	[SerializeField] float backgroundIntensity;

	[SerializeField] Material backgroundMaterial;
	[SerializeField] Color BGminColor;
	[SerializeField] Color BGmaxColor;

	[SerializeField] Material objectsMaterial;
	[SerializeField] Color OBJminColor;
	[SerializeField] Color OBJmaxColor;


	[SerializeField] float maxVisualScale;
	[SerializeField] float visualModifier = 50.0f;
	[SerializeField] float smoothSpeed = 10.0f;
	[SerializeField] float keepPercentage = 0.5f;

	private AudioSource audioSource;
	private float[] samples;
	private float[] spectrum;
	private float sampleRate;

	private Transform[] visualList;
	private float[] visualScale;
	[SerializeField] int amnVisual = 64;
	[SerializeField] GameObject[] visualObjects;

    void Start()
    {
		audioSource = GetComponent<AudioSource>();
		samples = new float[SAMPLE_SIZE];
		spectrum = new float[SAMPLE_SIZE];
		sampleRate = AudioSettings.outputSampleRate;

		//SpawnLine();
		//SpawnCircle();
		FreeForm();
    }

	private void SpawnLine()
	{
		visualScale = new float[amnVisual];
		visualList = new Transform[amnVisual];

		for (int i = 0; i < amnVisual; i++)
		{
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
			obj.GetComponent<MeshRenderer>().material = objectsMaterial;
			visualList[i] = obj.transform;
			visualList[i].position = Vector3.right * i;
		}
	}
	private void FreeForm()
	{
		visualScale = new float[visualObjects.Length];
		visualList = new Transform[visualObjects.Length];

		for (int i = 0; i < visualObjects.Length; i++)
		{
			visualList[i] = visualObjects[i].transform;
		}
		/*Vector3 center = new Vector3(0, 0, 0);
		float radius = 2.5f;

		for (int i = 0; i < visualObjects.Length; i++)
		{
			float angle = i * 1.0f / visualObjects.Length;
			angle = angle * Mathf.PI * 2;

			float x = center.x + Mathf.Cos(angle) * radius;
			float y = center.y + Mathf.Sin(angle) * radius;

			Vector3 pos = center + new Vector3(x, y, 0);
			visualObjects[i].transform.position = new Vector3(pos.x, pos.y - 7.5f, pos.z);
			visualObjects[i].transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
			visualList[i] = visualObjects[i].transform;
		}*/
	}
	private void SpawnCircle()
	{
		visualScale = new float[amnVisual];
		visualList = new Transform[amnVisual];

		Vector3 center = new Vector3(0, -8, 10);
		float radius = 4.5f;

		for (int i = 0; i < amnVisual; i++)
		{
			float angle = i * 1.0f / amnVisual;
			angle = angle * Mathf.PI * 2;

			float x = center.x + Mathf.Cos(angle) * radius;
			float y = center.y + Mathf.Sin(angle) * radius;

			Vector3 pos = center + new Vector3(x, y, 0);
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
			obj.GetComponent<MeshRenderer>().material = objectsMaterial;
			obj.transform.position = pos;
			obj.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
			visualList[i] = obj.transform;
		}
	}

    void Update()
    {
		AnalyzaSound();
		updateVisual();
		//updateBackground();
    }

	private void updateVisual()
	{
		// By creating primitives objects
		/*int visualIndex = 0;
		int spectrumIndex = 0;
		int averageSize = (int)(SAMPLE_SIZE * keepPercentage) / amnVisual;

		while(visualIndex < amnVisual)
		{
			int j = 0;
			float sum = 0;
			while(j < averageSize)
			{
				sum += spectrum[spectrumIndex];
				spectrumIndex++;
				j++;
			}

			float scaleY = sum / averageSize * visualModifier;
			visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;

			if(visualScale[visualIndex] < scaleY) visualScale[visualIndex] = scaleY;
			if(visualScale[visualIndex] > maxVisualScale) visualScale[visualIndex] = maxVisualScale;

			visualList[visualIndex].localScale = Vector3.one + Vector3.up * visualScale[visualIndex];
			visualIndex++;
		}*/

		//Personal Free form creation
		int visualIndex = 0;
		int spectrumIndex = 0;
		int averageSize = (int)(SAMPLE_SIZE * keepPercentage) / visualObjects.Length;

		while (visualIndex < visualObjects.Length)
		{
			int j = 0;
			float sum = 0;
			while (j < averageSize)
			{
				sum += spectrum[spectrumIndex];
				spectrumIndex++;
				j++;
			}

			float scaleY = sum / averageSize * visualModifier;
			visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;

			if (visualScale[visualIndex] < scaleY && visualScale[visualIndex] != visualScale[0]
				&& visualScale[visualIndex] != visualScale[1]
				&& visualScale[visualIndex] != visualScale[2]) visualScale[visualIndex] = scaleY;
			if (visualScale[visualIndex] == visualScale[0]) visualScale[visualIndex] = scaleY / 4;
			if (visualScale[visualIndex] == visualScale[1] || visualScale[visualIndex] == visualScale[2]) visualScale[visualIndex] = scaleY / 2.5f;
			if (visualScale[visualIndex] > maxVisualScale) visualScale[visualIndex] = maxVisualScale;

			visualList[visualIndex].localScale = new Vector3(visualList[visualIndex].localScale.x, 10 * visualScale[visualIndex],
				1);
			visualIndex++;
		}
	}

	private void updateBackground()
	{
		backgroundIntensity -= Time.deltaTime * smoothSpeed;
		if (backgroundIntensity < dbValue / 40) backgroundIntensity = dbValue / 40;

		backgroundMaterial.color = Color.Lerp(BGmaxColor, BGminColor, -backgroundIntensity);
		objectsMaterial.color = Color.Lerp(OBJmaxColor, OBJminColor, -backgroundIntensity);
	}
	private void AnalyzaSound()
	{
		audioSource.GetOutputData(samples, 0);

		//Get the RMS
		int i = 0;
		float sum = 0;
		for (; i < SAMPLE_SIZE; i++)
		{
			sum += samples[i] * samples[i];
		}
		rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

		//Get the DB value
		dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

		//Get sound spectrum
		audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

		//Find pitch
		float maxV = 0;
		var maxN = 0;
		for (i = 0; i < SAMPLE_SIZE; i++)
		{
			if (!(spectrum[i] > maxV) || !(spectrum[i] > 0.0f)) continue;

			maxV = spectrum[i];
			maxN = i;
		}

		float freqN = maxN;
		if(maxN > 0 && maxN < SAMPLE_SIZE - 1)
		{
			var dL = spectrum[maxN - 1] / spectrum[maxN];
			var dR = spectrum[maxN + 1] / spectrum[maxN];
			freqN += 0.5f * (dR * dR - dL * dL);
		}
		pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;
	}
}
