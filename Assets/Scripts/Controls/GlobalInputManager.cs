using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Collection.Controls
{
	/// <summary>
	/// Always-on input that lives outside any single game's context: the gamepad
	/// Start+Select / keyboard Shift+Escape shortcut back to the main menu, the cursor, and
	/// mouse emulation for mouse-only games with no gamepad support of their own. Bootstraps
	/// itself before the first scene loads, so no manual placement is needed.
	///
	/// The cursor is drawn by the collection itself, always - the OS cursor is never shown
	/// over the game. A mouse and a gamepad stick move the same sprite, so switching between
	/// them changes nothing on screen, and it is the only kind of pointer that exists on a
	/// platform with no OS cursor. With a gamepad it is shown only in games that opted into
	/// mouse emulation; everywhere else (menus included) a pad means no cursor at all.
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

		// The cursor is sized for a 1080-pixel-high screen and scaled with the real height, so
		// it is the same size relative to the picture on a Steam Deck and at 4K.
		private const float CursorReferenceHeight = 1080f;
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

		// A gamepad being connected doesn't mean it's what's driving the pointer right now -
		// the player might just be using the real mouse. Tracked so picking the mouse back up
		// hands control back immediately instead of the emulated cursor fighting it.
		private bool usingGamepadForMouse;
		private bool emulationActiveLastFrame;

		// Whether the real mouse has genuinely been moved or clicked at any point this session.
		// Until it has, a connected gamepad is taken to be the active device, and no mouse
		// cursor is drawn at all - see UpdateActiveDevice.
		private bool mouseEverUsed;

		private Canvas cursorCanvas;
		private RectTransform cursorRect;
		private Image cursorImage;
		private Sprite defaultCursorSprite;
		private Sprite gameCursorSprite;

		// The current cursor's size on a CursorReferenceHeight-high screen.
		private Vector2 cursorBaseSize = new Vector2(DefaultCursorSize, DefaultCursorSize);

		// What the current game's GameList entry asks for, kept so ClearGameCursor has
		// something to go back to after the game has overridden it from code.
		private Texture2D entryCursorTexture;
		private Vector2 entryCursorHotspot;
		private bool gameCursorHidden;

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
			ReturnToMainMenu();
		}

		/// <summary>
		/// Leaves the current game for the collection's main menu.
		///
		/// Public because game code calls it too, wherever a game used to call
		/// Application.Quit(): a game running inside the collection is never the thing that
		/// should be closing the application, and quitting one should land you back in the
		/// list rather than on the desktop. Keeping it here means the games don't each need
		/// to know which build index the menu is.
		///
		/// A no-op when the menu is already what's loaded.
		/// </summary>
		public static void ReturnToMainMenu()
		{
			if (SceneManager.GetActiveScene().buildIndex == MainMenuBuildIndex)
			{
				return;
			}

			SceneManager.LoadScene(MainMenuBuildIndex);
		}

		// ---- Cursor control for game code ------------------------------------------------
		// For a game whose pointer changes as it plays, which the fixed cursorTexture on its
		// GameList entry can't express. Unity's own Cursor.SetCursor and Cursor.visible do
		// nothing useful here: they act on the OS cursor, which the collection never shows.
		//
		// A game never has to undo any of this before it exits: everything set here is dropped
		// when the game changes (see ApplyMouseEmulation), by whatever route the player left.
		// It does persist across the game's own scenes, until the game clears it.

		/// <summary>
		/// Replaces the cursor until ClearGameCursor or the game is left. Null means the
		/// collection's default cursor, even for a game whose GameList entry sets one.
		/// The hotspot is the click point, in pixels from the texture's top-left corner.
		/// </summary>
		public static void SetGameCursor(Texture2D texture, Vector2 hotspot)
		{
			if (instance == null) return;

			instance.gameCursorHidden = false;
			instance.ApplyCursorTexture(texture, hotspot);
		}

		/// <summary>
		/// Stops the collection drawing any cursor, for while the game is drawing its own
		/// pointer. The pointer keeps working - a gamepad still moves and clicks it - there is
		/// just nothing of ours on top of the game's.
		/// </summary>
		public static void HideGameCursor()
		{
			if (instance == null) return;

			instance.gameCursorHidden = true;
		}

		/// <summary>
		/// Undoes SetGameCursor and HideGameCursor, back to what the game's GameList entry
		/// specifies (or the defaults, if it specifies nothing).
		/// </summary>
		public static void ClearGameCursor()
		{
			if (instance == null) return;

			instance.gameCursorHidden = false;
			instance.ApplyCursorTexture(instance.entryCursorTexture, instance.entryCursorHotspot);
		}

		private void OnDestroy()
		{
			if (exitAction != null)
			{
				exitAction.performed -= OnExitToMainMenu;
			}

			// Cursor.visible outlives play mode in the Editor, so the OS cursor would otherwise
			// stay hidden over the Game view after stopping.
			UnityEngine.Cursor.visible = true;
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
			Texture2D cursorTexture = null;
			Vector2 cursorHotspot = Vector2.zero;

			if (gameList != null && gameName != null && gameList.TryGetEntry(gameName, out GameList.Entry entry))
			{
				mouseEmulationEnabled = entry.enableMouseEmulation;
				mouseEmulationSpeed = entry.mouseEmulationSpeed > 0f ? entry.mouseEmulationSpeed : 1000f;
				cursorTexture = entry.cursorTexture;
				cursorHotspot = entry.cursorHotspot;
			}

			// Applied unconditionally rather than only when a game has a texture: the main menu
			// and games without one take the null branch, which is what puts the defaults back
			// after a game that had its own. There is no separate "leaving a game" cleanup to
			// forget. The same goes for anything the previous game set from its own code
			// (SetGameCursor/HideGameCursor): overwriting it here is the whole reset.
			entryCursorTexture = cursorTexture;
			entryCursorHotspot = cursorHotspot;
			gameCursorHidden = false;
			ApplyCursorTexture(cursorTexture, cursorHotspot);

			// New scene: nothing's told us yet whether the player's holding a pad or the mouse
			// for it, so default to the real mouse until the pad actually moves - Update()'s
			// UpdateActiveDevice takes over every frame from here.
			usingGamepadForMouse = false;
			emulationActiveLastFrame = false;
			MouseEmulationActive = false;

			if (mouseEmulationEnabled)
			{
				EmulatedMousePosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
			}

			if (cursorImage != null)
			{
				cursorImage.enabled = false;
			}
		}

		private void Update()
		{
			// Runs in every scene, not just ones with mouse emulation opted in - the cursor is
			// global (menus included); the stick-moves-the-pointer part below is the only bit
			// that is per-game.
			UpdateActiveDevice();

			// Written every frame rather than once: the Editor and the OS put the cursor back
			// when the window regains focus. Outside the window the OS shows it regardless.
			UnityEngine.Cursor.visible = false;

			bool gamepadDriving = hideCursorWhenUsingGamepad && usingGamepadForMouse;

			// Moving the pointer with the stick only makes sense for games that opted in, and
			// only while a gamepad is actually what's driving - if the debug toggle is keeping
			// the mouse cursor on regardless, emulation stands down too.
			bool emulating = mouseEmulationEnabled && gamepadDriving;
			MouseEmulationActive = emulating;

			Mouse mouse = Mouse.current;

			if (emulating)
			{
				emulationActiveLastFrame = true;

				Vector2 stick = mouseMoveAction != null ? mouseMoveAction.ReadValue<Vector2>() : Vector2.zero;
				Vector2 pos = EmulatedMousePosition + stick * mouseEmulationSpeed * Time.deltaTime;
				pos.x = Mathf.Clamp(pos.x, 0f, Screen.width);
				pos.y = Mathf.Clamp(pos.y, 0f, Screen.height);
				EmulatedMousePosition = pos;

				DrawCursor(EmulatedMousePosition, true);
				return;
			}

			if (emulationActiveLastFrame && mouse != null)
			{
				// Player picked the real mouse back up - warp it to where the emulated
				// cursor was so it doesn't jump. Only fires on the single frame the switch
				// happens, so it doesn't fight whatever mouse movement triggered the switch.
				mouse.WarpCursorPosition(EmulatedMousePosition);
			}
			emulationActiveLastFrame = false;

			Vector2 mousePosition = mouse != null ? mouse.position.ReadValue() : Vector2.zero;

			// Keep tracking the real mouse even while it isn't the active device, so
			// whenever the pad takes emulation back over it picks up from wherever the
			// mouse actually left the pointer instead of jumping to a stale position.
			if (mouseEmulationEnabled && mouse != null)
			{
				EmulatedMousePosition = mousePosition;
			}

			// The mouse's own cursor: only once the mouse has really been used (a position
			// nobody has moved to is meaningless, and on a pad-only machine there may never be
			// one), not while a gamepad is driving, and not when the pointer has left the
			// window, where the OS cursor takes over.
			bool mouseOnScreen = mouse != null && mouseEverUsed && !gamepadDriving && Application.isFocused
				&& mousePosition.x >= 0f && mousePosition.x <= Screen.width
				&& mousePosition.y >= 0f && mousePosition.y <= Screen.height;

			DrawCursor(mousePosition, mouseOnScreen);
		}

		/// Places the cursor sprite at a pointer position in screen pixels, or hides it. The
		/// size is reapplied every frame because the window can be resized at any time.
		private void DrawCursor(Vector2 screenPosition, bool pointerOnScreen)
		{
			if (cursorImage == null)
			{
				return;
			}

			bool visible = pointerOnScreen && !gameCursorHidden && cursorImage.sprite != null;
			if (cursorImage.enabled != visible)
			{
				cursorImage.enabled = visible;
				if (visible)
				{
					ResortCursorCanvas();
				}
			}

			if (!visible)
			{
				return;
			}

			cursorRect.sizeDelta = cursorBaseSize * (Screen.height / CursorReferenceHeight);
			cursorRect.anchoredPosition = screenPosition;
		}

		/// <summary>
		/// Makes Unity sort the cursor's canvas above the others again. Its order is the
		/// highest there is, but Unity can keep drawing it under an overlay canvas a game
		/// creates later (the full-screen picture of the Flash, Flixel and Phaser ports) until
		/// the order is changed - set to the same value, nothing happens. So it is nudged off
		/// and back.
		/// </summary>
		private void ResortCursorCanvas()
		{
			if (cursorCanvas == null)
			{
				return;
			}

			cursorCanvas.sortingOrder = short.MaxValue - 1;
			cursorCanvas.sortingOrder = short.MaxValue;
		}

		/// Whether a gamepad is the thing actually moving the pointer right now, not just
		/// whether one happens to be connected. InputPrompts decides which device the player
		/// is using, for the button prompts as well as for this, so the two never disagree.
		private void UpdateActiveDevice()
		{
			InputPrompts.Tick();

			// Also decides when the mouse's cursor first appears.
			mouseEverUsed = InputPrompts.MouseEverUsed;
			usingGamepadForMouse = InputPrompts.RawScheme == InputScheme.Gamepad;
		}

		/// Whether the current game has its mouse moved and clicked by a gamepad.
		public static bool MouseEmulationEnabled => instance != null && instance.mouseEmulationEnabled;

		/// <summary>
		/// The gamepad control that stands in for a mouse control while a pad is emulating the
		/// mouse ("&lt;Mouse&gt;/leftButton" gives the pad button that clicks), for prompts.
		/// Null when nothing does.
		/// </summary>
		public static string EmulatingGamepadPath(string mousePath)
		{
			if (instance == null) return null;

			InputAction action;
			switch (mousePath)
			{
				case "<Mouse>/leftButton": action = instance.mouseLeftClickAction; break;
				case "<Mouse>/rightButton": action = instance.mouseRightClickAction; break;
				case "<Mouse>/position":
				case "<Mouse>/delta": action = instance.mouseMoveAction; break;
				default: return null;
			}

			if (action == null) return null;

			foreach (InputBinding binding in action.bindings)
			{
				string path = binding.effectivePath;
				if (!string.IsNullOrEmpty(path) && path.StartsWith("<Gamepad>")) return path;
			}

			return null;
		}

		/// Positioned in raw screen pixels (anchored at the bottom-left, no CanvasScaler) so a
		/// pointer position - which follows Input.mousePosition's own screen-pixel convention -
		/// can be assigned to anchoredPosition directly. Drawn above every game's own UI.
		private void CreateCursor()
		{
			var canvasGo = new GameObject("CursorCanvas", typeof(Canvas));
			canvasGo.transform.SetParent(transform, false);
			Canvas canvas = canvasGo.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = short.MaxValue;
			cursorCanvas = canvas;

			var cursorGo = new GameObject("Cursor", typeof(RectTransform), typeof(Image));
			cursorGo.transform.SetParent(canvasGo.transform, false);
			cursorRect = cursorGo.GetComponent<RectTransform>();
			cursorRect.anchorMin = cursorRect.anchorMax = cursorRect.pivot = Vector2.zero;
			cursorRect.sizeDelta = new Vector2(DefaultCursorSize, DefaultCursorSize);

			cursorImage = cursorGo.GetComponent<Image>();
			cursorImage.raycastTarget = false;
			cursorImage.enabled = false;

			defaultCursorSprite = Resources.Load<Sprite>(CursorResourcePath);
			ApplyCursorTexture(null, Vector2.zero);
			if (defaultCursorSprite == null)
			{
				Debug.LogWarning($"GlobalInputManager: no cursor sprite at Resources/{CursorResourcePath} - the pointer still works, the cursor just won't be visible until one is added there.");
			}
		}

		/// Sets what the cursor looks like for the current game. Null means the default, the
		/// MouseEmulationCursor sprite.
		///
		/// Only the appearance changes here. Whether the cursor is shown at all is still
		/// decided in Update, so a game's texture can't override hide-when-using-gamepad.
		private void ApplyCursorTexture(Texture2D texture, Vector2 hotspot)
		{
			if (gameCursorSprite != null)
			{
				Destroy(gameCursorSprite);
				gameCursorSprite = null;
			}

			if (cursorImage == null)
			{
				return;
			}

			if (texture == null)
			{
				cursorImage.sprite = defaultCursorSprite;

				// The default sprite's click point is its own pivot, set in its import settings
				// (Sprite Editor) - so swapping the artwork means placing the pivot on the new
				// tip, with nothing to change here. Its height is DefaultCursorSize and its
				// width follows the artwork's aspect.
				if (defaultCursorSprite != null && defaultCursorSprite.rect.height > 0f)
				{
					Rect rect = defaultCursorSprite.rect;
					cursorRect.pivot = new Vector2(defaultCursorSprite.pivot.x / rect.width, defaultCursorSprite.pivot.y / rect.height);
					cursorBaseSize = new Vector2(DefaultCursorSize * rect.width / rect.height, DefaultCursorSize);
				}
				else
				{
					cursorRect.pivot = Vector2.zero;
					cursorBaseSize = new Vector2(DefaultCursorSize, DefaultCursorSize);
				}

				return;
			}

			// A game's own texture is drawn at its pixel size (on the reference screen) with
			// the hotspot as the pivot. The hotspot is measured from the top-left, a pivot from
			// the bottom-left, hence the flipped Y.
			gameCursorSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			cursorImage.sprite = gameCursorSprite;
			cursorRect.pivot = new Vector2(hotspot.x / texture.width, 1f - hotspot.y / texture.height);
			cursorBaseSize = new Vector2(texture.width, texture.height);
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
