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

		/// <summary>
		/// For games that depended on their old project's layer collision matrix. Makes the
		/// given layer pairs (a0, b0, a1, b1, ...) ignore each other, in 2D or 3D physics,
		/// and optionally sets the 3D gravity; everything is put back when a scene outside
		/// the game's folder loads. Call it from the game's first Awake; calling it again
		/// while it is in force does nothing.
		/// </summary>
		public static void PhysicsWithinGame(GameObject owner, bool twoD, int[] ignoredPairs, Vector3? gravity3D = null)
		{
			if (physicsInForce) return;
			physicsInForce = true;
			string folder = GameFolder(owner.scene.path);
			bool[] before = new bool[ignoredPairs.Length / 2];
			for (int i = 0; i < before.Length; i++)
			{
				int a = ignoredPairs[i * 2], b = ignoredPairs[i * 2 + 1];
				before[i] = twoD ? Physics2D.GetIgnoreLayerCollision(a, b) : Physics.GetIgnoreLayerCollision(a, b);
				if (twoD) Physics2D.IgnoreLayerCollision(a, b, true); else Physics.IgnoreLayerCollision(a, b, true);
			}
			Vector3 gravityBefore = Physics.gravity;
			if (gravity3D.HasValue) Physics.gravity = gravity3D.Value;

			UnityEngine.Events.UnityAction<Scene, LoadSceneMode> handler = null;
			handler = (scene, mode) =>
			{
				if (folder != "" && scene.path.StartsWith(folder)) return;
				SceneManager.sceneLoaded -= handler;
				physicsInForce = false;
				for (int i = 0; i < before.Length; i++)
				{
					int a = ignoredPairs[i * 2], b = ignoredPairs[i * 2 + 1];
					if (twoD) Physics2D.IgnoreLayerCollision(a, b, before[i]); else Physics.IgnoreLayerCollision(a, b, before[i]);
				}
				if (gravity3D.HasValue) Physics.gravity = gravityBefore;
			};
			SceneManager.sceneLoaded += handler;
		}

		static bool physicsInForce;

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

		static readonly System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> uiHits = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();

		/// <summary>
		/// For games with uGUI buttons and the collection's pointer emulation. The games'
		/// StandaloneInputModule only knows the real mouse, so a click of the emulated
		/// pointer never reaches their buttons. Call this once per frame: when the emulated
		/// pointer clicks, the Button under it is pressed, or the InputField under it gets
		/// the keyboard focus. Does nothing while the real mouse is in use.
		/// </summary>
		public static void ClickUiWithEmulatedPointer()
		{
			if (!GlobalInputManager.MouseEmulationActive || !GlobalInputManager.GetMouseButtonDown(0)) return;
			UnityEngine.EventSystems.EventSystem system = UnityEngine.EventSystems.EventSystem.current;
			if (system == null) return;
			var data = new UnityEngine.EventSystems.PointerEventData(system);
			data.position = GlobalInputManager.EmulatedMousePosition;
			uiHits.Clear();
			system.RaycastAll(data, uiHits);
			foreach (UnityEngine.EventSystems.RaycastResult hit in uiHits)
			{
				var button = hit.gameObject.GetComponentInParent<UnityEngine.UI.Button>();
				if (button != null && button.isActiveAndEnabled && button.interactable)
				{
					button.onClick.Invoke();
					return;
				}
				var field = hit.gameObject.GetComponentInParent<UnityEngine.UI.InputField>();
				if (field != null && field.isActiveAndEnabled && field.interactable)
				{
					field.ActivateInputField();
					return;
				}
			}
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
