using UnityEngine;
using System.Collections;

public class MousePosition : MonoBehaviour {

	public static float x()
	{
		return get ().x;
	}
	
	public static float y()
	{
		return get ().y;
	}

	public static Vector3 get()
	{
		return Camera.main.ScreenToWorldPoint (Input.mousePosition) + Vector3.forward;
	}
}
