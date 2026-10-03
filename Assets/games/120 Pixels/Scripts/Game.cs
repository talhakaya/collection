using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Pixels120
{
	public class Game : MonoBehaviour {

		public static float dt;
		public static bool input;
		private static bool inputOld;
		public static bool inputDown;
		public static bool inputUp;

		void Awake ()
		{
			Circle.resetCount();
			inputOld = false;
		}

		void Start ()
		{

		}

		void Update ()
		{
			if (TaloketoInputManager.GetButton("Fast"))
			{
				dt = 10 * Time.deltaTime;
			}
			else
			{
				dt = Time.deltaTime;
			}
			MousePosition.get = Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
			MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = TaloketoInputManager.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;
		}
	}
}
