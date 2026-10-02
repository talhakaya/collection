using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnitySprite = UnityEngine.Sprite;

namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// Draws the display list with Unity sprites and text, from pools that are handed out
	/// afresh every frame: walk the tree in Flash's order, put each rectangle, circle,
	/// line, bitmap or text where its concatenated matrix says, each on top of the last;
	/// hide whatever was not used this frame.
	///
	/// Stage pixels map to world units at FlashPlayer.PixelsPerUnit, Y flipped, so the
	/// stage camera sees (0,0)-(640,360) with no transform of its own.
	/// </summary>
	public class FlashRenderer
	{
		private const float TextScale = 0.1f;
		private const float UnitsPerPixel = 1f / (TextScale * FlashPlayer.PixelsPerUnit);
		private const float Gutter = 2f;
		private const string FontResourcePath = "WhereLostOnesGo/fonts/Verdana";

		internal readonly Transform root;

		private readonly List<SpriteRenderer> quads = new List<SpriteRenderer>();
		private readonly List<TextMeshPro> texts = new List<TextMeshPro>();
		private readonly Dictionary<Texture2D, UnitySprite> bitmapSprites = new Dictionary<Texture2D, UnitySprite>();
		private int quadCount;
		private int textCount;
		private int order;

		private readonly UnitySprite white;
		private readonly UnitySprite disc;
		private readonly TMP_FontAsset font;

		public FlashRenderer(Transform root)
		{
			this.root = root;

			var whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
			whiteTexture.SetPixel(0, 0, Color.white);
			whiteTexture.filterMode = FilterMode.Point;
			whiteTexture.Apply();
			white = UnitySprite.Create(whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

			// Flash drew circles with smoothed edges; this one is drawn once, large, and
			// scaled down.
			const int size = 64;
			var discTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
			var pixels = new Color[size * size];
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					float dx = x + 0.5f - size / 2f;
					float dy = y + 0.5f - size / 2f;
					float coverage = Mathf.Clamp01(size / 2f - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
					pixels[y * size + x] = new Color(1f, 1f, 1f, coverage);
				}
			}

			discTexture.SetPixels(pixels);
			discTexture.filterMode = FilterMode.Bilinear;
			discTexture.Apply();
			disc = UnitySprite.Create(discTexture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);

			font = Resources.Load<TMP_FontAsset>(FontResourcePath);
		}

		public void Render(DisplayObject stageRoot)
		{
			quadCount = 0;
			textCount = 0;
			order = 0;

			if (stageRoot != null && stageRoot.visible)
			{
				stageRoot.render(this, Matrix2D.Of(stageRoot), stageRoot.alpha);
			}

			for (int i = quadCount; i < quads.Count; i++)
			{
				if (quads[i].enabled)
				{
					quads[i].enabled = false;
				}
			}

			for (int i = textCount; i < texts.Count; i++)
			{
				if (texts[i].enabled)
				{
					texts[i].enabled = false;
				}
			}
		}

		internal void Draw(Graphics.Command command, Matrix2D m, double alpha)
		{
			switch (command.kind)
			{
				case Graphics.Kind.Rect:
					box(white, m, command.x, command.y, command.w, command.h, color(command.color, command.alpha * alpha));
					break;
				case Graphics.Kind.Circle:
					box(disc, m, command.x - command.w, command.y - command.w, command.w * 2, command.w * 2, color(command.color, command.alpha * alpha));
					break;
				case Graphics.Kind.Bitmap:
					box(bitmapSprite(command.texture), m, command.x, command.y, command.w, command.h, new Color(1f, 1f, 1f, (float)alpha));
					break;
				case Graphics.Kind.Line:
					line(m, command, alpha);
					break;
			}
		}

		/// An axis-aligned box in local space, under a matrix with no skew.
		private void box(UnitySprite sprite, Matrix2D m, double x, double y, double w, double h, Color c)
		{
			Point centre = m.Apply(x + w / 2, y + h / 2);
			double sx = Math.Sqrt(m.a * m.a + m.b * m.b);
			double sy = Math.Sqrt(m.c * m.c + m.d * m.d);
			double angle = Math.Atan2(m.b, m.a) * 180 / Math.PI;
			place(sprite, centre.x, centre.y, w * sx, h * sy, angle, c);
		}

		/// Flash's lines have round ends; this one is a bar half a thickness longer at each
		/// end. Very thin lines are drawn a pixel wide, as Flash's were.
		private void line(Matrix2D m, Graphics.Command command, double alpha)
		{
			Point a = m.Apply(command.x, command.y);
			Point b = m.Apply(command.w, command.h);
			double scale = Math.Sqrt(Math.Abs(m.a * m.d - m.b * m.c));
			double thickness = Math.Max(1, command.thickness * scale);
			double dx = b.x - a.x;
			double dy = b.y - a.y;
			double length = Math.Sqrt(dx * dx + dy * dy) + thickness;
			double angle = Math.Atan2(dy, dx) * 180 / Math.PI;
			place(white, (a.x + b.x) / 2, (a.y + b.y) / 2, length, thickness, angle, color(command.color, command.alpha * alpha));
		}

		/// Puts a sprite's centre at a stage point, sized in stage pixels, turned by Flash
		/// degrees (clockwise on screen).
		private void place(UnitySprite sprite, double cx, double cy, double width, double height, double angle, Color c)
		{
			SpriteRenderer quad = nextQuad();
			quad.sprite = sprite;
			quad.color = c;

			Transform t = quad.transform;
			t.localPosition = new Vector3((float)(cx / FlashPlayer.PixelsPerUnit), (float)(-cy / FlashPlayer.PixelsPerUnit), 0f);
			t.localRotation = Quaternion.Euler(0f, 0f, (float)-angle);
			Vector2 native = sprite.bounds.size;
			t.localScale = new Vector3(
				(float)(width / FlashPlayer.PixelsPerUnit / native.x),
				(float)(height / FlashPlayer.PixelsPerUnit / native.y),
				1f);
		}

		private SpriteRenderer nextQuad()
		{
			if (quadCount == quads.Count)
			{
				var go = new GameObject("Shape");
				go.transform.SetParent(root, false);
				quads.Add(go.AddComponent<SpriteRenderer>());
			}

			SpriteRenderer quad = quads[quadCount++];
			quad.sortingOrder = order++;
			if (!quad.enabled)
			{
				quad.enabled = true;
			}

			return quad;
		}

		private UnitySprite bitmapSprite(Texture2D texture)
		{
			UnitySprite sprite;
			if (!bitmapSprites.TryGetValue(texture, out sprite) || sprite == null)
			{
				sprite = UnitySprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f);
				bitmapSprites[texture] = sprite;
			}

			return sprite;
		}

		/// <summary>
		/// A text field, top-left at its position, with Flash's 2-pixel gutter. Only
		/// translation is taken from the matrix: no text in the game is scaled or turned.
		/// </summary>
		internal void DrawText(TextField field, Matrix2D m, double alpha)
		{
			if (textCount == texts.Count)
			{
				var go = new GameObject("TextField", typeof(RectTransform));
				go.transform.SetParent(root, false);
				var rect = (RectTransform)go.transform;
				rect.pivot = new Vector2(0f, 1f);
				TextMeshPro created = go.AddComponent<TextMeshPro>();
				created.textWrappingMode = TextWrappingModes.NoWrap;
				created.overflowMode = TextOverflowModes.Overflow;
				created.richText = false;
				if (font != null)
				{
					created.font = font;
				}

				texts.Add(created);
			}

			TextMeshPro tmp = texts[textCount++];
			if (!tmp.enabled)
			{
				tmp.enabled = true;
			}

			TextFormat format = field.defaultTextFormat;
			if (tmp.text != field.text)
			{
				tmp.text = field.text;
			}

			if (tmp.fontSize != (float)format.size)
			{
				tmp.fontSize = (float)format.size;
			}

			TextAlignmentOptions align = format.align == TextFormatAlign.CENTER ? TextAlignmentOptions.Top : TextAlignmentOptions.TopLeft;
			if (tmp.alignment != align)
			{
				tmp.alignment = align;
			}

			Color c = color(format.color, alpha);
			if (tmp.color != c)
			{
				tmp.color = c;
			}

			var rectTransform = (RectTransform)tmp.transform;
			var size = new Vector2(Mathf.Max(1f, (float)field.width - Gutter * 2f) * UnitsPerPixel, (float)field.height * UnitsPerPixel);
			if (rectTransform.sizeDelta != size)
			{
				rectTransform.sizeDelta = size;
			}

			Point topLeft = m.Apply(0, 0);
			rectTransform.localPosition = new Vector3(
				(float)((topLeft.x + Gutter) / FlashPlayer.PixelsPerUnit),
				(float)(-(topLeft.y + Gutter) / FlashPlayer.PixelsPerUnit),
				0f);
			rectTransform.localScale = new Vector3(TextScale, TextScale, 1f);
			tmp.sortingOrder = order++;
		}

		private static Color color(uint rgb, double alpha)
		{
			return new Color(
				((rgb >> 16) & 0xff) / 255f,
				((rgb >> 8) & 0xff) / 255f,
				(rgb & 0xff) / 255f,
				Mathf.Clamp01((float)alpha));
		}
	}
}
