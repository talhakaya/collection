using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {
	private SpriteRenderer spriteRenderer;

	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update ()
	{
		spriteRenderer.color = new Color(0f, 0f, 0f, Random.Range(0.5f, 1f));
	}
}
