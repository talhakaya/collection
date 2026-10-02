using UnityEngine;
using System.Collections;

public class ButtonRestart : MonoBehaviour {

	void Start ()
	{
		transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (0, Screen.height, 0f)) + new Vector3(0.75f, -0.75f, 5f);
	}
	
	void Update ()
	{
		
	}
	
	void OnMouseUp()
	{
		Ball.instance.init ();
	}
}
