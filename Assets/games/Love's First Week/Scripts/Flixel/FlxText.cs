using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Games.LovesFirstWeek
{
	/// <summary>
	/// org.flixel.FlxText: a sprite whose picture is a block of word-wrapped text.
	///
	/// Flixel rendered a Flash TextField into the sprite's bitmap. Here the text is 3D
	/// TextMeshPro placed where that bitmap would be - the same approach Sleepy Time's
	/// FlashText takes.
	///
	/// Two fonts, as in the game: Flixel's own embedded "system" pixel font for buttons
	/// and small print, and the game's "NES" font for dialogue and titles. The system font
	/// is a raster TMP asset sampled at its native 8 pixels, so one texel is one game
	/// pixel. NES is used at sizes that are not multiples of its grid (12, 20, 60), where
	/// Flash drew its outlines smoothed; it is a distance-field asset, which does the same.
	///
	/// A Flash TextField keeps a 2-pixel gutter inside its box on every side, so the text
	/// starts 2 pixels in and wraps 4 pixels short of the width.
	///
	/// A shadow is the same text drawn first, one pixel down and to the right.
	/// </summary>
	public class FlxText : FlxSprite
	{
		public const string FontResourceRoot = "LovesFirstWeek/fonts/";

		private const float TextScale = 0.1f;
		private const float UnitsPerPixel = 1f / (TextScale * FlxGame.PixelsPerUnit);
		private const float Gutter = 2f;

		private static readonly Dictionary<string, TMP_FontAsset> fonts = new Dictionary<string, TMP_FontAsset>();

		protected string _text;
		protected string _font;
		protected double _size;
		protected string _alignment;
		protected uint _shadow;

		private class Field
		{
			public FlxRenderer renderer;
			public TextMeshPro tmp;
			public RectTransform rect;
		}

		private Field _field;
		private Field _shadowField;

		public FlxText(double X, double Y, int Width, string Text = null, bool EmbeddedFont = true) : base(X, Y)
		{
			makeGraphic(Width, 1, 0);
			if (Text == null)
			{
				Text = "";
			}

			_text = Text;
			_font = "system";
			_size = 8;
			_alignment = "left";
			_shadow = 0;
			_color = 0xffffff;
			allowCollisions = NONE;
		}

		public override void destroy()
		{
			destroyField(ref _field);
			destroyField(ref _shadowField);
			base.destroy();
		}

		private static void destroyField(ref Field field)
		{
			if (field != null && field.renderer != null)
			{
				field.renderer.Destroy();
			}

			field = null;
		}

		public FlxText setFormat(string Font = null, double Size = 8, uint Color = 0xffffff, string Alignment = null, uint ShadowColor = 0)
		{
			_font = Font ?? "system";
			_size = Size;
			_color = Color & 0x00ffffff;
			_alignment = Alignment ?? "left";
			_shadow = ShadowColor;
			return this;
		}

		public string text
		{
			get { return _text; }
			set { _text = value ?? ""; }
		}

		public double size
		{
			get { return _size; }
			set { _size = value; }
		}

		public string alignment
		{
			get { return _alignment; }
			set { _alignment = value; }
		}

		public uint shadow
		{
			get { return _shadow; }
			set { _shadow = value; }
		}

		protected override void present(double pointX, double pointY)
		{
			if (_shadow > 0)
			{
				var shadowColor = new Color(
					((_shadow >> 16) & 0xff) / 255f,
					((_shadow >> 8) & 0xff) / 255f,
					(_shadow & 0xff) / 255f,
					(float)_alpha);
				presentField(ref _shadowField, pointX + 1, pointY + 1, shadowColor);
			}

			presentField(ref _field, pointX, pointY, renderColor());
		}

		private void presentField(ref Field field, double pointX, double pointY, Color color)
		{
			if (field == null || !field.renderer.Alive)
			{
				field = createField();
			}

			TextMeshPro tmp = field.tmp;
			TMP_FontAsset fontAsset = ResolveFont(_font);
			if (fontAsset != null && tmp.font != fontAsset)
			{
				tmp.font = fontAsset;
			}

			if (tmp.text != _text)
			{
				tmp.text = _text;
			}

			float fontSize = (float)_size;
			if (tmp.fontSize != fontSize)
			{
				tmp.fontSize = fontSize;
			}

			TextAlignmentOptions align = _alignment == "center" ? TextAlignmentOptions.Top
				: _alignment == "right" ? TextAlignmentOptions.TopRight
				: TextAlignmentOptions.TopLeft;
			if (tmp.alignment != align)
			{
				tmp.alignment = align;
			}

			float boxWidth = Mathf.Max(1f, (float)width - Gutter * 2f) * UnitsPerPixel;
			if (field.rect.sizeDelta.x != boxWidth)
			{
				field.rect.sizeDelta = new Vector2(boxWidth, field.rect.sizeDelta.y);
			}

			if (tmp.color != color)
			{
				tmp.color = color;
			}

			field.renderer.Present((int)pointX + Gutter, (int)pointY + Gutter, TextScale, TextScale, false, color);
		}

		private Field createField()
		{
			var field = new Field();
			field.renderer = FlxGame.NewRenderer("FlxText", typeof(RectTransform));
			field.rect = (RectTransform)field.renderer.gameObject.transform;
			field.rect.pivot = new Vector2(0f, 1f);
			field.rect.sizeDelta = new Vector2(100f, 10f);

			field.tmp = field.renderer.gameObject.AddComponent<TextMeshPro>();
			field.tmp.alignment = TextAlignmentOptions.TopLeft;
			field.tmp.textWrappingMode = TextWrappingModes.Normal;
			field.tmp.overflowMode = TextOverflowModes.Overflow;
			field.tmp.richText = false;
			field.tmp.fontSize = (float)_size;

			TMP_FontAsset fontAsset = ResolveFont(_font);
			if (fontAsset != null)
			{
				field.tmp.font = fontAsset;
			}

			field.renderer.renderer = field.tmp.renderer;
			return field;
		}

		/// "NES" is the game's embedded font; anything else is Flixel's "system".
		private static TMP_FontAsset ResolveFont(string name)
		{
			string asset = name == "NES" ? "NES" : "system";
			TMP_FontAsset font;
			if (fonts.TryGetValue(asset, out font) && font != null)
			{
				return font;
			}

			font = Resources.Load<TMP_FontAsset>(FontResourceRoot + asset);
			if (font == null)
			{
				Debug.LogWarning("LovesFirstWeek: no TMP font asset at Resources/" + FontResourceRoot + asset + " - falling back to the default font.");
			}

			fonts[asset] = font;
			return font;
		}
	}
}
