using UnityEngine;
using System.Collections;

public class Lines : MonoBehaviour {
	
	public TalhaAnimation walk;
	public TalhaAnimation idle;
	private int i = 0;

	void Start ()
	{
	
	}

	void Update ()
	{
		int iNew = Mathf.FloorToInt((Game.time % (0.4f)) / 0.2f);
		if (i != iNew)
		{
			transform.localPosition = Geometry.createVector3 (Random.Range (0f, 360), Random.Range (0f, 0.03f));
			transform.localScale = new Vector3(Random.Range (0.95f, 1.05f), Random.Range (0.95f, 1.05f), 1f);
		}
		i = iNew;
	}
}
