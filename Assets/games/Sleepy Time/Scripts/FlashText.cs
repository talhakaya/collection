using TMPro;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// A Flash TextField as a display-list node. Text in Sleepy Time is not UI - TextTalha
	/// scales, sways and blinks exactly like every other Citmap - so this is 3D TextMeshPro
	/// under a FlashObject, not a canvas.
	///
	/// The TMP component lives on a child scaled by TextScale, because the node's own
	/// transform belongs to FlashObject (scaleX/scaleY are gameplay state). That scale is
	/// picked so the two units line up exactly: with 100 pixels to the world unit, a TMP
	/// font size of N points renders N stage pixels tall, and widths convert by
	/// UnitsPerPixel. So every size in the source can be transcribed as the number it is.
	/// </summary>
	public class FlashText : FlashObject
	{
		/// <summary>
		/// The font TextTalha's TextFormat asks for, converted from fonts/. Raster rather
		/// than SDF - it is a pixel font drawn at 30 px on an 800x450 stage, and an SDF atlas
		/// would round its corners off. Sampled at 30 so one texel is one stage pixel.
		/// </summary>
		public const string FontResourcePath = "SleepyTime/Fonts/Victor's Pixel Font";

		private const float TextScale = 0.1f;
		private const float UnitsPerPixel = 1f / (TextScale * PixelsPerUnit);

		private static TMP_FontAsset font;
		private static bool fontResolved;

		private TextMeshPro tmp;
		private RectTransform rect;

		public static FlashText New(string name = "TextField")
		{
			GameObject node = NewNode(name);
			FlashText text = node.AddComponent<FlashText>();

			GameObject field = new GameObject("TMP", typeof(RectTransform));
			field.transform.SetParent(node.transform, false);
			field.transform.localScale = new Vector3(TextScale, TextScale, TextScale);

			text.rect = (RectTransform)field.transform;
			text.rect.pivot = new Vector2(0f, 1f);
			text.rect.anchorMin = text.rect.anchorMax = new Vector2(0f, 1f);
			text.rect.anchoredPosition = Vector2.zero;

			text.tmp = field.AddComponent<TextMeshPro>();
			text.tmp.alignment = TextAlignmentOptions.TopLeft;
			text.tmp.fontSize = 16f;
			// Flash TextFields default to multiline false / wordWrap false, and every one in
			// this game is left at that: a wide box holding a single centred line.
			text.tmp.textWrappingMode = TextWrappingModes.NoWrap;
			text.tmp.overflowMode = TextOverflowModes.Overflow;

			TMP_FontAsset fontAsset = ResolveFont();
			if (fontAsset != null)
			{
				text.tmp.font = fontAsset;
			}

			return text;
		}

		public TextMeshPro TMP
		{
			get { return tmp; }
		}

		public string text
		{
			get { return tmp.text; }
			set { tmp.text = value; }
		}

		/// Point size in stage pixels, as the source's TextFormat wrote it.
		public float size
		{
			get { return tmp.fontSize; }
			set { tmp.fontSize = value; }
		}

		public Color color
		{
			get { return tmp.color; }
			set { tmp.color = value; }
		}

		public TextAlignmentOptions align
		{
			get { return tmp.alignment; }
			set { tmp.alignment = value; }
		}

		/// <summary>
		/// Flash's TextField.width, in stage pixels. TextTalha sets it to the full stage
		/// width and shifts the field left by half of it so centred text is centred on the
		/// object's own origin - which only works if the box really is stage-width.
		/// </summary>
		public float width
		{
			get { return rect.sizeDelta.x / UnitsPerPixel; }
			set { rect.sizeDelta = new Vector2(value * UnitsPerPixel, rect.sizeDelta.y); }
		}

		public float height
		{
			get { return rect.sizeDelta.y / UnitsPerPixel; }
			set { rect.sizeDelta = new Vector2(rect.sizeDelta.x, value * UnitsPerPixel); }
		}

		public override void ApplyRender(float worldAlpha, int sortingOrder)
		{
			tmp.alpha = worldAlpha;
			tmp.sortingOrder = sortingOrder;
		}

		private static TMP_FontAsset ResolveFont()
		{
			if (!fontResolved)
			{
				fontResolved = true;
				font = Resources.Load<TMP_FontAsset>(FontResourcePath);
				if (font == null)
				{
					Debug.LogWarning(
						"SleepyTime: no TMP font asset at Resources/" + FontResourcePath +
						" - falling back to the default font. Generate one from " +
						"Assets/games/Sleepy Time/Fonts/Victor's Pixel Font.ttf.");
				}
			}

			return font;
		}
	}
}
