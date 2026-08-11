using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Collection.Controls
{
	/// <summary>
	/// Always-on input that lives outside any single game's context: the gamepad
	/// Start+Select / keyboard Shift+Escape shortcut back to the main menu, hiding the real
	/// mouse cursor while a gamepad is the active input device (everywhere - menus included,
	/// not just games), and mouse emulation for mouse-only games with no gamepad support of
	/// their own. Bootstraps itself before the first scene loads, so no manual placement is
	/// needed.
	///
	/// This lives here rather than in TaloketoInputManager because it needs an Update loop and
	/// a persistent on-screen cursor - the same always-on, scene-independent shape as the exit
	/// shortcut, and mouse emulation reads from this same Global map. TaloketoInputManager
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
		private bool hideCursorWhenUsingGamepad = true;

		// The game may show/hide the real cursor itself (a cutscene, a crosshair-driven
		// screen, ...) and we want to respect that rather than always forcing ours over it.
		// We also write to Cursor.visible ourselves though, so a bare read can't tell "the
		// game changed its mind" apart from "that's just what we set last frame" - comparing
		// against what we last wrote resolves that.
		private bool gameWantsCursorVisible = true;
		private bool lastAppliedCursorVisible = true;

		// A gamepad being connected doesn't mean it's what's driving the pointer right now -
		// the player might just be using the real mouse. Tracked so picking the mouse back up
		// hands control back immediately instead of the emulated cursor fighting it.
		private bool usingGamepadForMouse;
		private bool emulationActiveLastFrame;

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
			hideCursorWhenUsingGamepad = gameList == null || gameList.hideCursorWhenUsingGamepad;
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

			// New scene: nothing's told us yet whether the player's holding a pad or the mouse
			// for it, so default to the real mouse until the pad actually moves - Update()'s
			// UpdateActiveDevice takes over every frame from here.
			usingGamepadForMouse = false;
			emulationActiveLastFrame = false;
			MouseEmulationActive = false;
			// Fresh scene, fresh assumption: start as if it wants a visible cursor (Unity's own
			// default) until its own Cursor.visible writes say otherwise.
			gameWantsCursorVisible = true;

			if (mouseEmulationEnabled)
			{
				EmulatedMousePosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
			}

			// Whatever the previous scene left it as, a fresh scene starts with a normal real
			// cursor; Update() hides it again the moment a gamepad actually takes over.
			UnityEngine.Cursor.visible = true;
			lastAppliedCursorVisible = true;

			if (cursorImage != null)
			{
				cursorImage.enabled = false;
			}
		}

		private void Update()
		{
			// Runs in every scene, not just ones with mouse emulation opted in - hiding the
			// real cursor while a gamepad drives is a global behaviour (menus included); the
			// stick-moves-a-virtual-cursor part below is the only bit still per-game.
			UpdateActiveDevice();

			bool suppressRealCursor = hideCursorWhenUsingGamepad && usingGamepadForMouse;

			// The game/scene may show or hide the real cursor itself (a cutscene, a
			// crosshair-driven screen, ...) and that should be respected rather than always
			// forcing our own idea of visibility over it. We also write to Cursor.visible
			// ourselves though, so a bare read can't tell "it changed its mind" apart from
			// "that's just what we set last frame" - comparing against what we last wrote
			// resolves that.
			if (UnityEngine.Cursor.visible != lastAppliedCursorVisible)
			{
				gameWantsCursorVisible = UnityEngine.Cursor.visible;
			}

			UnityEngine.Cursor.visible = gameWantsCursorVisible && !suppressRealCursor;
			lastAppliedCursorVisible = UnityEngine.Cursor.visible;

			// Moving a virtual cursor with the stick only makes sense for games that opted in,
			// and only while we're actually suppressing the real one for a gamepad - if the
			// debug toggle is forcing the real cursor to stay on, emulation stands down too.
			bool emulating = mouseEmulationEnabled && suppressRealCursor;
			MouseEmulationActive = emulating;

			if (!emulating)
			{
				if (cursorImage != null) cursorImage.enabled = false;

				if (emulationActiveLastFrame && Mouse.current != null)
				{
					// Player picked the real mouse back up - warp it to where the emulated
					// cursor was so it doesn't jump. Only fires on the single frame the switch
					// happens, so it doesn't fight whatever mouse movement triggered the switch.
					Mouse.current.WarpCursorPosition(EmulatedMousePosition);
				}
				emulationActiveLastFrame = false;

				// Keep tracking the real mouse even while it isn't the active device, so
				// whenever the pad takes emulation back over it picks up from wherever the
				// mouse actually left the pointer instead of jumping to a stale position.
				if (mouseEmulationEnabled && Mouse.current != null)
				{
					EmulatedMousePosition = Mouse.current.position.ReadValue();
				}

				return;
			}

			emulationActiveLastFrame = true;

			if (cursorImage != null)
			{
				cursorImage.enabled = gameWantsCursorVisible && cursorImage.sprite != null;
			}

			Vector2 stick = mouseMoveAction != null ? mouseMoveAction.ReadValue<Vector2>() : Vector2.zero;
			Vector2 pos = EmulatedMousePosition + stick * mouseEmulationSpeed * Time.deltaTime;
			pos.x = Mathf.Clamp(pos.x, 0f, Screen.width);
			pos.y = Mathf.Clamp(pos.y, 0f, Screen.height);
			EmulatedMousePosition = pos;

			if (cursorRect != null)
			{
				cursorRect.anchoredPosition = EmulatedMousePosition;
			}
		}

		/// Whether a gamepad is the thing actually moving the pointer right now, not just
		/// whether one happens to be connected. Compares each device's own InputSystem
		/// timestamp for its last actual input event, so whichever was touched most recently
		/// wins - same idea as GolfinityGamepad's shortcut-prompt visibility.
		private void UpdateActiveDevice()
		{
			Gamepad pad = Gamepad.current;
			if (pad == null)
			{
				usingGamepadForMouse = false;
				return;
			}

			double mouseTime = Mouse.current != null ? Mouse.current.lastUpdateTime : 0.0;
			if (pad.lastUpdateTime > mouseTime) usingGamepadForMouse = true;
			else if (mouseTime > pad.lastUpdateTime) usingGamepadForMouse = false;
			// Equal (neither touched yet this session) - keep whatever it already was.
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
