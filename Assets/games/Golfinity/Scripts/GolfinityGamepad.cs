using Collection.Controls;
using UnityEngine;
using UnityEngine.EventSystems;
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

		[Header("Map navigation")]
		[Tooltip("World units/sec the right stick free-scrolls the map. Fixed speed, no inertia.")]
		public float scrollSpeed = 40f;
		[Tooltip("Seconds the scroll back to the selection takes. Fixed, so the speed varies with how far the player wandered.")]
		public float scrollBackDuration = 0.35f;
		[Tooltip("Limit how far back-scrolling can go past the first level.")]
		public bool clampToFirstLevel = true;
		[Tooltip("Where back-scrolling stops, as the first level's offset from centre in world units. 0 stops with it centred; negative stops earlier, showing less empty scenery to its left.")]
		public float leftStopOffset = 0f;
		[Tooltip("Stick deflection needed to register a level step.")]
		public float stepThreshold = 0.5f;
		public float stepRepeatDelay = 0.4f;
		public float stepRepeatRate = 0.18f;

		private const int NoTarget = -1;
		private int anchorHole = NoTarget;
		private bool scrollingBack;
		private int scrollBackTo;
		private float scrollBackElapsed;
		private float scrollBackTotal;
		private float scrollBackApplied;

		private RectTransform canvasRect;
		private RectTransform cursorRect;
		private Image cursorImage;
		private GameObject activePopup;
		private float stepCooldown;
		private bool stepLatched;

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
		}

		private void Update()
		{
			if (cursorRect == null) return;

			// Keep the cursor on top - popups shown later would otherwise draw over it.
			cursorRect.SetAsLastSibling();

			GameObject popup = GetActivePopup();
			if (popup != activePopup)
			{
				activePopup = popup;
				if (popup != null) SelectFirstIn(popup);
			}

			if (popup != null)
			{
				PositionCursorAt(EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null);
				return;
			}

			HandleTopButtonShortcuts();

			if (Game.state == GameState.Map) UpdateMap();
			else ShowCursor(false);
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
			else if (TaloketoInputManager.GetButtonDown("Upgrade")) Game.instance.OnClickUpgrade();
		}

		private void UpdateMap()
		{
			Map map = Game.instance != null ? Game.instance.map : null;
			if (map == null || !map.gameObject.activeInHierarchy)
			{
				ShowCursor(false);
				return;
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
			float t = scrollBackDuration > 0f ? Mathf.Clamp01(scrollBackElapsed / scrollBackDuration) : 1f;

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

			cursorRect.anchoredPosition = local + cursorOffset;
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
