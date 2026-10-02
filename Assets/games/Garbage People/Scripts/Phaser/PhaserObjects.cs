using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Games.GarbagePeople
{
	public enum BlendModes
	{
		NORMAL,
		ADD,
	}

	/// <summary>
	/// A Phaser 3 game object, as the game uses them: position, origin, scale, rotation
	/// (radians), alpha, scroll factor, blend mode - with Phaser's chainable setters.
	///
	/// Each one owns a Unity object that PhaserGame repositions every frame from these
	/// values, through the scene's camera, in display-list order.
	/// </summary>
	public abstract class GameObject
	{
		public double x;
		public double y;
		public double originX = 0.5;
		public double originY = 0.5;
		public double scaleX = 1;
		public double scaleY = 1;
		public double rotation;
		public double alpha = 1;
		public double scrollFactorX = 1;
		public double scrollFactorY = 1;
		public bool visible = true;
		public BlendModes blendMode = BlendModes.NORMAL;

		internal Scene scene;
		internal bool destroyed;

		public GameObject setOrigin(double x, double y)
		{
			originX = x;
			originY = y;
			return this;
		}

		public GameObject setScale(double x, double? y = null)
		{
			scaleX = x;
			scaleY = y ?? x;
			return this;
		}

		public GameObject setAlpha(double value)
		{
			alpha = value;
			return this;
		}

		public GameObject setScrollFactor(double value)
		{
			scrollFactorX = scrollFactorY = value;
			return this;
		}

		public virtual void destroy()
		{
			if (destroyed)
			{
				return;
			}

			destroyed = true;
			if (scene != null)
			{
				scene.displayList.Remove(this);
			}

			destroyRenderer();
		}

		internal abstract void preUpdate(double delta);
		internal abstract void render(int order, Camera camera);
		internal abstract void destroyRenderer();
	}

	/// An animation definition: a list of texture names played at a frame rate.
	public class AnimationConfig
	{
		public string key;
		public List<string> frames = new List<string>();
		public double frameRate = 24;
		public int repeat;
	}

	/// <summary>
	/// Phaser's Image and Sprite (one class here: a Sprite is an Image that can play
	/// animations and, made through physics.add, has a velocity). A texture is named by its
	/// Phaser key and drawn from Resources.
	/// </summary>
	public class Sprite : GameObject
	{
		/// Not part of Phaser: the game hangs "hp" on the things it smashes.
		public int hp;

		public double velocityX;
		public double velocityY;
		internal bool hasBody;

		public readonly SpriteAnimations anims;

		private string textureKey;
		private Texture2D texture;
		private uint tint = 0xffffff;
		private UnityEngine.GameObject unityObject;
		private SpriteRenderer spriteRenderer;

		public Sprite(Scene scene, double x, double y, string key)
		{
			this.scene = scene;
			this.x = x;
			this.y = y;
			anims = new SpriteAnimations(this);
			setTexture(key);
		}

		public double width
		{
			get { return texture != null ? texture.width : 0; }
		}

		public double height
		{
			get { return texture != null ? texture.height : 0; }
		}

		public Sprite setTexture(string key)
		{
			textureKey = key;
			texture = PhaserAssets.GetTexture(key);
			return this;
		}

		public Sprite setTint(uint color)
		{
			tint = color & 0xffffff;
			return this;
		}

		/// Sets the scale so the texture is drawn this many pixels wide and high.
		public Sprite setDisplaySize(double width, double height)
		{
			scaleX = width / this.width;
			scaleY = height / this.height;
			return this;
		}

		public Sprite play(string key)
		{
			anims.play(key);
			return this;
		}

		public Sprite setVelocity(double value)
		{
			velocityX = velocityY = value;
			return this;
		}

		public Sprite setVelocityX(double value)
		{
			velocityX = value;
			return this;
		}

		public Sprite setVelocityY(double value)
		{
			velocityY = value;
			return this;
		}

		// Chainable versions of the base setters that keep the Sprite type.
		public new Sprite setOrigin(double x, double y) { base.setOrigin(x, y); return this; }
		public new Sprite setScale(double x, double? y = null) { base.setScale(x, y); return this; }
		public new Sprite setAlpha(double value) { base.setAlpha(value); return this; }
		public new Sprite setScrollFactor(double value) { base.setScrollFactor(value); return this; }

		internal override void preUpdate(double delta)
		{
			anims.update(delta);
		}

		internal override void render(int order, Camera camera)
		{
			if (texture == null)
			{
				return;
			}

			if (unityObject == null)
			{
				unityObject = new UnityEngine.GameObject(textureKey);
				unityObject.transform.SetParent(PhaserGame.DisplayRoot, false);
				spriteRenderer = unityObject.AddComponent<SpriteRenderer>();
			}

			spriteRenderer.sprite = PhaserAssets.GetSprite(texture);
			spriteRenderer.sharedMaterial = PhaserAssets.Material(texture, blendMode);
			spriteRenderer.sortingOrder = order;
			spriteRenderer.enabled = visible;
			spriteRenderer.color = new Color(
				((tint >> 16) & 0xff) / 255f,
				((tint >> 8) & 0xff) / 255f,
				(tint & 0xff) / 255f,
				(float)alpha);

			// The texture's centre, in the object's own space, then through its scale,
			// rotation and position, then through the camera.
			double localX = (0.5 - originX) * width * scaleX;
			double localY = (0.5 - originY) * height * scaleY;
			double cos = Math.Cos(rotation);
			double sin = Math.Sin(rotation);
			double worldX = x + localX * cos - localY * sin;
			double worldY = y + localX * sin + localY * cos;
			Vector2 screen = camera.toScreen(worldX, worldY, scrollFactorX, scrollFactorY);

			Transform t = unityObject.transform;
			t.localPosition = PhaserGame.ToUnity(screen.x, screen.y);
			t.localRotation = Quaternion.Euler(0f, 0f, (float)(-rotation * 180 / Math.PI));
			t.localScale = new Vector3((float)(scaleX * camera.zoom), (float)(scaleY * camera.zoom), 1f);
		}

		internal override void destroyRenderer()
		{
			if (unityObject != null)
			{
				UnityEngine.Object.Destroy(unityObject);
				unityObject = null;
			}
		}
	}

	/// <summary>
	/// A sprite's animation state. Animations are defined game-wide (Phaser's
	/// AnimationManager): defining a name that exists already keeps the first definition.
	///
	/// play restarts an animation even if it is the one playing, as Phaser 3.16 does.
	/// </summary>
	public class SpriteAnimations
	{
		internal static readonly Dictionary<string, AnimationConfig> Defined = new Dictionary<string, AnimationConfig>();

		private readonly Sprite sprite;
		private AnimationConfig current;
		private int frameIndex;
		private double elapsed;
		private bool playing;
		private string next;

		internal SpriteAnimations(Sprite sprite)
		{
			this.sprite = sprite;
		}

		public void play(string key)
		{
			AnimationConfig config;
			if (!Defined.TryGetValue(key, out config) || config.frames.Count == 0)
			{
				Debug.LogWarning("GarbagePeople: no animation called '" + key + "'.");
				return;
			}

			current = config;
			frameIndex = 0;
			elapsed = 0;
			playing = true;
			next = null;
			sprite.setTexture(config.frames[0]);
		}

		/// Plays this animation when the current one completes.
		public void chain(string key)
		{
			next = key;
		}

		internal void update(double delta)
		{
			if (!playing || current == null || current.frameRate <= 0)
			{
				return;
			}

			double msPerFrame = 1000.0 / current.frameRate;
			elapsed += delta;
			while (playing && elapsed >= msPerFrame)
			{
				elapsed -= msPerFrame;
				if (frameIndex < current.frames.Count - 1)
				{
					frameIndex++;
				}
				else if (current.repeat == -1)
				{
					frameIndex = 0;
				}
				else
				{
					playing = false;
					if (next != null)
					{
						string chained = next;
						next = null;
						play(chained);
					}

					return;
				}

				sprite.setTexture(current.frames[frameIndex]);
			}
		}
	}

	/// The style a Text is made with; only alignment is set at creation in the game.
	public class TextStyle
	{
		public string align = "left";
	}

	/// <summary>
	/// Phaser's Text: canvas text with a fill colour and a drop shadow, top-left at its
	/// position (origin 0, 0), lines aligned within the widest one.
	///
	/// The game's font is Arial Black, which the browser took from the player's system.
	/// It is not ours to ship, so the open-licensed Archivo Black stands in for it. The
	/// shadow is drawn as a copy of the text, offset; Phaser also blurred it by 2 pixels.
	/// </summary>
	public class Text : GameObject
	{
		private const float TextScale = 0.1f;
		private const string FontPath = "GarbagePeople/fonts/ArchivoBlack";
		private static TMP_FontAsset font;

		public string text = "";
		public double fontSize = 16;
		public Color fill = Color.white;
		public double shadowX;
		public double shadowY;
		public Color shadowColor = Color.clear;
		public readonly TextStyle style;

		private TextMeshPro main;
		private TextMeshPro shadow;

		public Text(Scene scene, double x, double y, string text, TextStyle style)
		{
			this.scene = scene;
			this.x = x;
			this.y = y;
			this.text = text ?? "";
			this.style = style ?? new TextStyle();
			originX = 0;
			originY = 0;
		}

		public Text setText(string value)
		{
			text = value ?? "";
			return this;
		}

		/// Only the size is read from a CSS font string ("32px Arial Black"); the face is
		/// always the stand-in.
		public Text setFont(string css)
		{
			string size = css.Split(' ')[0].Replace("px", "");
			fontSize = double.Parse(size, CultureInfo.InvariantCulture);
			return this;
		}

		public Text setFill(string color)
		{
			fill = PhaserColor.Parse(color);
			return this;
		}

		public Text setShadow(double x, double y, string color, double blur)
		{
			shadowX = x;
			shadowY = y;
			shadowColor = PhaserColor.Parse(color);
			return this;
		}

		public new Text setOrigin(double x, double y) { base.setOrigin(x, y); return this; }
		public new Text setScrollFactor(double value) { base.setScrollFactor(value); return this; }
		public new Text setAlpha(double value) { base.setAlpha(value); return this; }

		internal override void preUpdate(double delta)
		{
		}

		internal override void render(int order, Camera camera)
		{
			if (shadowColor.a > 0)
			{
				if (shadow == null)
				{
					shadow = create("Text shadow");
				}

				place(shadow, order, camera, shadowX, shadowY, shadowColor);
			}

			if (main == null)
			{
				main = create("Text");
			}

			place(main, order + 1, camera, 0, 0, fill);
		}

		private void place(TextMeshPro tmp, int order, Camera camera, double offsetX, double offsetY, Color color)
		{
			if (tmp.text != text)
			{
				tmp.text = text;
			}

			if (tmp.fontSize != (float)fontSize)
			{
				tmp.fontSize = (float)fontSize;
			}

			TextAlignmentOptions align = style.align == "center" ? TextAlignmentOptions.Top
				: style.align == "right" ? TextAlignmentOptions.TopRight
				: TextAlignmentOptions.TopLeft;
			if (tmp.alignment != align)
			{
				tmp.alignment = align;
			}

			// Lines align within the widest line, as on Phaser's canvas.
			var rect = (RectTransform)tmp.transform;
			Vector2 preferred = tmp.GetPreferredValues(text);
			if (rect.sizeDelta.x != preferred.x)
			{
				rect.sizeDelta = new Vector2(preferred.x, Mathf.Max(1f, preferred.y));
			}

			color.a *= (float)alpha;
			tmp.color = color;
			tmp.sortingOrder = order;
			tmp.enabled = visible && text.Length > 0;

			// Local units are a tenth of a world unit, which is a hundred game pixels.
			double left = x - originX * preferred.x * TextScale * 100 + offsetX;
			double top = y - originY * preferred.y * TextScale * 100 + offsetY;
			Vector2 screen = camera.toScreen(left, top, scrollFactorX, scrollFactorY);
			rect.localPosition = PhaserGame.ToUnity(screen.x, screen.y);
			float s = TextScale * (float)camera.zoom;
			rect.localScale = new Vector3(s, s, 1f);
		}

		private static TextMeshPro create(string name)
		{
			var go = new UnityEngine.GameObject(name, typeof(RectTransform));
			go.transform.SetParent(PhaserGame.DisplayRoot, false);
			var rect = (RectTransform)go.transform;
			rect.pivot = new Vector2(0f, 1f);
			TextMeshPro tmp = go.AddComponent<TextMeshPro>();
			tmp.textWrappingMode = TextWrappingModes.NoWrap;
			tmp.overflowMode = TextOverflowModes.Overflow;
			tmp.richText = false;
			if (font == null)
			{
				font = Resources.Load<TMP_FontAsset>(FontPath);
			}

			if (font != null)
			{
				tmp.font = font;
			}

			return tmp;
		}

		internal override void destroyRenderer()
		{
			if (main != null)
			{
				UnityEngine.Object.Destroy(main.gameObject);
				main = null;
			}

			if (shadow != null)
			{
				UnityEngine.Object.Destroy(shadow.gameObject);
				shadow = null;
			}
		}
	}

	internal static class PhaserColor
	{
		public static Color Parse(string css)
		{
			Color color;
			return ColorUtility.TryParseHtmlString(css, out color) ? color : Color.white;
		}
	}
}
