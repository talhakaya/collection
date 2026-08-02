using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

namespace Collection.Controls
{
	/// <summary>
	/// Drop-in replacement for UnityEngine.Input's axis/button polling API, backed by the
	/// New Input System. Game code migrated off the legacy Input Manager should only need
	/// to swap "Input." for "TaloketoInputManager." at each call site - everything else
	/// (which action map is active, resolving names, enabling/disabling) is handled here.
	///
	/// The active game's action map is inferred automatically from the loaded scene's path
	/// (Assets/games/&lt;Name&gt;/...), matching the convention MainMenuController already
	/// uses - no per-scene or per-script wiring is required.
	/// </summary>
	public static class TaloketoInputManager
	{
		private const string AssetResourcePath = "Input/CollectionInput";

		private static InputActionAsset asset;
		private static InputActionMap currentMap;
		private static string currentGameName;
		private static readonly HashSet<string> warnedMissingActions = new HashSet<string>();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Bootstrap()
		{
			asset = Resources.Load<InputActionAsset>(AssetResourcePath);
			if (asset == null)
			{
				Debug.LogError($"TaloketoInputManager: couldn't load '{AssetResourcePath}' from Resources.");
				return;
			}

			SceneManager.sceneLoaded += (scene, mode) => SetActiveGame(GameContext.FromScenePath(scene.path));
			SetActiveGame(GameContext.FromScenePath(SceneManager.GetActiveScene().path));
		}

		private static void SetActiveGame(string gameName)
		{
			if (gameName == currentGameName)
			{
				return;
			}

			currentMap?.Disable();
			currentMap = null;
			currentGameName = gameName;

			if (asset == null || string.IsNullOrEmpty(gameName))
			{
				return;
			}

			currentMap = asset.FindActionMap(gameName);
			if (currentMap == null)
			{
				Debug.LogWarning($"TaloketoInputManager: no action map named '{gameName}' in {AssetResourcePath}.");
				return;
			}

			currentMap.Enable();
		}

		public static float GetAxis(string name)
		{
			return GetAxisRaw(name);
		}

		public static float GetAxisRaw(string name)
		{
			InputAction action = FindAction(name);
			return action?.ReadValue<float>() ?? 0f;
		}

		/// Reads a Vector2-valued action (stick, dpad, 2D composite). Legacy Input had no
		/// equivalent - games polled two named axes - but sticks are natively Vector2 in the
		/// new system, and reading them as one action keeps deadzone/normalisation intact.
		public static Vector2 GetVector2(string name)
		{
			InputAction action = FindAction(name);
			return action?.ReadValue<Vector2>() ?? Vector2.zero;
		}

		public static bool GetButton(string name)
		{
			InputAction action = FindAction(name);
			return action != null && action.IsPressed();
		}

		public static bool GetButtonDown(string name)
		{
			InputAction action = FindAction(name);
			return action != null && action.WasPressedThisFrame();
		}

		public static bool GetButtonUp(string name)
		{
			InputAction action = FindAction(name);
			return action != null && action.WasReleasedThisFrame();
		}

		// ---- Pointer -------------------------------------------------------------------
		// Device-level rather than action-driven, mirroring UnityEngine.Input's mouse API:
		// these are the same for every game, so there's nothing per-game to rebind and no
		// action map entry to look up. Games that polled Input.mousePosition /
		// Input.GetMouseButton* can swap in these directly.
		//
		// One deliberate behaviour difference from legacy Input: these respect window/game
		// view focus (legacy Input polls the OS regardless). So in the Editor these read
		// zero while the Game view is unfocused - that's the New Input System working as
		// intended, and matches how a built player behaves when it loses focus.

		public static Vector3 mousePosition
		{
			get
			{
				Mouse mouse = Mouse.current;
				return mouse == null ? Vector3.zero : (Vector3)mouse.position.ReadValue();
			}
		}

		public static bool GetMouseButton(int button)
		{
			ButtonControl control = MouseButton(button);
			return control != null && control.isPressed;
		}

		public static bool GetMouseButtonDown(int button)
		{
			ButtonControl control = MouseButton(button);
			return control != null && control.wasPressedThisFrame;
		}

		public static bool GetMouseButtonUp(int button)
		{
			ButtonControl control = MouseButton(button);
			return control != null && control.wasReleasedThisFrame;
		}

		private static ButtonControl MouseButton(int button)
		{
			Mouse mouse = Mouse.current;
			if (mouse == null)
			{
				return null;
			}

			switch (button)
			{
				case 0: return mouse.leftButton;
				case 1: return mouse.rightButton;
				case 2: return mouse.middleButton;
				default: return null;
			}
		}

		/// The underlying action on the active game's map, for cases that need more than a
		/// polled value - building shortcut prompts from the real binding, say, so they stay
		/// correct when it's rebound. Null if the game has no such action.
		public static InputAction GetAction(string name)
		{
			return currentMap?.FindAction(name);
		}

		private static InputAction FindAction(string name)
		{
			InputAction action = currentMap?.FindAction(name);
			if (action == null && warnedMissingActions.Add($"{currentGameName}/{name}"))
			{
				Debug.LogWarning($"TaloketoInputManager: no action named '{name}' in map '{currentGameName}'.");
			}

			return action;
		}
	}
}
