using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Collection.Controls
{
	/// <summary>
	/// Button prompts for a game that draws itself into a small texture and stretches that
	/// to the window (the Flash, Flixel and Phaser ports). Drawn inside that texture, a
	/// glyph would be a handful of pixels; here the prompts are drawn over the stretched
	/// picture at the window's resolution, and placed in the game's own pixel coordinates.
	///
	/// The game's loop owns one of these and drives it the way such engines draw: every
	/// frame it calls BeginFrame, whatever wants a prompt on screen calls Show on its label,
	/// and EndFrame hides the labels nobody asked for.
	/// </summary>
	public class InputPromptOverlay
	{
		public class Label
		{
			internal RectTransform rect;
			internal TextMeshProUGUI text;
			internal InputPromptText prompt;
			internal int shownFrame = -1;
			internal InputPromptOverlay owner;

			public bool Alive => rect != null;

			/// <summary>
			/// Puts the prompt on screen this frame, centred on (x, y) in game pixels from
			/// the picture's top-left corner, Y down. keyHeight is how tall a key cap comes
			/// out, in game pixels.
			/// </summary>
			public void Show(string template, double x, double y, double keyHeight, Color color, bool shadowed)
			{
				// A key cap is 74 of the sheet's pixels, drawn at font size / 68 per pixel.
				float fontSize = (float)keyHeight * 68f / 74f;
				if (text.fontSize != fontSize) text.fontSize = fontSize;
				if (text.color != color) text.color = color;

				if (prompt.template != template || prompt.shadowed != shadowed)
				{
					prompt.shadowed = shadowed;
					prompt.SetTemplate(template);
				}

				rect.anchoredPosition = new Vector2((float)x, -(float)y);
				if (!text.enabled) text.enabled = true;
				shownFrame = owner.frame;
			}

			public void Destroy()
			{
				if (rect != null)
				{
					Object.Destroy(rect.gameObject);
				}

				rect = null;
				text = null;
				prompt = null;
			}
		}

		private readonly RectTransform root;
		private readonly RectTransform picture;
		private readonly float gameWidth;
		private readonly List<Label> labels = new List<Label>();
		private int frame;

		/// picture is the UI element showing the game's texture; the prompts become its
		/// children, so they stay over it wherever it is fitted in the window.
		public InputPromptOverlay(RectTransform picture, int gameWidth, int gameHeight)
		{
			this.picture = picture;
			this.gameWidth = gameWidth;

			var go = new GameObject("Prompts", typeof(RectTransform));
			root = (RectTransform)go.transform;
			root.SetParent(picture, false);

			// Top-left corner of the picture, one unit a game pixel.
			root.anchorMin = root.anchorMax = root.pivot = new Vector2(0f, 1f);
			root.anchoredPosition = Vector2.zero;
			root.sizeDelta = new Vector2(gameWidth, gameHeight);
		}

		public Label NewLabel()
		{
			var go = new GameObject("Prompt", typeof(RectTransform));
			var label = new Label { owner = this, rect = (RectTransform)go.transform };
			label.rect.SetParent(root, false);
			label.rect.anchorMin = label.rect.anchorMax = new Vector2(0f, 1f);
			label.rect.pivot = new Vector2(0.5f, 0.5f);
			label.rect.sizeDelta = Vector2.zero;

			label.text = go.AddComponent<TextMeshProUGUI>();
			label.text.alignment = TextAlignmentOptions.Center;
			label.text.textWrappingMode = TextWrappingModes.NoWrap;
			label.text.overflowMode = TextOverflowModes.Overflow;
			label.text.raycastTarget = false;
			label.text.enabled = false;
			label.text.text = "";

			label.prompt = go.AddComponent<InputPromptText>();
			labels.Add(label);
			return label;
		}

		public void BeginFrame()
		{
			frame++;

			float scale = picture.rect.width / gameWidth;
			if (root.localScale.x != scale)
			{
				root.localScale = new Vector3(scale, scale, 1f);
			}
		}

		public void EndFrame()
		{
			for (int i = labels.Count - 1; i >= 0; i--)
			{
				Label label = labels[i];
				if (!label.Alive)
				{
					labels.RemoveAt(i);
				}
				else if (label.shownFrame != frame && label.text.enabled)
				{
					label.text.enabled = false;
				}
			}
		}

		/// Destroys every label, for when the game throws away what it was drawing.
		public void Clear()
		{
			for (int i = 0; i < labels.Count; i++)
			{
				labels[i].Destroy();
			}

			labels.Clear();
		}
	}
}
