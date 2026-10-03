using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.ButYouAreAHorse
{
	public class Game : MonoBehaviour {

		public static float time;
		public static float dt;
		public static Color color0 = new Color(183 / 255f, 162 / 255f, 130 / 255f);
		public static Color color1 = new Color(193 / 255f, 106 / 255f, 68 / 255f);
		public static Color color2 = new Color(170 / 255f, 68 / 255f, 68 / 255f);
		public static Color color3 = new Color(189 / 255f, 68 / 255f, 193 / 255f);
		public static Color color4 = new Color(110 / 255f, 64 / 255f, 183 / 255f);
		public static Color[] colors = new Color[]{color0, color1, color2, color3, color4};
		public static bool input;
		private static bool inputOld;
		public static bool inputDown;
		public static bool inputUp;

		void Start ()
		{

		}

		void Update ()
		{
			// In the collection: the Escape-quit is gone (the collection has its own exit).

	        if (PlatformerController.instance != null)
	        {
	            transform.position = new Vector3(12.8f * Mathf.FloorToInt((PlatformerController.instance.position.x + 6.4f) / 12.8f), transform.position.y, transform.position.z);
	        }

	        if (Collection.Controls.PortHelpers.KeyHeld(UnityEngine.InputSystem.Key.NumpadEnter))
	        {
	            dt = Time.deltaTime * 50f;
	        }
	        else
	        {
	            dt = Time.deltaTime;
	        }

			time += dt;

	        //MousePosition.get = Camera.main.ScreenToWorldPoint(TaloketoInputManager.mousePosition) + Vector3.forward;
	        MousePosition.get = Vector3.zero;
			MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = TaloketoInputManager.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;
		}
	}
}
