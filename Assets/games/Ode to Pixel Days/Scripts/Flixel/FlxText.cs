using TMPro;
using UnityEngine;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// org.flixel.FlxText: a sprite whose picture is a block of word-wrapped text.
	///
	/// Flixel rendered a Flash TextField into the sprite's bitmap. Here the text is 3D
	/// TextMeshPro placed where that bitmap would be - the same approach Sleepy Time's
	/// FlashText takes. The font is Flixel's own embedded "system" pixel font, converted to
	/// a raster TMP font asset sampled at its native 8 pixels so one texel is one game
	/// pixel.
	///
	/// A Flash TextField keeps a 2-pixel gutter inside its box on every side, so the text
	/// starts 2 pixels in and wraps 4 pixels short of the width.
	/// </summary>
	public class FlxText : FlxSprite
	{
		public const string FontResourcePath = "OdeToPixelDays/fonts/system";

		private const float TextScale = 0.1f;
		private const float UnitsPerPixel = 1f / (TextScale * FlxGame.PixelsPerUnit);
		private const float Gutter = 2f;

		private static TMP_FontAsset font;
		private static bool fontResolved;

		protected string _text;
		protected double _size;
		protected string _alignment;
		protected uint _shadow;

		private FlxRenderer _textRenderer;
		private TextMeshPro _tmp;
		private RectTransform _rect;

		public FlxText(double X, double Y, int Width, string Text = null, bool EmbeddedFont = true) : base(X, Y)
		{
			makeGraphic(Width, 1, 0);
			if (Text == null)
			{
				Text = "";
			}

			_text = Text;
			_size = 8;
			_alignment = "left";
			_shadow = 0;
			_color = 0xffffff;
			allowCollisions = NONE;
		}

		public override void destroy()
		{
			if (_textRenderer != null)
			{
				_textRenderer.Destroy();
				_textRenderer = null;
			}

			_tmp = null;
			_rect = null;
			base.destroy();
		}

		public FlxText setFormat(string Font = null, double Size = 8, uint Color = 0xffffff, string Alignment = null, uint ShadowColor = 0)
		{
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
			if (_textRenderer == null || !_textRenderer.Alive)
			{
				createField();
			}

			if (_tmp.text != _text)
			{
				_tmp.text = _text;
			}

			float fontSize = (float)_size;
			if (_tmp.fontSize != fontSize)
			{
				_tmp.fontSize = fontSize;
			}

			TextAlignmentOptions align = _alignment == "center" ? TextAlignmentOptions.Top
				: _alignment == "right" ? TextAlignmentOptions.TopRight
				: TextAlignmentOptions.TopLeft;
			if (_tmp.alignment != align)
			{
				_tmp.alignment = align;
			}

			float boxWidth = Mathf.Max(1f, (float)width - Gutter * 2f) * UnitsPerPixel;
			if (_rect.sizeDelta.x != boxWidth)
			{
				_rect.sizeDelta = new Vector2(boxWidth, _rect.sizeDelta.y);
			}

			Color color = renderColor();
			if (_tmp.color != color)
			{
				_tmp.color = color;
			}

			_textRenderer.Present((int)pointX + Gutter, (int)pointY + Gutter, TextScale, TextScale, false, color);
		}

		private void createField()
		{
			_textRenderer = FlxGame.NewRenderer("FlxText", typeof(RectTransform));
			_rect = (RectTransform)_textRenderer.gameObject.transform;
			_rect.pivot = new Vector2(0f, 1f);
			_rect.sizeDelta = new Vector2(100f, 10f);

			_tmp = _textRenderer.gameObject.AddComponent<TextMeshPro>();
			_tmp.alignment = TextAlignmentOptions.TopLeft;
			_tmp.textWrappingMode = TextWrappingModes.Normal;
			_tmp.overflowMode = TextOverflowModes.Overflow;
			_tmp.richText = false;
			_tmp.fontSize = (float)_size;

			TMP_FontAsset fontAsset = ResolveFont();
			if (fontAsset != null)
			{
				_tmp.font = fontAsset;
			}

			_textRenderer.renderer = _tmp.renderer;
		}

		private static TMP_FontAsset ResolveFont()
		{
			if (!fontResolved || font == null)
			{
				fontResolved = true;
				font = Resources.Load<TMP_FontAsset>(FontResourcePath);
				if (font == null)
				{
					Debug.LogWarning("OdeToPixelDays: no TMP font asset at Resources/" + FontResourcePath + " - falling back to the default font.");
				}
			}

			return font;
		}
	}
}
