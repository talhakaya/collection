using System;
using System.Collections.Generic;
using Collection.Controls;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Collection.UI
{
	/// One row of a MenuScreen.
	public class MenuItem
	{
		public enum Kind
		{
			Button,
			Slider,
			Choice,
			Text
		}

		public Kind kind;
		public string label;

		/// Optional: a label that changes while the screen is up. Read every frame.
		public Func<string> liveLabel;

		/// Button: what choosing it does.
		public Action submit;

		/// Slider: a value from 0 to 1.
		public Func<float> get;
		public Action<float> set;

		/// Choice: the value as text, and a step to the previous (-1) or next (+1) one.
		public Func<string> value;
		public Action<int> step;

		public bool Selectable => kind != Kind.Text;

		internal RectTransform row;
		internal Image back;
		internal TMP_Text labelText;
		internal TMP_Text valueText;
		internal RectTransform bar;
		internal RectTransform fill;
		internal Image barImage;
		internal Image fillImage;
	}

	/// <summary>
	/// A screen of the collection's own menus: a title, some words, a column of rows and a
	/// line of help. Made in code and shown with Menus.Push.
	/// </summary>
	public class MenuScreen
	{
		public string title;

		/// Words under the title. Read every frame.
		public Func<string> body;

		/// A line above the controls at the bottom. Read every frame.
		public Func<string> footer;

		public readonly List<MenuItem> items = new List<MenuItem>();

		/// What Cancel does. Null takes the screen away (Menus.Pop).
		public Action cancel;

		/// Hides what is behind completely, rather than dimming it.
		public bool opaque;

		/// The row the screen opens on.
		public int selected;

		public MenuItem Button(string label, Action submit)
		{
			return Add(new MenuItem { kind = MenuItem.Kind.Button, label = label, submit = submit });
		}

		public MenuItem Slider(string label, Func<float> get, Action<float> set)
		{
			return Add(new MenuItem { kind = MenuItem.Kind.Slider, label = label, get = get, set = set });
		}

		public MenuItem Choice(string label, Func<string> value, Action<int> step)
		{
			return Add(new MenuItem { kind = MenuItem.Kind.Choice, label = label, value = value, step = step });
		}

		public MenuItem Text(Func<string> text)
		{
			return Add(new MenuItem { kind = MenuItem.Kind.Text, liveLabel = text });
		}

		private MenuItem Add(MenuItem item)
		{
			items.Add(item);
			return item;
		}
	}

	/// <summary>
	/// The collection's own menus: the main menu's screens, the settings and the pause
	/// screen. One stack of screens on a canvas above every game, the top one shown.
	///
	/// It does not use the scene's EventSystem. A game may have one of its own, set up any
	/// way, or none; so the rows are driven straight from the Global action map (MenuMove,
	/// MenuSubmit, MenuCancel) and the mouse. While a screen is up, whatever EventSystem
	/// there is has its input modules switched off, and the game gets no input
	/// (TaloketoInputManager.Blocked) until the buttons that closed the menu are let go.
	///
	/// Placeholder looks.
	/// </summary>
	[DefaultExecutionOrder(-10000)]
	public class Menus : MonoBehaviour
	{
		private const string AssetResourcePath = "Input/CollectionInput";
		private const float RowWidth = 820f;
		private const float RowHeight = 68f;
		private const float RowGap = 10f;
		private const float RepeatDelay = 0.4f;
		private const float RepeatRate = 0.12f;
		private const float SliderStep = 0.05f;

		private const string Controls =
			"{<Keyboard>/upArrow|<Keyboard>/downArrow|<Keyboard>/leftArrow|<Keyboard>/rightArrow|<Gamepad>/dpad} Move"
			+ "      {<Keyboard>/enter|<Gamepad>/buttonSouth} Select"
			+ "      {<Keyboard>/escape|<Gamepad>/buttonEast} Back";

		private static readonly Color RowColour = new Color(1f, 1f, 1f, 0.025f);
		private static readonly Color RowSelectedColour = new Color(1f, 1f, 1f, 0.95f);
		private static readonly Color TextColour = new Color(0.92f, 0.92f, 0.92f, 1f);
		private static readonly Color TextSelectedColour = new Color(0.05f, 0.05f, 0.05f, 1f);
		private static readonly Color QuietTextColour = new Color(0.65f, 0.65f, 0.65f, 1f);

		private static Menus instance;
		private static readonly List<MenuScreen> stack = new List<MenuScreen>();

		private InputAction moveAction;
		private InputAction submitAction;
		private InputAction cancelAction;
		private InputAction pauseAction;

		private GameObject root;
		private Image dim;
		private TMP_Text titleText;
		private TMP_Text bodyText;
		private TMP_Text footerText;
		private TMP_Text controlsText;
		private RectTransform rows;

		private MenuScreen built;
		private int changedFrame = -1;
		private bool waitingForRelease;
		private readonly List<BaseInputModule> suspendedModules = new List<BaseInputModule>();
		private readonly List<BaseInputModule> scratchModules = new List<BaseInputModule>();

		private Vector2Int heldDirection;
		private float nextRepeat;
		private MenuItem dragging;
		private string controlsShown;

		public static bool IsOpen => stack.Count > 0;

		public static MenuScreen Top => stack.Count > 0 ? stack[stack.Count - 1] : null;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			stack.Clear();
			var go = new GameObject(nameof(Menus));
			DontDestroyOnLoad(go);
			instance = go.AddComponent<Menus>();
		}

		/// Shows a screen over whatever is up.
		public static void Push(MenuScreen screen)
		{
			stack.Add(screen);
			Changed();
		}

		/// Takes the top screen away, back to the one under it (or to no menu).
		public static void Pop()
		{
			if (stack.Count == 0) return;

			stack.RemoveAt(stack.Count - 1);
			Changed();
		}

		/// Takes screens away until this one is on top. All of them if it is not there.
		public static void PopTo(MenuScreen screen)
		{
			while (stack.Count > 0 && Top != screen)
			{
				stack.RemoveAt(stack.Count - 1);
			}

			Changed();
		}

		public static void CloseAll()
		{
			stack.Clear();
			Changed();
		}

		private static void Changed()
		{
			if (instance == null) return;

			instance.changedFrame = Time.frameCount;
			instance.dragging = null;
			if (stack.Count > 0)
			{
				// At once, not at the next Update: a screen can open from an input callback,
				// before the game's scripts have run for the frame.
				TaloketoInputManager.Blocked = true;
			}
			else
			{
				instance.waitingForRelease = true;
			}

			instance.Show();
		}

		private void Awake()
		{
			InputActionAsset asset = Resources.Load<InputActionAsset>(AssetResourcePath);
			InputActionMap map = asset != null ? asset.FindActionMap("Global") : null;
			if (map == null)
			{
				Debug.LogError($"Menus: no Global action map in '{AssetResourcePath}'.");
				return;
			}

			moveAction = map.FindAction("MenuMove");
			submitAction = map.FindAction("MenuSubmit");
			cancelAction = map.FindAction("MenuCancel");
			pauseAction = map.FindAction("Pause");
			moveAction.Enable();
			submitAction.Enable();
			cancelAction.Enable();
			pauseAction.Enable();

			BuildCanvas();
			InputPrompts.Changed += OnPromptsChanged;
		}

		private void OnDestroy()
		{
			InputPrompts.Changed -= OnPromptsChanged;
		}

		private void OnPromptsChanged()
		{
			controlsShown = null;
		}

		// ---- Every frame -----------------------------------------------------------------

		private void Update()
		{
			if (root == null) return;

			if (stack.Count == 0)
			{
				if (waitingForRelease && NothingHeld())
				{
					waitingForRelease = false;
					ResumeModules();
					TaloketoInputManager.Blocked = false;
				}

				return;
			}

			TaloketoInputManager.Blocked = true;
			SuspendModules();

			MenuScreen screen = Top;
			if (built != screen) Show();

			// A screen that opened or closed this frame was opened by a press that is still
			// "pressed this frame". It is not also a press on the new screen.
			if (Time.frameCount != changedFrame)
			{
				HandleInput(screen);
			}

			if (Top == screen)
			{
				Refresh(screen);
			}
		}

		private bool NothingHeld()
		{
			Mouse mouse = Mouse.current;
			return !submitAction.IsPressed() && !cancelAction.IsPressed() && !pauseAction.IsPressed()
				&& (mouse == null || !mouse.leftButton.isPressed);
		}

		/// Switches off the input modules of the scene's EventSystem, so its buttons do not
		/// answer to what is pressed in the menu. The EventSystem itself stays, for scripts
		/// that expect EventSystem.current.
		private void SuspendModules()
		{
			EventSystem events = EventSystem.current;
			if (events == null) return;

			events.GetComponents(scratchModules);
			foreach (BaseInputModule module in scratchModules)
			{
				if (module.enabled)
				{
					module.enabled = false;
					suspendedModules.Add(module);
				}
			}
		}

		private void ResumeModules()
		{
			foreach (BaseInputModule module in suspendedModules)
			{
				if (module != null) module.enabled = true;
			}

			suspendedModules.Clear();
		}

		private void HandleInput(MenuScreen screen)
		{
			if (cancelAction.WasPressedThisFrame())
			{
				if (screen.cancel != null) screen.cancel();
				else Pop();
				return;
			}

			MenuItem selected = screen.selected >= 0 && screen.selected < screen.items.Count ? screen.items[screen.selected] : null;

			if (submitAction.WasPressedThisFrame() && selected != null)
			{
				if (selected.kind == MenuItem.Kind.Button) selected.submit?.Invoke();
				else if (selected.kind == MenuItem.Kind.Choice) selected.step?.Invoke(1);
				return;
			}

			Vector2Int direction = Direction(moveAction.ReadValue<Vector2>());
			bool step = false;
			if (direction != heldDirection)
			{
				heldDirection = direction;
				nextRepeat = Time.unscaledTime + RepeatDelay;
				step = direction != Vector2Int.zero;
			}
			else if (direction != Vector2Int.zero && Time.unscaledTime >= nextRepeat)
			{
				nextRepeat = Time.unscaledTime + RepeatRate;
				step = true;
			}

			if (step)
			{
				if (direction.y != 0)
				{
					MoveSelection(screen, -direction.y);
				}
				else if (selected != null)
				{
					Adjust(selected, direction.x);
				}
			}

			if (HandleMouse(screen)) return;
		}

		private static Vector2Int Direction(Vector2 move)
		{
			if (Mathf.Abs(move.y) >= Mathf.Abs(move.x))
			{
				return Mathf.Abs(move.y) > 0.5f ? new Vector2Int(0, move.y > 0f ? 1 : -1) : Vector2Int.zero;
			}

			return Mathf.Abs(move.x) > 0.5f ? new Vector2Int(move.x > 0f ? 1 : -1, 0) : Vector2Int.zero;
		}

		private static void MoveSelection(MenuScreen screen, int by)
		{
			int count = screen.items.Count;
			for (int i = 1; i <= count; i++)
			{
				int at = ((screen.selected + by * i) % count + count) % count;
				if (screen.items[at].Selectable)
				{
					screen.selected = at;
					return;
				}
			}
		}

		private static void Adjust(MenuItem item, int by)
		{
			if (item.kind == MenuItem.Kind.Slider)
			{
				float value = Mathf.Round((item.get() + by * SliderStep) / SliderStep) * SliderStep;
				item.set(Mathf.Clamp01(value));
			}
			else if (item.kind == MenuItem.Kind.Choice)
			{
				item.step?.Invoke(by);
			}
		}

		/// The mouse: moving it over a row selects the row, a click chooses it, and a slider
		/// is dragged. True when a click was taken.
		private bool HandleMouse(MenuScreen screen)
		{
			Mouse mouse = Mouse.current;
			if (mouse == null || !InputPrompts.MouseEverUsed) return false;

			Vector2 position = mouse.position.ReadValue();

			if (dragging != null)
			{
				if (!mouse.leftButton.isPressed)
				{
					dragging = null;
				}
				else
				{
					DragSlider(dragging, position);
				}

				return true;
			}

			bool moved = mouse.delta.ReadValue() != Vector2.zero;
			bool clicked = mouse.leftButton.wasPressedThisFrame;
			if (!moved && !clicked) return false;

			for (int i = 0; i < screen.items.Count; i++)
			{
				MenuItem item = screen.items[i];
				if (!item.Selectable || item.row == null) continue;
				if (!RectTransformUtility.RectangleContainsScreenPoint(item.row, position, null)) continue;

				screen.selected = i;
				if (!clicked) return false;

				switch (item.kind)
				{
					case MenuItem.Kind.Button:
						item.submit?.Invoke();
						break;
					case MenuItem.Kind.Choice:
						// The left half of the value steps back, the rest forward.
						RectTransformUtility.ScreenPointToLocalPointInRectangle(item.row, position, null, out Vector2 local);
						item.step?.Invoke(local.x > 0f && local.x < RowWidth * 0.25f ? -1 : 1);
						break;
					case MenuItem.Kind.Slider:
						dragging = item;
						DragSlider(item, position);
						break;
				}

				return true;
			}

			return false;
		}

		private static void DragSlider(MenuItem item, Vector2 screenPosition)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(item.bar, screenPosition, null, out Vector2 local);
			Rect rect = item.bar.rect;
			float value = Mathf.Clamp01((local.x - rect.xMin) / rect.width);
			item.set(Mathf.Round(value / 0.01f) * 0.01f);
		}

		// ---- Drawing ---------------------------------------------------------------------

		private void BuildCanvas()
		{
			root = new GameObject("MenuCanvas", typeof(Canvas), typeof(CanvasScaler));
			root.transform.SetParent(transform, false);
			Canvas canvas = root.GetComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;

			// Above every game's own canvases, under the cursor (GlobalInputManager).
			canvas.sortingOrder = short.MaxValue - 10;

			CanvasScaler scaler = root.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1920f, 1080f);
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			scaler.matchWidthOrHeight = 1f;

			var dimObject = new GameObject("Dim", typeof(RectTransform), typeof(Image));
			var dimRect = (RectTransform)dimObject.transform;
			dimRect.SetParent(root.transform, false);
			dimRect.anchorMin = Vector2.zero;
			dimRect.anchorMax = Vector2.one;
			dimRect.offsetMin = dimRect.offsetMax = Vector2.zero;
			dim = dimObject.GetComponent<Image>();
			dim.raycastTarget = false;

			titleText = NewText("Title", root.transform, 84f, TextAlignmentOptions.Center);
			Place(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -150f), new Vector2(1700f, 120f));

			bodyText = NewText("Body", root.transform, 34f, TextAlignmentOptions.Top);
			bodyText.color = QuietTextColour;
			Place(bodyText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -280f), new Vector2(1500f, 120f));

			var rowsObject = new GameObject("Rows", typeof(RectTransform));
			rows = (RectTransform)rowsObject.transform;
			rows.SetParent(root.transform, false);
			Place(rows, new Vector2(0.5f, 0.5f), new Vector2(0f, -60f), new Vector2(RowWidth, 0f));

			footerText = NewText("Footer", root.transform, 30f, TextAlignmentOptions.Center);
			footerText.color = QuietTextColour;
			Place(footerText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 130f), new Vector2(1700f, 50f));

			controlsText = NewText("Controls", root.transform, 28f, TextAlignmentOptions.Center);
			controlsText.color = QuietTextColour;
			controlsText.spriteAsset = InputPrompts.SpriteAsset();
			Place(controlsText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 60f), new Vector2(1700f, 60f));

			root.SetActive(false);
		}

		private static void Place(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
		{
			rect.anchorMin = rect.anchorMax = anchor;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = position;
			rect.sizeDelta = size;
		}

		private static TMP_Text NewText(string name, Transform parent, float size, TextAlignmentOptions alignment)
		{
			var go = new GameObject(name, typeof(RectTransform));
			go.transform.SetParent(parent, false);
			TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
			text.fontSize = size;
			text.alignment = alignment;
			text.color = TextColour;
			text.raycastTarget = false;
			text.textWrappingMode = TextWrappingModes.Normal;
			text.overflowMode = TextOverflowModes.Overflow;
			return text;
		}

		/// Puts the top screen's rows on the canvas, or hides the canvas when there is none.
		private void Show()
		{
			if (root == null) return;

			MenuScreen screen = Top;
			built = screen;
			for (int i = rows.childCount - 1; i >= 0; i--)
			{
				Destroy(rows.GetChild(i).gameObject);
			}

			root.SetActive(screen != null);
			if (screen == null) return;

			// A screen opened from an opaque one (the settings from the main menu) is opaque too.
			// (The project is in linear colour, where see-through black darkens far less than
			// its alpha suggests; hence an alpha this close to 1 for the dimming.)
			bool opaque = false;
			foreach (MenuScreen open in stack) opaque |= open.opaque;
			dim.color = opaque ? new Color(0.06f, 0.06f, 0.08f, 1f) : new Color(0f, 0f, 0f, 0.975f);
			titleText.text = screen.title ?? "";

			int count = screen.items.Count;
			float total = count * RowHeight + (count - 1) * RowGap;
			for (int i = 0; i < count; i++)
			{
				BuildRow(screen.items[i], total * 0.5f - RowHeight * 0.5f - i * (RowHeight + RowGap));
			}

			// The rows sit in the middle of the screen, and the text over them starts at a fixed
			// height: when it is long (a game's description) they move down to stay clear of it.
			SetText(bodyText, screen.body != null ? screen.body() : "");
			bodyText.ForceMeshUpdate();
			// Heights from the middle of the 1080-high canvas: the text's top is 220 from the
			// canvas's top, and the rows' own middle is 60 under the canvas's.
			float bodyBottom = 540f - 220f - (string.IsNullOrEmpty(bodyText.text) ? 0f : bodyText.preferredHeight);
			float down = Mathf.Max(0f, -60f + total * 0.5f - (bodyBottom - 40f));
			rows.anchoredPosition = new Vector2(0f, -60f - down);

			if (screen.selected < 0 || screen.selected >= count || !screen.items[screen.selected].Selectable)
			{
				screen.selected = 0;
				if (count > 0 && !screen.items[0].Selectable) MoveSelection(screen, 1);
			}

			heldDirection = Direction(moveAction.ReadValue<Vector2>());
			nextRepeat = Time.unscaledTime + RepeatDelay;
			Refresh(screen);
		}

		private void BuildRow(MenuItem item, float y)
		{
			var go = new GameObject(item.label ?? "Row", typeof(RectTransform), typeof(Image));
			item.row = (RectTransform)go.transform;
			item.row.SetParent(rows, false);
			Place(item.row, new Vector2(0.5f, 0.5f), new Vector2(0f, y), new Vector2(RowWidth, RowHeight));
			item.back = go.GetComponent<Image>();
			item.back.raycastTarget = false;
			item.back.enabled = item.Selectable;

			bool wide = item.kind == MenuItem.Kind.Button || item.kind == MenuItem.Kind.Text;
			item.labelText = NewText("Label", item.row, item.kind == MenuItem.Kind.Text ? 30f : 36f,
				wide ? TextAlignmentOptions.Center : TextAlignmentOptions.Left);
			item.labelText.textWrappingMode = TextWrappingModes.NoWrap;
			Place(item.labelText.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(RowWidth - 56f, RowHeight));

			item.valueText = null;
			item.bar = item.fill = null;
			if (wide) return;

			item.valueText = NewText("Value", item.row, 36f, TextAlignmentOptions.Right);
			item.valueText.textWrappingMode = TextWrappingModes.NoWrap;
			Place(item.valueText.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(RowWidth - 56f, RowHeight));

			if (item.kind != MenuItem.Kind.Slider) return;

			var barObject = new GameObject("Bar", typeof(RectTransform), typeof(Image));
			item.bar = (RectTransform)barObject.transform;
			item.bar.SetParent(item.row, false);
			Place(item.bar, new Vector2(0.5f, 0.5f), new Vector2(110f, 0f), new Vector2(300f, 10f));
			item.barImage = barObject.GetComponent<Image>();
			item.barImage.raycastTarget = false;

			var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
			item.fill = (RectTransform)fillObject.transform;
			item.fill.SetParent(item.bar, false);
			item.fill.anchorMin = Vector2.zero;
			item.fill.anchorMax = new Vector2(0f, 1f);
			item.fill.pivot = new Vector2(0f, 0.5f);
			item.fill.offsetMin = new Vector2(0f, -5f);
			item.fill.offsetMax = new Vector2(0f, 5f);
			item.fillImage = fillObject.GetComponent<Image>();
			item.fillImage.raycastTarget = false;
		}

		private void Refresh(MenuScreen screen)
		{
			SetText(bodyText, screen.body != null ? screen.body() : "");
			SetText(footerText, screen.footer != null ? screen.footer() : "");

			// The glyphs follow the device, and the generic pad's ones swap as they animate.
			string controls = InputPrompts.Format(Controls, out bool _);
			if (controls != controlsShown)
			{
				controlsShown = controls;
				controlsText.text = controls;
			}

			for (int i = 0; i < screen.items.Count; i++)
			{
				MenuItem item = screen.items[i];
				if (item.row == null) continue;

				bool selected = i == screen.selected && item.Selectable;
				Color text = !item.Selectable ? QuietTextColour : selected ? TextSelectedColour : TextColour;
				item.back.color = selected ? RowSelectedColour : RowColour;
				item.labelText.color = text;
				SetText(item.labelText, item.liveLabel != null ? item.liveLabel() : item.label);

				if (item.kind == MenuItem.Kind.Choice)
				{
					item.valueText.color = text;
					SetText(item.valueText, "<  " + item.value() + "  >");
				}
				else if (item.kind == MenuItem.Kind.Slider)
				{
					float value = Mathf.Clamp01(item.get());
					item.valueText.color = text;
					SetText(item.valueText, Mathf.RoundToInt(value * 100f) + "%");
					item.fill.anchorMax = new Vector2(value, 1f);
					item.fillImage.color = text;
					item.barImage.color = new Color(text.r, text.g, text.b, 0.25f);
				}
			}
		}

		private static void SetText(TMP_Text text, string value)
		{
			value = value ?? "";
			if (text.text != value)
			{
				text.text = value;
			}
		}
	}
}
