using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Collection.Controls
{
	/// <summary>
	/// Small helpers for games brought in from legacy projects, for the things that keep
	/// coming up: objects a game marks DontDestroyOnLoad, aiming at the mouse, keys that
	/// stay on the keyboard, and "any key".
	/// </summary>
	public static class PortHelpers
	{
		/// <summary>
		/// DontDestroyOnLoad, but only for as long as the player stays in the game the
		/// object belongs to: it is destroyed when a scene outside that game's folder
		/// loads. For music objects and the like, which used to live until the game quit.
		/// </summary>
		public static void KeepWithinGame(GameObject go)
		{
			string folder = GameFolder(go.scene.path);
			Object.DontDestroyOnLoad(go);
			UnityEngine.Events.UnityAction<Scene, LoadSceneMode> handler = null;
			handler = (scene, mode) =>
			{
				if (go == null)
				{
					SceneManager.sceneLoaded -= handler;
				}
				else if (folder == "" || !scene.path.StartsWith(folder))
				{
					SceneManager.sceneLoaded -= handler;
					Object.Destroy(go);
				}
			};
			SceneManager.sceneLoaded += handler;
		}

		static string GameFolder(string scenePath)
		{
			// "Assets/games/<Name>/..." -> "Assets/games/<Name>/"
			string[] parts = scenePath.Split('/');
			return parts.Length >= 3 ? parts[0] + "/" + parts[1] + "/" + parts[2] + "/" : "";
		}

		static Vector3 lastScreenMouse;
		static Vector2 stickAim = Vector2.right;
		static bool aimingWithStick;

		/// <summary>
		/// For games that aim at the mouse. Returns mouseWorld while the mouse is in use.
		/// Once the gamepad's right stick is pushed, returns a point at the given distance
		/// from origin in the stick's direction instead, and keeps doing so (with the last
		/// direction) until the mouse moves again. Call once per frame.
		/// </summary>
		public static Vector3 AimPoint(Vector3 mouseWorld, Vector3 origin, float distance)
		{
			Vector3 screenMouse = TaloketoInputManager.mousePosition;
			if ((screenMouse - lastScreenMouse).sqrMagnitude > 4f)
			{
				aimingWithStick = false;
			}
			lastScreenMouse = screenMouse;

			Gamepad pad = Gamepad.current;
			if (pad != null)
			{
				Vector2 stick = pad.rightStick.ReadValue();
				if (stick.magnitude > 0.3f)
				{
					stickAim = stick.normalized;
					aimingWithStick = true;
				}
			}

			if (!aimingWithStick)
			{
				return mouseWorld;
			}
			return new Vector3(origin.x + stickAim.x * distance, origin.y + stickAim.y * distance, mouseWorld.z);
		}

		/// <summary>True while the right stick, not the mouse, is doing the aiming.</summary>
		public static bool AimingWithStick
		{
			get { return aimingWithStick; }
		}

		/// <summary>A key that stays on the keyboard only (debug keys, cheats).</summary>
		public static bool KeyDown(Key key)
		{
			Keyboard keyboard = Keyboard.current;
			return keyboard != null && keyboard[key].wasPressedThisFrame;
		}

		/// <summary>A key that stays on the keyboard only, held.</summary>
		public static bool KeyHeld(Key key)
		{
			Keyboard keyboard = Keyboard.current;
			return keyboard != null && keyboard[key].isPressed;
		}

		/// <summary>
		/// Stands in for Input.anyKeyDown: any keyboard key, the left mouse button, or a
		/// face button, shoulder button or Start on any gamepad.
		/// </summary>
		public static bool AnyKeyDown()
		{
			Keyboard keyboard = Keyboard.current;
			if (keyboard != null && keyboard.anyKey.wasPressedThisFrame) return true;
			Mouse mouse = Mouse.current;
			if (mouse != null && mouse.leftButton.wasPressedThisFrame) return true;
			foreach (Gamepad pad in Gamepad.all)
			{
				if (pad.buttonSouth.wasPressedThisFrame || pad.buttonEast.wasPressedThisFrame || pad.buttonWest.wasPressedThisFrame || pad.buttonNorth.wasPressedThisFrame
					|| pad.startButton.wasPressedThisFrame || pad.leftShoulder.wasPressedThisFrame || pad.rightShoulder.wasPressedThisFrame) return true;
			}
			return false;
		}
	}
}
