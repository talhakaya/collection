using UnityEngine;
using System.Collections;

public class GoldPiece : MonoBehaviour {

	private float downdowndown;
	private float right;
	private Vector3 firstPos;

	void Start ()
	{
		downdowndown = Random.Range (-8f, -12f);
		firstPos = transform.position;
		right = Random.Range (-5f, 5f);
	}

	void Update ()
	{
		downdowndown += Time.deltaTime * 10f;
		transform.position += new Vector3 (right, -downdowndown, 0f) * Time.deltaTime;
		if (downdowndown > 15f)
		{
			downdowndown = Random.Range (-8f, -12f);
			transform.position = firstPos;
			right = Random.Range (-5f, 5f);
		}
	}
}
