using System.Collections.Generic;
using UnityEngine;

namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Stands in for Flash's embedded assets. The source embeds every image and sound as a
	/// class - [Embed] private static var S_hans:Class = Hans_S_hans - and passes the class
	/// around. Here the class is its name as a string, and the file of that name sits under
	/// Resources/LovesFirstWeek/img or /snd, so call sites keep the identifiers the source
	/// used.
	///
	/// Textures are point-filtered and readable (the tilemap copies tile pixels out of
	/// them). Sprites are cut from them on demand, one per frame, and kept.
	/// </summary>
	public static class FlxAssets
	{
		private const string ImageRoot = "LovesFirstWeek/img/";
		private const string SoundRoot = "LovesFirstWeek/snd/";

		private struct FrameKey
		{
			public int texture;
			public int width;
			public int height;
			public int index;
		}

		private static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
		private static readonly Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
		private static readonly Dictionary<string, Texture2D> solids = new Dictionary<string, Texture2D>();
		private static readonly Dictionary<FrameKey, Sprite> frames = new Dictionary<FrameKey, Sprite>();
		private static readonly List<Object> created = new List<Object>();

		public static Texture2D GetTexture(string name)
		{
			Texture2D texture;
			if (textures.TryGetValue(name, out texture) && texture != null)
			{
				return texture;
			}

			texture = Resources.Load<Texture2D>(ImageRoot + name);
			if (texture == null)
			{
				Debug.LogError("LovesFirstWeek: no image for '" + name + "'.");
				texture = CreateTexture(16, 16, 0xffff00ff);
			}

			textures[name] = texture;
			return texture;
		}

		public static AudioClip GetSound(string name)
		{
			AudioClip clip;
			if (sounds.TryGetValue(name, out clip) && clip != null)
			{
				return clip;
			}

			clip = Resources.Load<AudioClip>(SoundRoot + name);
			if (clip == null)
			{
				Debug.LogError("LovesFirstWeek: no sound for '" + name + "'.");
			}

			sounds[name] = clip;
			return clip;
		}

		/// A solid Width x Height texture of one 0xAARRGGBB colour, shared between
		/// everything that asks for the same one.
		public static Texture2D CreateTexture(int width, int height, uint color)
		{
			string key = width + "x" + height + ":" + color;
			Texture2D texture;
			if (solids.TryGetValue(key, out texture) && texture != null)
			{
				return texture;
			}

			texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;

			var fill = new Color32(
				(byte)((color >> 16) & 0xff),
				(byte)((color >> 8) & 0xff),
				(byte)(color & 0xff),
				(byte)((color >> 24) & 0xff));
			var pixels = new Color32[width * height];
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = fill;
			}

			texture.SetPixels32(pixels);
			texture.Apply(false);

			solids[key] = texture;
			created.Add(texture);
			return texture;
		}

		/// <summary>
		/// Frame number index of a sheet cut into frameWidth x frameHeight cells, counted
		/// left to right and wrapping onto the next row - Flixel's calcFrame. The sprite's
		/// pivot is its centre, which is the origin Flixel scales a sprite about.
		/// </summary>
		public static Sprite GetFrame(Texture2D texture, int frameWidth, int frameHeight, int index)
		{
			var key = new FrameKey { texture = texture.GetInstanceID(), width = frameWidth, height = frameHeight, index = index };
			Sprite sprite;
			if (frames.TryGetValue(key, out sprite) && sprite != null)
			{
				return sprite;
			}

			int indexX = index * frameWidth;
			int indexY = 0;
			if (indexX >= texture.width)
			{
				indexY = (indexX / texture.width) * frameHeight;
				indexX %= texture.width;
			}

			// Clamped so a frame number past the end of the sheet, or a frame size that
			// does not divide it, draws what there is rather than throwing.
			int w = Mathf.Min(frameWidth, texture.width - indexX);
			int h = Mathf.Min(frameHeight, Mathf.Max(0, texture.height - indexY));
			if (w <= 0 || h <= 0)
			{
				indexX = 0;
				indexY = 0;
				w = Mathf.Min(frameWidth, texture.width);
				h = Mathf.Min(frameHeight, texture.height);
			}

			// Flash measures from the top of the image, Unity from the bottom.
			var rect = new Rect(indexX, texture.height - indexY - h, w, h);
			sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), FlxGame.PixelsPerUnit, 0, SpriteMeshType.FullRect);
			frames[key] = sprite;
			created.Add(sprite);
			return sprite;
		}

		/// Dropped when the game is left, so nothing made at runtime outlives it.
		public static void Clear()
		{
			for (int i = 0; i < created.Count; i++)
			{
				if (created[i] != null)
				{
					Object.Destroy(created[i]);
				}
			}

			created.Clear();
			frames.Clear();
			solids.Clear();
			textures.Clear();
			sounds.Clear();
		}
	}
}
