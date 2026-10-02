using UnityEngine;
using System.Collections;

public class ButtonEyesore : MonoBehaviour {

	void Start ()
	{
		transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (0, 0, 0f)) + new Vector3(0.5f, 1.5f, 5f);
	}
	
	void Update ()
	{
		
	}
	
	void OnMouseUp()
	{
		GameManager.eyesore = !GameManager.eyesore;
	}
}
