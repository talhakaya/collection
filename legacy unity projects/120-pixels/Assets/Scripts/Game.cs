using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public static float dt;
	public static bool input;
	private static bool inputOld;
	public static bool inputDown;
	public static bool inputUp;

	void Start ()
	{
		
	}
	
	void Update ()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			dt = 10 * Time.deltaTime;
		}
		else
		{
			dt = Time.deltaTime;
		}
		MousePosition.get = Camera.main.ScreenToWorldPoint (Input.mousePosition) + Vector3.forward;
		MousePosition.x = MousePosition.get.x;
		MousePosition.y = MousePosition.get.y;

		input = Input.GetMouseButton (0);
		inputDown = input && !inputOld;
		inputUp = !input && inputOld;

		inputOld = input;
	}
}
