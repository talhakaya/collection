using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Collection.Controls
{
	/// <summary>
	/// Always-on input that lives outside any single game's context: currently just the
	/// gamepad Start+Select / keyboard Shift+Escape shortcut back to the main menu.
	/// Bootstraps itself before the first scene loads, so no manual placement is needed.
	/// </summary>
	public class GlobalInputManager : MonoBehaviour
	{
		private const string AssetResourcePath = "Input/CollectionInput";
		private const string MapName = "Global";
		private const string ExitActionName = "ExitToMainMenu";
		private const int MainMenuBuildIndex = 0;

		private InputActionAsset asset;
		private InputAction exitAction;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			var go = new GameObject(nameof(GlobalInputManager));
			DontDestroyOnLoad(go);
			go.AddComponent<GlobalInputManager>();
		}

		private void Awake()
		{
			asset = Resources.Load<InputActionAsset>(AssetResourcePath);
			if (asset == null)
			{
				Debug.LogError($"GlobalInputManager: couldn't load '{AssetResourcePath}' from Resources.");
				return;
			}

			exitAction = asset.FindActionMap(MapName).FindAction(ExitActionName);
			exitAction.performed += OnExitToMainMenu;
			exitAction.Enable();
		}

		private void OnExitToMainMenu(InputAction.CallbackContext context)
		{
			if (SceneManager.GetActiveScene().buildIndex == MainMenuBuildIndex)
			{
				return;
			}

			SceneManager.LoadScene(MainMenuBuildIndex);
		}

		private void OnDestroy()
		{
			if (exitAction != null)
			{
				exitAction.performed -= OnExitToMainMenu;
			}
		}
	}
}
