using TMPro;
using UnityEngine;

namespace Collection.Controls
{
	/// <summary>
	/// Put on a TextMesh Pro text whose words include button prompts. The {tokens} in the
	/// template (see InputPrompts) are drawn as glyphs for whatever device the player is
	/// using, and change when the device does.
	///
	/// The glyphs take the text's colour, so the same prompt works in white and in black.
	/// </summary>
	[RequireComponent(typeof(TMP_Text))]
	public class InputPromptText : MonoBehaviour
	{
		[TextArea]
		[Tooltip("The text, with prompts as {tokens}: {Jump} for an action of this game, or {<Keyboard>/space|<Gamepad>/buttonSouth} for controls. Left empty, the text component's own text is taken as the template.")]
		public string template;

		[Tooltip("Use the glyphs with a black drop shadow, for text over a busy picture.")]
		public bool shadowed;

		[Tooltip("Optional. The text takes this renderer's colour every frame - for a prompt that replaces part of a sprite which the game's own scripts tint or fade.")]
		public SpriteRenderer colourFrom;

		private TMP_Text text;
		private bool animated;
		private bool lastBlink;

		private void Awake()
		{
			text = GetComponent<TMP_Text>();
			if (string.IsNullOrEmpty(template))
			{
				template = text.text;
			}
		}

		private void OnEnable()
		{
			InputPrompts.Changed += Refresh;
			Refresh();
		}

		// Again in Start: an object that is part of a game's first scene is enabled before the
		// game's action map is switched in, so its actions could not be found in OnEnable.
		private void Start()
		{
			Refresh();
		}

		private void OnDisable()
		{
			InputPrompts.Changed -= Refresh;
		}

		private void Update()
		{
			if (colourFrom != null && text.color != colourFrom.color)
			{
				text.color = colourFrom.color;
			}

			if (animated && InputPrompts.Blink != lastBlink)
			{
				Refresh();
			}
		}

		/// For text that changes while the game runs.
		public void SetTemplate(string newTemplate)
		{
			template = newTemplate;
			Refresh();
		}

		public void Refresh()
		{
			if (text == null)
			{
				return;
			}

			text.spriteAsset = InputPrompts.SpriteAsset(shadowed);
			lastBlink = InputPrompts.Blink;
			text.text = InputPrompts.Format(template, out animated);
		}
	}
}
