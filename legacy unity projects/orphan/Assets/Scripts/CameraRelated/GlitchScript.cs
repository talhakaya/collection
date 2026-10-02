using UnityEngine;
using System.Collections;

public class GlitchScript : MonoBehaviour {
	
	public static float size;
	
	private float period;
	private float periodCounter;
	private Vector3 randomPositionChange;
	private Vector3 randomScaleChange;
	private const float scaleConst = 0.1f;
	private const float speedConst = 0.05f;
	
	void Start ()
	{
		size = 10f;
		GetComponent<tk2dSprite>().color = new Color(1f, 1f, 1f, 0.7f);
		Reset();
	}
	
	void Update ()
	{
		periodCounter += Time.deltaTime;
		if (periodCounter >= period)
		{
			Reset();
		}
		else
		{
			transform.localPosition += randomPositionChange;
			transform.localScale += randomScaleChange;
		}
	}
	
	void Reset()
	{
		periodCounter = 0;
		float multiplier = 1f;
		if (CameraScript.instance.cameraTk2d.ZoomFactor > 0f)
		{
			multiplier = 1f / CameraScript.instance.cameraTk2d.ZoomFactor;
		}
		transform.localPosition = (Vector3.right * Random.Range(-48f, 48f) + Vector3.up * Random.Range(-27f, 27f)) * multiplier;
		transform.localScale = new Vector3(Random.Range(size / 2f, size), Random.Range(size / 2f, size), 1f) * multiplier;
		randomPositionChange = new Vector3(Random.Range(-speedConst, speedConst), Random.Range(-speedConst, speedConst), 0f);
		randomScaleChange = new Vector3(Random.Range(-scaleConst, scaleConst), Random.Range(-scaleConst, scaleConst), 0f);
		transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
		period = Random.Range(0.05f, 0.2f);
	}
}
