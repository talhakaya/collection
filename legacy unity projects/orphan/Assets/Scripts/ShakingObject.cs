using UnityEngine;
using System.Collections;

public class ShakingObject : MonoBehaviour {
	
	private Vector3 originalPosition;
	
	public float shakeRadiusX;
	public float shakeRadiusY;
	public bool isShaking;
	
	void Start ()
	{
		originalPosition = transform.position;
	}
	
	void Update ()
	{
		transform.position = originalPosition + new Vector3(-shakeRadiusX + 2 * shakeRadiusX * Random.Range (0f, 1f),
			-shakeRadiusY + 2 * shakeRadiusY * Random.Range (0f, 1f), 0);
	}
}
