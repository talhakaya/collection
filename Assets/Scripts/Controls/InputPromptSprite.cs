using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Collection.Controls
{
	/// <summary>
	/// A single button prompt as a sprite, on a SpriteRenderer or a UI Image - for prompts
	/// that sit in a picture rather than in a line of text. Shows the glyph for whatever
	/// device the player is using, and changes when the device does.
	///
	/// One glyph only: a token that comes to several (two keys, say) shows the first.
	/// </summary>
	public class InputPromptSprite : MonoBehaviour
	{
		[Tooltip("What the prompt is for, without braces: Jump for an action of this game, or <Keyboard>/space|<Gamepad>/buttonSouth for controls.")]
		public string token;

		[Tooltip("Use the glyph with a black drop shadow, for a prompt over a busy picture.")]
		public bool shadowed;

		private readonly List<InputGlyph> glyphs = new List<InputGlyph>();
		private SpriteRenderer spriteRenderer;
		private Image image;
		private bool animated;
		private bool lastBlink;

		private void Awake()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			image = GetComponent<Image>();
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
			if (animated && InputPrompts.Blink != lastBlink)
			{
				Refresh();
			}
		}

		public void Refresh()
		{
			glyphs.Clear();
			lastBlink = InputPrompts.Blink;
			animated = false;
			if (!InputPrompts.Resolve(token, glyphs))
			{
				return;
			}

			animated = glyphs[0].Animated;
			Sprite sprite = InputPrompts.GetSprite(glyphs[0].Current, shadowed);
			if (spriteRenderer != null) spriteRenderer.sprite = sprite;
			if (image != null) image.sprite = sprite;
		}
	}
}
