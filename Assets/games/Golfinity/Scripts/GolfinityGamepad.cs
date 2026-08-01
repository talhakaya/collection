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
		public Vector2 cursorSize = new Vector2(16f, 16f);
		[Tooltip("Offset from the selected item's centre, in canvas units.")]
		public Vector2 cursorOffset = new Vector2(6f, -6f);

		[Header("Map navigation")]
		[Tooltip("World units/sec the right stick free-scrolls the map. Fixed speed, no inertia.")]
		public float scrollSpeed = 40f;
		[Tooltip("World units/sec the map travels when stepping between levels.")]
		public float stepSpeed = 60f;
		[Tooltip("Stick deflection needed to register a level step.")]
		public float stepThreshold = 0.5f;
		public float stepRepeatDelay = 0.4f;
		public float stepRepeatRate = 0.18f;

		private const int NoTarget = -1;
		private int targetHole = NoTarget;

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
			cursorRect.sizeDelta = cursorSize;

			cursorImage = go.GetComponent<Image>();
			cursorImage.sprite = cursorSprite;
			cursorImage.raycastTarget = false;
			cursorImage.enabled = false;
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

			// Right stick: free scroll at a fixed speed, no inertia.
			float scroll = TaloketoInputManager.GetVector2("ScrollMap").x;
			if (!Mathf.Approximately(scroll, 0f))
			{
				map.ScrollBy(-scroll * scrollSpeed * Time.deltaTime);
				targetHole = NoTarget; // free scroll overrides an in-flight step
			}

			MapUi focused = map.GetFocusedUi();

			// Left stick / dpad: discrete level-to-level steps, with menu-style key repeat so
			// holding the stick walks the list instead of firing once or racing through it.
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
					RequestStep(map, focused, nav > 0f ? 1 : -1);
				}
			}

			bool stepping = AdvanceTowardTarget(map);

			if (focused == null)
			{
				ShowCursor(false);
				return;
			}

			PositionCursorAtWorld(focused.transform.position);

			// Don't let a press land mid-step, or it fires on whichever level happens to be
			// passing under the centre of the screen at that instant.
			if (!stepping && TaloketoInputManager.GetButtonDown("Throw")) focused.OnClick();
		}

		/// Targets a hole *number*, not a node: Refresh recycles the nine nodes and reassigns
		/// which hole each one shows as the map scrolls, so a node reference would drift.
		/// Refuses to target a level that isn't interactable, which is what keeps navigation
		/// from walking off the end into locked holes.
		private void RequestStep(Map map, MapUi focused, int dir)
		{
			int from = targetHole != NoTarget
				? targetHole
				: (focused is MapLevelUi level ? level.holeNo : map.HoleNo);

			MapLevelUi candidate = map.FindLevelUi(from + dir);
			if (candidate == null || !candidate.interactable) return;

			targetHole = from + dir;
		}

		/// Scrolls until the targeted hole sits under the camera. Driving to a world position
		/// each frame rather than applying a fixed nudge means it self-corrects, and it needs
		/// no knowledge of MoveMap's xDelta bookkeeping or its colour-band edge cases (where
		/// one dot-spacing of drag doesn't advance at all).
		private bool AdvanceTowardTarget(Map map)
		{
			if (targetHole == NoTarget) return false;

			MapLevelUi target = map.FindLevelUi(targetHole);
			if (target == null)
			{
				targetHole = NoTarget;
				return false;
			}

			Camera cam = Game.cam != null ? Game.cam : Camera.main;
			float offset = target.transform.position.x - (cam != null ? cam.transform.position.x : 0f);
			if (Mathf.Abs(offset) < 0.05f)
			{
				targetHole = NoTarget;
				return false;
			}

			float move = Mathf.Sign(offset) * Mathf.Min(Mathf.Abs(offset), stepSpeed * Time.deltaTime);
			map.ScrollBy(-move);
			return true;
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
			if (visible && cursorImage.sprite != cursorSprite) cursorImage.sprite = cursorSprite;
			if (cursorImage.enabled != visible) cursorImage.enabled = visible;
		}
	}
}
