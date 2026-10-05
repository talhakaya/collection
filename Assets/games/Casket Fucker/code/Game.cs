using UnityEngine;
using System.Collections;
using Collection.Controls;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Games.CasketFucker
{
	public class Game : MonoBehaviour {

	    public static Game instance;
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

		// In the collection: the process outlives the game, so the clock starts again with it.
		void Awake ()
		{
			time = 0f;
			input = inputOld = inputDown = inputUp = false;
		}

		void Start ()
		{

	        shadowVector = Vector3.down + Vector3.left;
		}

		void Update ()
		{
			// In the collection: the Escape-quit that stood here is gone; the collection has its own exit.

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

		// In the collection: stands in for Input.anyKeyDown, with the gamepad's buttons added.
		// Every key counts on its own, as it did, so mashing two keys in turn still works.
		public static bool anyKeyDown
		{
			get
			{
				// In the collection: not while the pause screen is up, nor the press that opens it.
				if (Collection.Controls.TaloketoInputManager.Blocked) return false;

				Keyboard keyboard = Keyboard.current;
				if (keyboard != null)
				{
					foreach (KeyControl key in keyboard.allKeys)
					{
						if (key != null && key.wasPressedThisFrame)
						{
							return true;
						}
					}
				}
				Mouse mouse = Mouse.current;
				if (mouse != null)
				{
					if (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame ||
					    mouse.middleButton.wasPressedThisFrame)
					{
						return true;
					}
				}
				Gamepad pad = Gamepad.current;
				if (pad != null)
				{
					if (pad.buttonSouth.wasPressedThisFrame || pad.buttonEast.wasPressedThisFrame ||
					    pad.buttonWest.wasPressedThisFrame || pad.buttonNorth.wasPressedThisFrame ||
					    pad.leftShoulder.wasPressedThisFrame || pad.rightShoulder.wasPressedThisFrame ||
					    pad.leftTrigger.wasPressedThisFrame || pad.rightTrigger.wasPressedThisFrame ||
					    pad.dpad.up.wasPressedThisFrame || pad.dpad.down.wasPressedThisFrame ||
					    pad.dpad.left.wasPressedThisFrame || pad.dpad.right.wasPressedThisFrame)
					{
						return true;
					}
				}
				return false;
			}
		}
	}
}
