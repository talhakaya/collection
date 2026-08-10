using System.Collections.Generic;
using Collection.Controls;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Games.Golfinity
{
	/// <summary>
	/// Gamepad navigation for Golfinity. Golfinity is a mouse game end to end - the map is
	/// world-space objects hit-tested against the cursor, the popups are uGUI - so rather
	/// than bolting selection state onto each screen, this drives one on-screen cursor that
	/// snaps to whatever is currently selected, and moves that selection discretely.
	///
	/// Deliberately does NOT handle Back/B: Game.Update already runs the whole
	/// popup -> popup -> back-to-map priority chain off the "Back" action, which Escape and
	/// gamepad B both feed. Duplicating it here would double-fire.
	///
	/// Aiming lives in GolfBall (it owns aimAngle/aimLength); this only covers menus.
	/// </summary>
	public class GolfinityGamepad : MonoBehaviour
	{
		[Header("Cursor")]
		[Tooltip("Sprite drawn over the current selection. Swap freely - nothing depends on which sprite it is.")]
		public Sprite cursorSprite;
		[Tooltip("Cursor height in canvas units. Width follows the sprite's own aspect ratio, so swapping in a differently-shaped sprite won't squash it.")]
		public float cursorHeight = 16f;
		[Tooltip("Offset from the selected item's centre, in canvas units.")]
		public Vector2 cursorOffset = new Vector2(6f, -6f);
		[Tooltip("Put the cursor on the other side of the selection: mirrors the sprite and flips the offset's X, so one tick moves it across and turns it to face the item.")]
		public bool mirrorCursor;

		[Header("Map navigation")]
		[Tooltip("World units/sec the right stick free-scrolls the map. Fixed speed, no inertia.")]
		public float scrollSpeed = 40f;
		[Tooltip("Seconds for a normal step to an adjacent level. Kept separate from scrollBackDuration so stepping stays snappy while a long return from a peek still takes a consistent, longer beat.")]
		public float stepDuration = 0.14f;
		[Tooltip("Seconds to scroll back after peeking away with the right stick. Fixed, so the speed varies with how far the player wandered.")]
		public float scrollBackDuration = 0.35f;
		[Tooltip("Travel beyond this many level-spacings counts as a return-from-peek and uses scrollBackDuration instead of stepDuration.")]
		public float peekReturnSpacings = 1.5f;
		[Tooltip("Limit how far back-scrolling can go past the first level.")]
		public bool clampToFirstLevel = true;
		[Tooltip("Where back-scrolling stops, as the first level's offset from centre in world units. 0 stops with it centred; negative stops earlier, showing less empty scenery to its left.")]
		public float leftStopOffset = 0f;
		[Tooltip("Stick deflection needed to register a level step.")]
		public float stepThreshold = 0.5f;
		public float stepRepeatDelay = 0.4f;
		public float stepRepeatRate = 0.18f;

		[Header("Shortcut labels (temporary)")]
		[Tooltip("Show which button triggers each top-bar action. Debug aid - turn off to hide them all.")]
		public bool showShortcutLabels = true;
		[Tooltip("Gap below the button, in canvas units.")]
		public float shortcutLabelGap = 1f;
		public float shortcutLabelFontSize = 8f;

		private const int NoTarget = -1;
		private int anchorHole = NoTarget;
		private bool scrollingBack;
		private int scrollBackTo;
		private float scrollBackElapsed;
		private float scrollBackTotal;
		private float scrollBackApplied;
		private float scrollBackSeconds;

		private struct ShortcutLabel
		{
			public TextMeshProUGUI text;
			public string actionName;
			public string lastGlyph;
			public RectTransform button;
			public bool hideWhenPopupOpen;
			public bool positioned;
		}

		private int lastPadId;
		private bool usingGamepad;

		private readonly List<ShortcutLabel> shortcutLabels = new List<ShortcutLabel>();

		private RectTransform canvasRect;
		private RectTransform cursorRect;
		private Image cursorImage;
		private GameObject activePopup;
		private float stepCooldown;
		private bool stepLatched;
		private bool mapWasActive;

		private void Start()
		{
			Canvas canvas = UIReferences.instance != null
				? UIReferences.instance.GetComponentInParent<Canvas>()
				: FindFirstObjectByType<Canvas>();
			if (canvas == null)
			{
				enabled = false;
				return;
			}

			canvasRect = canvas.GetComponent<RectTransform>();
			CreateCursor(canvas);
			CreateShortcutLabels(canvas);
		}

		/// Labels the top-bar buttons with the pad button that triggers them. Text comes from
		/// the binding itself rather than hardcoded letters, so it stays honest if the action
		/// is rebound. Parented to each button, so buttonMap's label inherits its show/hide
		/// (it only exists during Play).
		private void CreateShortcutLabels(Canvas canvas)
		{
			if (!showShortcutLabels) return;

			TMP_FontAsset font = null;
			TextMeshProUGUI sample = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
			if (sample != null) font = sample.font;

			// Top bar: only usable when no popup is covering it, so these hide with one open.
			CreateShortcutLabel(canvas, "buttonOptions", "Settings", font, true);
			CreateShortcutLabel(canvas, "buttonUpgrade", "Upgrade", font, true);
			CreateShortcutLabel(canvas, "buttonMap", "Back", font, true);

			// Level score popup. Its buttons are only active while it's up, so they need no
			// extra visibility rule. buttonRetry goes to the map despite the name.
			CreateShortcutLabel(canvas, "LevelScorePopup/Buttons/buttonRetry", "Back", font, false);
			CreateShortcutLabel(canvas, "LevelScorePopup/Buttons/buttonPlay", "Throw", font, false);
		}

		private void CreateShortcutLabel(Canvas canvas, string buttonPath, string actionName, TMP_FontAsset font, bool hideWhenPopupOpen)
		{
			var button = canvas.transform.Find(buttonPath) as RectTransform;
			if (button == null) return;

			// Parented to the canvas, not the button. The top-bar buttons are zero-size rects
			// at localScale 0.15 with their artwork in a 96x96 child, so a label parented to
			// one inherits that 0.15 and renders about four pixels tall.
			var go = new GameObject("ShortcutLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
			var rect = go.GetComponent<RectTransform>();
			rect.SetParent(canvas.transform, false);
			rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
			rect.pivot = new Vector2(0.5f, 1f);
			rect.sizeDelta = new Vector2(40f, shortcutLabelFontSize * 1.6f);

			var text = go.GetComponent<TextMeshProUGUI>();
			if (font != null) text.font = font;
			text.fontSize = shortcutLabelFontSize;
			text.alignment = TextAlignmentOptions.Top;
			text.raycastTarget = false;
			text.textWrappingMode = TextWrappingModes.NoWrap;

			shortcutLabels.Add(new ShortcutLabel
			{
				text = text,
				actionName = actionName,
				button = button,
				hideWhenPopupOpen = hideWhenPopupOpen,
			});
		}

		/// A gamepad being connected doesn't mean it's the thing driving the game right now - it
		/// might just be sitting there while the player uses mouse/keyboard, and the prompts
		/// would be lying. Compares each device's own InputSystem timestamp for its last actual
		/// input event, so whichever was touched most recently wins.
		private void UpdateActiveDevice()
		{
			Gamepad pad = Gamepad.current;
			if (pad == null)
			{
				usingGamepad = false;
				return;
			}

			double pointerTime = 0.0;
			if (Mouse.current != null) pointerTime = System.Math.Max(pointerTime, Mouse.current.lastUpdateTime);
			if (Keyboard.current != null) pointerTime = System.Math.Max(pointerTime, Keyboard.current.lastUpdateTime);

			if (pad.lastUpdateTime > pointerTime) usingGamepad = true;
			else if (pointerTime > pad.lastUpdateTime) usingGamepad = false;
			// Equal (neither touched yet this session) - keep whatever it already was.
		}

		/// Refreshes the labels: hidden unless the player is actively driving with a pad right
		/// now, and re-read from the binding so a rebind shows up. Only writes when the string
		/// actually changes - assigning TMP.text every frame forces a mesh rebuild.
		private void UpdateShortcutLabels()
		{
			if (shortcutLabels.Count == 0) return;

			Gamepad pad = Gamepad.current;

			// Resolving a binding to its display string allocates, so it's done only when the
			// pad actually changes rather than every frame for every label.
			int padId = pad != null ? pad.deviceId : 0;
			bool bindingsMayHaveChanged = padId != lastPadId;
			lastPadId = padId;

			for (int i = 0; i < shortcutLabels.Count; i++)
			{
				ShortcutLabel label = shortcutLabels[i];
				if (label.text == null || label.button == null) continue;

				// The label isn't a child of its button any more, so it has to mirror the
				// button's visibility itself - buttonMap only exists during Play. The top bar
				// also stays active behind an open popup, but its shortcuts don't fire then,
				// so advertising them would be a lie.
				bool visible = usingGamepad
					&& label.button.gameObject.activeInHierarchy
					&& !(label.hideWhenPopupOpen && activePopup != null);
				if (label.text.enabled != visible) label.text.enabled = visible;
				if (!visible) continue;

				// These buttons don't move, so their position is measured once - the bounds
				// call walks the whole child hierarchy and isn't worth repeating every frame.
				if (!label.positioned)
				{
					Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvasRect, label.button);
					label.text.rectTransform.anchoredPosition =
						new Vector2(bounds.center.x, bounds.min.y - shortcutLabelGap);
					label.positioned = true;
					shortcutLabels[i] = label;
				}

				if (bindingsMayHaveChanged || label.lastGlyph == null)
				{
					string glyph = GamepadGlyph(label.actionName);
					if (glyph != label.lastGlyph)
					{
						label.text.text = glyph;
						label.lastGlyph = glyph;
						shortcutLabels[i] = label;
					}
				}
			}
		}

		/// The pad button bound to an action, as a short display string ("X", "Y", "B").
		private static string GamepadGlyph(string actionName)
		{
			InputAction action = TaloketoInputManager.GetAction(actionName);
			if (action == null) return "";

			for (int i = 0; i < action.bindings.Count; i++)
			{
				InputBinding binding = action.bindings[i];
				if (binding.path != null && binding.path.StartsWith("<Gamepad>"))
				{
					return action.GetBindingDisplayString(i);
				}
			}

			return "";
		}

		private void CreateCursor(Canvas canvas)
		{
			var go = new GameObject("GamepadCursor", typeof(RectTransform), typeof(Image));
			cursorRect = go.GetComponent<RectTransform>();
			cursorRect.SetParent(canvas.transform, false);
			cursorRect.anchorMin = cursorRect.anchorMax = cursorRect.pivot = new Vector2(0.5f, 0.5f);

			cursorImage = go.GetComponent<Image>();
			cursorImage.sprite = cursorSprite;
			cursorImage.raycastTarget = false;
			cursorImage.enabled = false;

			ApplyCursorSize();
		}

		/// Sizes the cursor from its height, taking width from the sprite's own aspect so a
		/// non-square sprite (hand0 is 70x93) isn't stretched.
		private void ApplyCursorSize()
		{
			float aspect = 1f;
			if (cursorSprite != null && cursorSprite.rect.height > 0f)
			{
				aspect = cursorSprite.rect.width / cursorSprite.rect.height;
			}

			cursorRect.sizeDelta = new Vector2(cursorHeight * aspect, cursorHeight);
			cursorRect.localScale = new Vector3(mirrorCursor ? -1f : 1f, 1f, 1f);
		}

		private void Update()
		{
			if (cursorRect == null) return;

			// The cursor has to stay on top of popups shown after it was created, but
			// reordering siblings dirties the canvas and forces a full UGUI rebuild, so this
			// must not run every frame - only when the layering could actually have changed.

			UpdateActiveDevice();
			UpdateShortcutLabels();

			GameObject popup = GetActivePopup();
			if (popup != activePopup)
			{
				activePopup = popup;
				cursorRect.SetAsLastSibling();
				if (popup != null) OnPopupOpened(popup);
			}

			if (popup != null)
			{
				HandlePopupShortcuts(popup);
				PositionCursorAt(EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null);
				return;
			}

			HandleTopButtonShortcuts();

			if (Game.state == GameState.Map)
			{
				UpdateMap();
			}
			else
			{
				// Reset here, not in UpdateMap - it doesn't run at all during Play, so the
				// map would never re-sync its selection on the way back from a level.
				mapWasActive = false;
				ShowCursor(false);
			}
		}

		/// Mirrors Game.Update's Escape priority chain so "which screen owns input" is
		/// decided in exactly one order, not two that can drift apart.
		private static GameObject GetActivePopup()
		{
			if (UIReferences.instance == null) return null;
			if (UIReferences.optionsPopup.gameObject.activeSelf) return UIReferences.optionsPopup.gameObject;
			if (UIReferences.levelScorePopup.gameObject.activeSelf) return UIReferences.levelScorePopup.gameObject;
			if (UIReferences.cheatPopup.gameObject.activeSelf) return UIReferences.cheatPopup.gameObject;
			if (UIReferences.upgradePopup.gameObject.activeSelf) return UIReferences.upgradePopup.gameObject;
			return null;
		}

		/// Wires the popup's controls into an explicit navigation ring and selects the first.
		///
		/// The explicit ring is not optional: uGUI's Automatic navigation picks neighbours
		/// geometrically across *every* Selectable in the scene, so pressing down inside the
		/// options list jumps out to the always-present buttons behind the popup (the cheat
		/// button, the top bar) instead of moving to the next setting.
		///
		/// Links both axes to the same prev/next so either stick direction walks the list,
		/// which keeps it correct whether the popup lays its buttons out in a column
		/// (options) or a row (level score).
		/// The level score popup drives its two real buttons from dedicated keys instead of a
		/// moving selection, so it gets no initial selection - otherwise Submit and the direct
		/// shortcut would both fire. Its layout also puts the map button leftmost, which would
		/// make it the default and quietly turn "confirm" into "leave the level".
		private static void OnPopupOpened(GameObject popup)
		{
			if (UsesDirectShortcuts(popup))
			{
				if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
				return;
			}

			SelectFirstIn(popup);
		}

		private static bool UsesDirectShortcuts(GameObject popup)
		{
			return UIReferences.instance != null && popup == UIReferences.levelScorePopup.gameObject;
		}

		private static void HandlePopupShortcuts(GameObject popup)
		{
			if (!UsesDirectShortcuts(popup)) return;

			// Back is left to Game.Update, which owns the whole popup priority chain.
			//
			// While the reveal is still animating, Throw only speeds it up (LevelScorePopup.Update
			// reads it as held, same as Game.input) - it must not also advance to the next level,
			// or holding it through the animation would skip straight past the score reveal.
			if (TaloketoInputManager.GetButtonDown("Throw") && UIReferences.levelScorePopup.ButtonsShown)
			{
				UIReferences.levelScorePopup.OnClickNext();
			}
		}

		private static void SelectFirstIn(GameObject popup)
		{
			if (EventSystem.current == null) return;

			var items = new System.Collections.Generic.List<Selectable>();
			foreach (Selectable s in popup.GetComponentsInChildren<Selectable>(false))
			{
				if (s.interactable && s.gameObject.activeInHierarchy) items.Add(s);
			}

			if (items.Count == 0)
			{
				EventSystem.current.SetSelectedGameObject(null);
				return;
			}

			// Reading order: top to bottom, then left to right.
			items.Sort((a, b) =>
			{
				float dy = b.transform.position.y - a.transform.position.y;
				if (Mathf.Abs(dy) > 0.01f) return dy > 0f ? 1 : -1;
				return a.transform.position.x.CompareTo(b.transform.position.x);
			});

			for (int i = 0; i < items.Count; i++)
			{
				Selectable prev = items[(i - 1 + items.Count) % items.Count];
				Selectable next = items[(i + 1) % items.Count];
				Navigation nav = items[i].navigation;
				nav.mode = Navigation.Mode.Explicit;
				nav.selectOnUp = nav.selectOnLeft = prev;
				nav.selectOnDown = nav.selectOnRight = next;
				items[i].navigation = nav;
			}

			EventSystem.current.SetSelectedGameObject(items[0].gameObject);
		}

		private void HandleTopButtonShortcuts()
		{
			if (Game.instance == null) return;
			if (TaloketoInputManager.GetButtonDown("Settings")) Game.instance.OnClickOptions();
			// buttonUpgrade is hidden now that ads/upgrades are gone - mirror that here too,
			// same as the shortcut label already does, so Y doesn't open a popup for a button
			// that's no longer on screen.
			else if (TaloketoInputManager.GetButtonDown("Upgrade") && Game.instance.buttonUpgrade.gameObject.activeInHierarchy)
			{
				Game.instance.OnClickUpgrade();
			}
		}

		private void UpdateMap()
		{
			Map map = Game.instance != null ? Game.instance.map : null;
			if (map == null || !map.gameObject.activeInHierarchy)
			{
				mapWasActive = false;
				ShowCursor(false);
				return;
			}

			if (!mapWasActive)
			{
				mapWasActive = true;
				SelectUnbeatenLevel(map);
			}

			// A scroll-back owns the map until it finishes. Input stays locked for its whole
			// duration so a press can't land on whichever level happens to be sliding past.
			if (UpdateScrollBack(map)) return;

			MapUi focused = map.GetFocusedUi();

			// Right stick free-scrolls at fixed speed with no inertia. This is a "peek": it
			// deliberately does NOT commit a new selection, it just moves the view (and the
			// highlight follows whatever is nearest, as before). Forward is unbounded, like
			// the mouse drag; only the left edge is limited.
			float scroll = TaloketoInputManager.GetVector2("ScrollMap").x;
			bool pastLeftLimit = clampToFirstLevel && scroll < 0f && IsAtLeftLimit(map);
			if (!Mathf.Approximately(scroll, 0f) && !pastLeftLimit)
			{
				map.ScrollBy(-scroll * scrollSpeed * Time.deltaTime);
				focused = map.GetFocusedUi();
			}

			// The committed selection tracks the highlight while peeking, but survives being
			// scrolled past the end of the unlocked run (where GetFocusedUi goes null), so
			// there's always somewhere to come back to.
			if (focused is MapLevelUi lit) anchorHole = lit.holeNo;

			// Left stick / dpad: commit a selection change. Scrolls back to the new selection
			// first, so the player always ends up looking at what they selected.
			float nav = TaloketoInputManager.GetVector2("NavigateLevel").x;
			if (Mathf.Abs(nav) < stepThreshold)
			{
				stepLatched = false;
				stepCooldown = 0f;
			}
			else
			{
				stepCooldown -= Time.deltaTime;
				if (!stepLatched || stepCooldown <= 0f)
				{
					stepCooldown = stepLatched ? stepRepeatRate : stepRepeatDelay;
					stepLatched = true;
					if (BeginStep(map, nav > 0f ? 1 : -1)) return;
				}
			}

			if (focused == null)
			{
				ShowCursor(false);
				return;
			}

			PositionCursorAtWorld(focused.transform.position);

			if (TaloketoInputManager.GetButtonDown("Throw")) focused.OnClick();
		}

		/// Puts the selection on the furthest unlocked level whenever the map appears - the one
		/// carrying the bobbing hand, i.e. the level you'd actually play next.
		///
		/// Map.Init centres the hole *before* the one it's given, so opening the map left the
		/// selection on the last completed level with the hand sitting one place further on.
		/// Centres immediately rather than animating, since this is the opening view.
		private void SelectUnbeatenLevel(Map map)
		{
			scrollingBack = false;

			int target = Game.GetUnbeatenLastHole();
			MapLevelUi node = map.FindLevelUi(target);

			// Paywalled or otherwise not yet reachable - fall back to whatever is selectable.
			if (node == null || !node.interactable)
			{
				node = map.GetFocusedUi() as MapLevelUi;
				if (node == null)
				{
					anchorHole = NoTarget;
					return;
				}

				target = node.holeNo;
			}

			anchorHole = target;

			Camera cam = Game.cam != null ? Game.cam : Camera.main;
			if (cam != null) map.ScrollBy(-(node.transform.position.x - cam.transform.position.x));
		}

		/// Starts a scroll back to the selection one step away. Targets a hole *number*, not a
		/// node: Refresh recycles the nine nodes and reassigns which hole each shows as the map
		/// scrolls, so a node reference would drift. Returns true if a scroll-back started.
		private bool BeginStep(Map map, int dir)
		{
			if (anchorHole == NoTarget) return false;

			int candidate = anchorHole + dir;
			MapLevelUi next = map.FindLevelUi(candidate);

			// Off the end of the unlocked run, or outside the recycled window. Fall back to
			// re-centring the current selection, so a press always brings the player back to
			// something rather than appearing to do nothing.
			int destination = (next != null && next.interactable) ? candidate : anchorHole;

			BeginScrollBack(map, destination);
			return true;
		}

		/// True once the first level has reached the left limit. Map's own clamp stops a full
		/// dot-spacing further left, which reads as mouse-drag rubber-banding but leaves empty
		/// scenery on screen with nothing selected. leftStopOffset shifts where that limit
		/// sits: 0 stops with the first level centred, negative stops earlier still.
		private bool IsAtLeftLimit(Map map)
		{
			MapLevelUi first = map.FindLevelUi(0);
			if (first == null) return false;

			Camera cam = Game.cam != null ? Game.cam : Camera.main;
			if (cam == null) return false;

			return first.transform.position.x - cam.transform.position.x >= leftStopOffset;
		}

		/// Scrolls until the targeted hole sits under the camera. Driving to a world position
		/// each frame rather than applying a fixed nudge means it self-corrects, and it needs
		/// no knowledge of MoveMap's xDelta bookkeeping or its colour-band edge cases (where
		/// one dot-spacing of drag doesn't advance at all).
		private void BeginScrollBack(Map map, int destinationHole)
		{
			scrollBackTo = destinationHole;
			scrollBackElapsed = 0f;
			scrollBackApplied = 0f;
			scrollBackTotal = ScrollDeltaTo(map, destinationHole);

			// A step to the neighbouring level travels about one spacing and should feel
			// immediate; a return from a long peek is a different move and gets the slower,
			// distance-independent beat. Charging every step the return's duration is what
			// made plain stepping feel sluggish.
			bool returningFromPeek = Mathf.Abs(scrollBackTotal) > Spline.distPerPoint * peekReturnSpacings;
			scrollBackSeconds = returningFromPeek ? scrollBackDuration : stepDuration;

			scrollingBack = true;
		}

		/// Signed ScrollBy amount that would centre a hole.
		///
		/// Uses the node's real position when it's inside the recycled window. When the player
		/// has peeked far enough that it isn't, falls back to counting hole spacings - adding
		/// one extra per colour-band edge crossed, because MoveMap needs a double spacing to
		/// step across those. The estimate only has to get close; UpdateScrollBack does an
		/// exact centring once the node is back in the window.
		private static float ScrollDeltaTo(Map map, int holeNumber)
		{
			Camera cam = Game.cam != null ? Game.cam : Camera.main;
			float camX = cam != null ? cam.transform.position.x : 0f;

			MapLevelUi node = map.FindLevelUi(holeNumber);
			if (node != null) return -(node.transform.position.x - camX);

			int from = map.HoleNo;
			int lo = Mathf.Min(from, holeNumber);
			int hi = Mathf.Max(from, holeNumber);
			int bandEdges = 0;
			for (int k = lo; k < hi; k++)
			{
				if (k % LevelGenerator.numLevelsPerColor == LevelGenerator.numLevelsPerColor - 1) bandEdges++;
			}

			float distance = (hi - lo + bandEdges) * Spline.distPerPoint;
			return holeNumber > from ? -distance : distance;
		}

		/// Drives the scroll-back on a fixed clock, so the trip always takes the same time and
		/// the speed instead varies with how far the player wandered. Returns true while it
		/// still owns the map.
		private bool UpdateScrollBack(Map map)
		{
			if (!scrollingBack) return false;

			scrollBackElapsed += Time.deltaTime;
			float t = scrollBackSeconds > 0f ? Mathf.Clamp01(scrollBackElapsed / scrollBackSeconds) : 1f;

			// Ease-out cubic against elapsed time rather than remaining distance, so the
			// duration stays fixed instead of drifting with the distance travelled.
			float eased = 1f - Mathf.Pow(1f - t, 3f);
			float desired = scrollBackTotal * eased;
			map.ScrollBy(desired - scrollBackApplied);
			scrollBackApplied = desired;

			MapLevelUi destination = map.FindLevelUi(scrollBackTo);
			if (destination != null) PositionCursorAtWorld(destination.transform.position);
			else ShowCursor(false);

			if (t < 1f) return true;

			// Land exactly, in case the delta was estimated from outside the window.
			if (destination != null)
			{
				Camera cam = Game.cam != null ? Game.cam : Camera.main;
				if (cam != null) map.ScrollBy(-(destination.transform.position.x - cam.transform.position.x));
			}

			anchorHole = scrollBackTo;
			scrollingBack = false;
			return false;
		}

		private void PositionCursorAt(GameObject target)
		{
			var rect = target != null ? target.transform as RectTransform : null;
			if (rect == null)
			{
				ShowCursor(false);
				return;
			}

			// Overlay canvas -> no camera in the conversion, for both of these.
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, rect.position);
			PositionCursorAtScreen(screenPoint);
		}

		private void PositionCursorAtWorld(Vector3 worldPosition)
		{
			Camera cam = Game.cam != null ? Game.cam : Camera.main;
			if (cam == null)
			{
				ShowCursor(false);
				return;
			}

			PositionCursorAtScreen(cam.WorldToScreenPoint(worldPosition));
		}

		private void PositionCursorAtScreen(Vector2 screenPoint)
		{
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out Vector2 local))
			{
				ShowCursor(false);
				return;
			}

			// Mirroring flips the offset too, so the cursor actually moves to the other side
			// rather than just facing the wrong way from the same spot.
			Vector2 offset = mirrorCursor ? new Vector2(-cursorOffset.x, cursorOffset.y) : cursorOffset;
			cursorRect.anchoredPosition = local + offset;
			ShowCursor(true);
		}

		private void ShowCursor(bool visible)
		{
			if (cursorImage == null) return;
			if (visible && cursorImage.sprite != cursorSprite)
			{
				cursorImage.sprite = cursorSprite;
				ApplyCursorSize();
			}
			if (cursorImage.enabled != visible) cursorImage.enabled = visible;
		}
	}
}
