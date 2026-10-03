using UnityEngine;
using System.Collections;
using Collection.Controls;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Games.LostShader
{
	public class Game : MonoBehaviour {

	    public static float time;
	    public static float timeSpeed;
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

		// In the collection: statics outlive the game here, where they used to die with the
		// application. A mini-game left open when the player went back to the menu would
		// still freeze the player at the next start.
		void Awake ()
		{
			time = 0f;
			timeSpeed = 1f;
			dt = 0f;
			inputOld = false;
			CameraScript.followObject = null;
			CameraScript.inMiniGame = false;

			// In the collection: two settings of the old project that live outside the game's
			// folder. Its mini-game objects sit on the Water layer, which did not collide
			// with Default (the world under them), and Unity 5 moved a body along with its
			// transform at once. Both are put back as they were found when the game is left.
			waterIgnoredBefore = Physics2D.GetIgnoreLayerCollision(0, 4);
			autoSyncBefore = Physics2D.autoSyncTransforms;
			Physics2D.IgnoreLayerCollision(0, 4, true);
			Physics2D.autoSyncTransforms = true;
		}

		private bool waterIgnoredBefore;
		private bool autoSyncBefore;

		void OnDestroy ()
		{
			Physics2D.IgnoreLayerCollision(0, 4, waterIgnoredBefore);
			Physics2D.autoSyncTransforms = autoSyncBefore;
		}

		void Start ()
		{
	        shadowVector = Vector3.down + Vector3.left;
		}

		void Update ()
		{
			// In the collection: the Escape quit is gone, the collection has its own exit.
	        timeSpeed = 1f;
	#if UNITY_EDITOR
	        timeSpeed = (Keyboard.current != null && Keyboard.current.numpadEnterKey.isPressed ? 20f : 1f);
	#endif
	        dt = Time.deltaTime * timeSpeed;
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
