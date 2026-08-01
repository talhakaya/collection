using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
