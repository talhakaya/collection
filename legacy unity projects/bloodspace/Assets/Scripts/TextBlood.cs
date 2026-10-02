using UnityEngine;
using System.Collections;

public class TextBlood : MonoBehaviour {

	public TextMesh bloodSpaceMesh;
	public TextMesh nesMesh;
	public string text;
	public float size;
	public Color color;
	public bool reversed;
	public bool alwaysReadable;

	void Start ()
	{

	}

	void Update ()
	{
		bloodSpaceMesh.text = text;
		nesMesh.text = text;
		bloodSpaceMesh.color = color;
		nesMesh.color = color;
		bool readable = Game.textReadable;
		if (reversed)
		{
			readable = !Game.textReadable;
		}
		if (alwaysReadable)
		{
			readable = true;
		}
		if (readable)
		{
			nesMesh.gameObject.SetActive(true);
			bloodSpaceMesh.gameObject.SetActive(false);
			nesMesh.transform.localScale = Vector3.one * size;
		}
		else
		{
			bloodSpaceMesh.gameObject.SetActive(true);
			nesMesh.gameObject.SetActive(false);
			bloodSpaceMesh.transform.localScale = Vector3.one * size * 0.7f;
		}
	}
}
