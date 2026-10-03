using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.TalhasScreensaver
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
	    public static Vector3 shadowVector;

		// In the collection: the project is in Linear colour space, where a colour handed to a
		// material is converted before the shader sees it. The tints here were written as plain
		// numbers for the particle shader (which doubles them), so they are converted the other
		// way first and arrive as written. Alpha is not converted.
		public static Color gammaTint(Color tint)
		{
			return new Color(Mathf.LinearToGammaSpace(tint.r), Mathf.LinearToGammaSpace(tint.g), Mathf.LinearToGammaSpace(tint.b), tint.a);
		}

		void Awake ()
		{
			// In the collection: the process outlives the toy, so the clock starts over here.
			time = 0f;
			inputOld = false;
		}

		void Start ()
		{
	        shadowVector = Vector3.down + Vector3.left;
		}

		void Update ()
		{
			// In the collection: the Escape-quit is gone (the collection has its own exit).

			dt = Time.deltaTime;
			time += dt;

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
