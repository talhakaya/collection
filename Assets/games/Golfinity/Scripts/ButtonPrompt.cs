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

		[Tooltip("Gap between the bottom of the button's artwork and the middle of the prompt, in canvas units. Measured from the rendered corners, so buttons of different sizes all get the same gap.")]
		public float gap = 6.5f;

		private TextMeshProUGUI text;
		private RectTransform self;
		private RectTransform owner;
		private string lastGlyph;
		private int lastPadId = -1;
		private readonly Vector3[] corners = new Vector3[4];

		private void Awake()
		{
			text = GetComponent<TextMeshProUGUI>();
			self = (RectTransform)transform;

			ButtonExtended button = GetComponentInParent<ButtonExtended>(true);
			if (button != null)
			{
				// The node that actually carries the artwork, and the one the button's
				// animation moves - following it is what makes the prompt bob with the button.
				owner = button.transform.Find("button") as RectTransform;
			}

			NormaliseScale();
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
		/// four is the tip, which is what the eye reads as the bottom.
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
			self.position = new Vector3(centreX, lowest - gap * scale, self.position.z);
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
			}

			if (!visible)
			{
				return;
			}

			// Resolving a binding to its display string allocates, so it is done only when the
			// pad actually changes rather than every frame for every prompt.
			Gamepad pad = Gamepad.current;
			int padId = pad != null ? pad.deviceId : 0;
			if (padId == lastPadId && lastGlyph != null)
			{
				return;
			}

			lastPadId = padId;
			string glyph = GolfinityGamepad.GamepadGlyph(actionName);
			if (glyph != lastGlyph)
			{
				lastGlyph = glyph;
				text.text = glyph;
			}
		}
	}
}
