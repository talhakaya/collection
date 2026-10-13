using Collection.Controls;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Games.Golfinity
{
	/// <summary>
	/// The gamepad button prompt under a button.
	///
	/// It lives on a child of button.prefab's animated node, which buys two things the
	/// previous canvas-parented labels had to work for: it follows the button's bob without
	/// anything tracking its position, and it inherits the button's own show/hide, so a
	/// prompt can never outlive or out-live-hide the button it labels.
	///
	/// Every prompt in the game is the same prefab child, so position, font size and outline
	/// match by construction rather than by tuning each one. Adding a prompt to a new button
	/// is filling in actionName.
	/// </summary>
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class ButtonPrompt : MonoBehaviour
	{
		[Tooltip("Action whose gamepad binding is drawn here. The glyph is read from the binding itself, so it stays honest if the action is rebound. Leave empty to show nothing.")]
		public string actionName;

		[Tooltip("For the top bar, which stays on screen behind an open popup but stops responding to its shortcuts - advertising them then would be a lie. Popup buttons don't need it: they only exist while their popup is up.")]
		public bool hideWhenPopupOpen;

		[Tooltip("Gap between the bottom of the button's shadow and the middle of the prompt, in canvas units. Measured from the rendered corners, so buttons of different sizes all get the same gap.")]
		public float gap = 5f;

		[Tooltip("How far below the glyph its drop shadow is drawn, in canvas units. Zero for no shadow.")]
		public float shadowOffset = 0.7f;

		// What the glyph's white is swapped for in the shadow's copy of the text.
		private const string GlyphColour = "color=#FFFFFF";
		private const string ShadowColour = "color=#000000C0";

		// The shadow is this object's own text, and the glyph is a child drawn over it: a child
		// is what the canvas draws after, and so on top of, its parent.
		private TextMeshProUGUI text;
		private TextMeshProUGUI front;
		private RectTransform self;
		private RectTransform owner;
		private float shadowDrop;
		private string lastGlyph;
		private int lastPadId = -1;
		private GamepadKind lastPadKind;
		private bool animated;
		private bool lastBlink;
		private readonly Vector3[] corners = new Vector3[4];

		private void Awake()
		{
			text = GetComponent<TextMeshProUGUI>();
			self = (RectTransform)transform;

			// In the collection: the prompt is the collection's glyph for the button rather
			// than its letter in the game's font, in white.
			text.spriteAsset = InputPrompts.SpriteAsset();

			ButtonExtended button = GetComponentInParent<ButtonExtended>(true);
			if (button != null)
			{
				// The node that actually carries the artwork, and the one the button's
				// animation moves - following it is what makes the prompt bob with the button.
				owner = button.transform.Find("button") as RectTransform;

				// How far the shadow hangs below the artwork at rest, in the button's own
				// units. The shadow is the same diamond, so this is also how far its tip is
				// below the artwork's - and the prompt goes below that, clear of the shadow.
				RectTransform shadow = button.transform.Find("shadow") as RectTransform;
				if (owner != null && shadow != null)
				{
					shadowDrop = Mathf.Max(0f, owner.anchoredPosition.y - shadow.anchoredPosition.y);
				}
			}

			NormaliseScale();
			CreateFront();
		}

		/// <summary>
		/// The glyph proper: a copy of this text as a child, shadowOffset above it. This
		/// object's own text is left to be the shadow underneath, so it goes dark here.
		///
		/// Drawn twice because the glyphs are sprites, which TextMesh Pro's own outline and
		/// underlay do nothing for - those belong to the font's shader.
		/// </summary>
		private void CreateFront()
		{
			var go = new GameObject("glyph", typeof(RectTransform));
			go.layer = gameObject.layer;
			var rect = (RectTransform)go.transform;
			rect.SetParent(self, false);
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.sizeDelta = Vector2.zero;
			rect.anchoredPosition = new Vector2(0f, shadowOffset);

			front = go.AddComponent<TextMeshProUGUI>();
			front.font = text.font;
			front.fontSharedMaterial = text.fontSharedMaterial;
			front.spriteAsset = text.spriteAsset;
			front.fontSize = text.fontSize;
			front.alignment = text.alignment;
			front.textWrappingMode = text.textWrappingMode;
			front.overflowMode = text.overflowMode;
			front.color = text.color;
			front.raycastTarget = false;
			front.enabled = text.enabled;

			// For a pad the sheet has no glyph for, where the prompt is a letter in the font.
			text.color = new Color(0f, 0f, 0f, 0.75f);
		}

		/// <summary>
		/// Buttons are placed at 0.15 (top bar), 0.25 (popup) and 0.3 (upgrade), and a child
		/// inherits that - so one font size would render three different sizes on screen.
		/// Cancelling it is what lets a single prefab-level style hold for all of them, and it
		/// is the reason the labels this replaces were parented to the canvas rather than to
		/// their buttons.
		///
		/// Normalised against the canvas rather than against any particular parent's scale, so
		/// it does not care how the button got its size. One canvas unit of font size is then
		/// one canvas unit on screen, whatever it hangs under.
		/// </summary>
		private void NormaliseScale()
		{
			Canvas canvas = GetComponentInParent<Canvas>(true);
			if (canvas == null || transform.parent == null) return;

			float inherited = transform.parent.lossyScale.x / canvas.transform.lossyScale.x;
			if (Mathf.Approximately(inherited, 0f)) return;

			transform.localScale = new Vector3(1f / inherited, 1f / inherited, 1f);
		}

		/// <summary>
		/// Placed from the button's rendered corners rather than a fixed offset, and in
		/// LateUpdate so the button's animation has already moved for this frame.
		///
		/// The offset cannot simply be baked into the prefab: the prompt's scale is normalised
		/// so the glyph is the same size on every button, which means its own rect no longer
		/// scales with the button while an offset expressed in the button's units would. The
		/// two only agree at one button size. Measuring the artwork each frame keeps the gap
		/// identical whatever the button's scale, and costs four corner reads.
		///
		/// The corners are the rotated ones - these buttons are diamonds - so the lowest of the
		/// four is the tip, which is what the eye reads as the bottom. The shadow's tip is
		/// shadowDrop below that, and the gap is counted from there.
		/// </summary>
		private void PlaceBelowButton()
		{
			if (owner == null) return;

			owner.GetWorldCorners(corners);
			float lowest = corners[0].y;
			float centreX = 0f;
			for (int i = 0; i < 4; i++)
			{
				if (corners[i].y < lowest) lowest = corners[i].y;
				centreX += corners[i].x;
			}

			centreX *= 0.25f;
			float scale = self.lossyScale.y;
			float drop = owner.parent != null ? shadowDrop * owner.parent.lossyScale.y : 0f;
			self.position = new Vector3(centreX, lowest - drop - (gap + shadowOffset) * scale, self.position.z);
		}

		private void LateUpdate()
		{
			PlaceBelowButton();

			bool visible = !string.IsNullOrEmpty(actionName)
				&& GolfinityGamepad.UsingGamepad
				&& !(hideWhenPopupOpen && GolfinityGamepad.PopupOpen);

			if (text.enabled != visible)
			{
				text.enabled = visible;
				front.enabled = visible;
			}

			if (!visible)
			{
				return;
			}

			// Resolving a binding to its glyph allocates, so it is done only when the pad
			// actually changes (or an animated glyph is due to swap) rather than every frame
			// for every prompt.
			Gamepad pad = Gamepad.current;
			int padId = pad != null ? pad.deviceId : 0;
			bool blink = InputPrompts.Blink;
			if (padId == lastPadId && InputPrompts.PadKind == lastPadKind && lastGlyph != null && !(animated && blink != lastBlink))
			{
				return;
			}

			lastPadId = padId;
			lastPadKind = InputPrompts.PadKind;
			lastBlink = blink;
			string glyph = GolfinityGamepad.GamepadGlyph(actionName, out animated);
			if (glyph != lastGlyph)
			{
				lastGlyph = glyph;
				front.text = glyph;
				text.text = glyph.Replace(GlyphColour, ShadowColour);
			}
		}
	}
}
