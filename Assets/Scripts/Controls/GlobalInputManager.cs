using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Collection.Controls
{
	/// <summary>
	/// Always-on input that lives outside any single game's context: the gamepad
	/// Start+Select / keyboard Shift+Escape shortcut back to the main menu, and mouse
	/// emulation for mouse-only games with no gamepad support of their own. Bootstraps
	/// itself before the first scene loads, so no manual placement is needed.
	///
	/// Mouse emulation lives here rather than in TaloketoInputManager because it needs an
	/// Update loop and a persistent on-screen cursor - the same always-on, scene-independent
	/// shape as the exit shortcut, and it reads from this same Global map. TaloketoInputManager
	/// stays the single per-game Input-API surface: its mouse methods read the emulated state
	/// from here when active, so migrated call sites don't need to know which kind of pointer
	/// they're getting.
	/// </summary>
	public class GlobalInputManager : MonoBehaviour
	{
		private const string AssetResourcePath = "Input/CollectionInput";
		private const string GameListResourcePath = "Games/GameList";
		private const string CursorResourcePath = "UI/MouseEmulationCursor";
		private const string MapName = "Global";
		private const string ExitActionName = "ExitToMainMenu";
		private const int MainMenuBuildIndex = 0;
		private const float DefaultCursorSize = 32f;

		private InputActionAsset asset;
		private InputAction exitAction;

		private GameList gameList;
		private InputAction mouseMoveAction;
		private InputAction mouseLeftClickAction;
		private InputAction mouseRightClickAction;
		private string currentGameName;
		private bool mouseEmulationEnabled;
		private float mouseEmulationSpeed;

		private RectTransform cursorRect;
		private Image cursorImage;

		public static bool MouseEmulationActive { get; private set; }
		public static Vector2 EmulatedMousePosition { get; private set; }

		private static GlobalInputManager instance;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			var go = new GameObject(nameof(GlobalInputManager));
			DontDestroyOnLoad(go);
			instance = go.AddComponent<GlobalInputManager>();
		}

		private void Awake()
		{
			asset = Resources.Load<InputActionAsset>(AssetResourcePath);
			if (asset == null)
			{
				Debug.LogError($"GlobalInputManager: couldn't load '{AssetResourcePath}' from Resources.");
				return;
			}

			InputActionMap map = asset.FindActionMap(MapName);

			exitAction = map.FindAction(ExitActionName);
			exitAction.performed += OnExitToMainMenu;
			exitAction.Enable();

			mouseMoveAction = map.FindAction("MouseEmulateMove");
			mouseLeftClickAction = map.FindAction("MouseEmulateLeftClick");
			mouseRightClickAction = map.FindAction("MouseEmulateRightClick");
			mouseMoveAction?.Enable();
			mouseLeftClickAction?.Enable();
			mouseRightClickAction?.Enable();

			gameList = Resources.Load<GameList>(GameListResourcePath);
			CreateCursor();

			SceneManager.sceneLoaded += (scene, mode) => ApplyMouseEmulation(GameContext.FromScenePath(scene.path));
			ApplyMouseEmulation(GameContext.FromScenePath(SceneManager.GetActiveScene().path));
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

		// ---- Mouse emulation -------------------------------------------------------------

		private void ApplyMouseEmulation(string gameName)
		{
			if (gameName == currentGameName)
			{
				return;
			}

			currentGameName = gameName;
			mouseEmulationEnabled = false;
			mouseEmulationSpeed = 1000f;

			if (gameList != null && gameName != null && gameList.TryGetEntry(gameName, out GameList.Entry entry))
			{
				mouseEmulationEnabled = entry.enableMouseEmulation;
				mouseEmulationSpeed = entry.mouseEmulationSpeed > 0f ? entry.mouseEmulationSpeed : 1000f;
			}

			MouseEmulationActive = mouseEmulationEnabled;
			if (mouseEmulationEnabled)
			{
				EmulatedMousePosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
			}

			UnityEngine.Cursor.visible = !mouseEmulationEnabled;
			if (cursorImage != null)
			{
				cursorImage.enabled = mouseEmulationEnabled && cursorImage.sprite != null;
			}
		}

		private void Update()
		{
			if (!mouseEmulationEnabled || mouseMoveAction == null)
			{
				return;
			}

			Vector2 stick = mouseMoveAction.ReadValue<Vector2>();
			Vector2 pos = EmulatedMousePosition + stick * mouseEmulationSpeed * Time.deltaTime;
			pos.x = Mathf.Clamp(pos.x, 0f, Screen.width);
			pos.y = Mathf.Clamp(pos.y, 0f, Screen.height);
			EmulatedMousePosition = pos;

			if (cursorRect != null)
			{
				cursorRect.anchoredPosition = EmulatedMousePosition;
			}
		}

		/// Sizes and positions itself in raw screen pixels (anchored/pivot at the bottom-left,
		/// no CanvasScaler) so EmulatedMousePosition - which mirrors Input.mousePosition's own
		/// screen-pixel convention - can be assigned to anchoredPosition directly.
		private void CreateCursor()
		{
			var canvasGo = new GameObject("MouseEmulationCanvas", typeof(Canvas));
			canvasGo.transform.SetParent(transform, false);
			Canvas canvas = canvasGo.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = short.MaxValue;

			var cursorGo = new GameObject("Cursor", typeof(RectTransform), typeof(Image));
			cursorGo.transform.SetParent(canvasGo.transform, false);
			cursorRect = cursorGo.GetComponent<RectTransform>();
			cursorRect.anchorMin = cursorRect.anchorMax = cursorRect.pivot = Vector2.zero;
			cursorRect.sizeDelta = new Vector2(DefaultCursorSize, DefaultCursorSize);

			cursorImage = cursorGo.GetComponent<Image>();
			cursorImage.raycastTarget = false;
			cursorImage.enabled = false;

			Sprite sprite = Resources.Load<Sprite>(CursorResourcePath);
			if (sprite != null)
			{
				cursorImage.sprite = sprite;
			}
			else
			{
				Debug.LogWarning($"GlobalInputManager: no cursor sprite at Resources/{CursorResourcePath} - mouse emulation still works, the cursor just won't be visible until one is added there.");
			}
		}

		public static bool GetMouseButton(int button)
		{
			return MouseButtonAction(button)?.IsPressed() == true;
		}

		public static bool GetMouseButtonDown(int button)
		{
			return MouseButtonAction(button)?.WasPressedThisFrame() == true;
		}

		public static bool GetMouseButtonUp(int button)
		{
			return MouseButtonAction(button)?.WasReleasedThisFrame() == true;
		}

		private static InputAction MouseButtonAction(int button)
		{
			if (instance == null) return null;
			switch (button)
			{
				case 0: return instance.mouseLeftClickAction;
				case 1: return instance.mouseRightClickAction;
				default: return null;
			}
		}
	}
}
