using System.Collections.Generic;
using UnityEngine;

namespace Games.LovesFirstWeek
{
	/// org.flixel.system.FlxAnim.
	public class FlxAnim
	{
		public string name;
		public double delay;
		public int[] frames;
		public bool looped;

		public FlxAnim(string Name, int[] Frames, double FrameRate = 0, bool Looped = true)
		{
			name = Name;
			delay = 0;
			if (FrameRate > 0)
			{
				delay = 1.0 / FrameRate;
			}

			frames = Frames;
			looped = Looped;
		}
	}

	/// <summary>
	/// org.flixel.FlxSprite: an FlxObject that draws one frame of a sprite sheet.
	///
	/// The collision box (x, y, width, height) and the picture are separate, joined by
	/// offset - a sheet frame can be bigger or smaller than the box, and the game sets all
	/// three by hand for every size of Hans. Scaling happens about the centre of the frame,
	/// which is why a low-resolution sprite scaled up 8x still needs its own offsets.
	///
	/// Embedded graphics are named by the class the source embedded them as ("Hans_S_hans");
	/// FlxAssets resolves the name.
	/// </summary>
	public class FlxSprite : FlxObject
	{
		private const string ImgDefault = "FlxSprite_ImgDefault";

		public FlxPoint origin;
		public FlxPoint offset;
		public FlxPoint scale;
		public bool finished;
		public int frameWidth;
		public int frameHeight;
		public int frames;
		public bool dirty;

		protected List<FlxAnim> _animations;
		protected int _flipped;
		protected FlxAnim _curAnim;
		protected int _curFrame;
		protected int _curIndex;
		protected double _frameTimer;
		protected uint _facing;
		protected double _alpha;
		protected uint _color;
		protected Texture2D _pixels;

		private FlxRenderer _renderer;

		public FlxSprite(double X = 0, double Y = 0, string SimpleGraphic = null) : base(X, Y)
		{
			health = 1;

			offset = new FlxPoint();
			origin = new FlxPoint();

			scale = new FlxPoint(1.0, 1.0);
			_alpha = 1;
			_color = 0x00ffffff;
			finished = false;
			_facing = RIGHT;
			_animations = new List<FlxAnim>();
			_flipped = 0;
			_curAnim = null;
			_curFrame = 0;
			_curIndex = 0;
			_frameTimer = 0;

			if (SimpleGraphic == null)
			{
				SimpleGraphic = ImgDefault;
			}

			loadGraphic(SimpleGraphic);
		}

		public override void destroy()
		{
			if (_animations != null)
			{
				_animations.Clear();
				_animations = null;
			}

			if (_renderer != null)
			{
				_renderer.Destroy();
				_renderer = null;
			}

			offset = null;
			origin = null;
			scale = null;
			_curAnim = null;
			base.destroy();
		}

		/// <summary>
		/// Loads a sheet. Reverse means "this sprite faces both ways": Flixel built a
		/// mirrored copy of the sheet beside the original and drew from it when facing left.
		/// Here the same frame is drawn flipped, which is the same picture.
		///
		/// With no frame size given, an animated sheet is taken to be a strip of squares as
		/// tall as the image.
		/// </summary>
		public FlxSprite loadGraphic(string Graphic, bool Animated = false, bool Reverse = false, int Width = 0, int Height = 0, bool Unique = false)
		{
			_pixels = FlxAssets.GetTexture(Graphic);
			_flipped = Reverse ? _pixels.width : 0;

			if (Width == 0)
			{
				if (Animated)
				{
					Width = _pixels.height;
				}
				else
				{
					Width = _pixels.width;
				}
			}

			width = frameWidth = Width;
			if (Height == 0)
			{
				if (Animated)
				{
					Height = (int)width;
				}
				else
				{
					Height = _pixels.height;
				}
			}

			height = frameHeight = Height;
			resetHelpers();
			return this;
		}

		/// A solid rectangle. Color is 0xAARRGGBB.
		public FlxSprite makeGraphic(int Width, int Height, uint Color = 0xffffffff, bool Unique = false, string Key = null)
		{
			_pixels = FlxAssets.CreateTexture(Width, Height, Color);
			_flipped = 0;
			width = frameWidth = _pixels.width;
			height = frameHeight = _pixels.height;
			resetHelpers();
			return this;
		}

		protected void resetHelpers()
		{
			origin.make(frameWidth * 0.5, frameHeight * 0.5);
			frames = (_pixels.width / frameWidth) * (_pixels.height / frameHeight);
			_curIndex = 0;
			dirty = true;
		}

		public override void postUpdate()
		{
			base.postUpdate();
			updateAnimation();
		}

		public override void draw()
		{
			if (_flickerTimer != 0)
			{
				_flicker = !_flicker;
				if (_flicker)
				{
					return;
				}
			}

			dirty = false;

			FlxCamera camera = FlxG.camera;
			_point.x = x - (int)(camera.scroll.x * scrollFactor.x) - offset.x;
			_point.y = y - (int)(camera.scroll.y * scrollFactor.y) - offset.y;
			_point.x += _point.x > 0 ? 0.0000001 : -0.0000001;
			_point.y += _point.y > 0 ? 0.0000001 : -0.0000001;

			present(_point.x, _point.y);
		}

		/// <summary>
		/// Puts the current frame on screen with its top-left corner at (pointX, pointY) in
		/// screen pixels - or, when scaled, scaled about its origin from there.
		///
		/// Unscaled sprites land on whole pixels, as Flixel's copyPixels put them. Scaled
		/// ones keep their fractional position, as its matrix draw did; the game renders at
		/// its native 320x240, so the result is sampled onto the same pixel grid either way.
		/// </summary>
		protected virtual void present(double pointX, double pointY)
		{
			if (_renderer == null || !_renderer.Alive)
			{
				_renderer = FlxGame.NewRenderer(GetType().Name);
			}

			_renderer.sprite.sprite = FlxAssets.GetFrame(_pixels, frameWidth, frameHeight, _curIndex);

			bool simple = scale.x == 1 && scale.y == 1;
			double left = simple ? (int)pointX : pointX;
			double top = simple ? (int)pointY : pointY;

			_renderer.Present(left + origin.x, top + origin.y, scale.x, scale.y, _flipped != 0 && _facing == LEFT, renderColor());
		}

		protected Color renderColor()
		{
			return new Color(
				((_color >> 16) & 0xff) / 255f,
				((_color >> 8) & 0xff) / 255f,
				(_color & 0xff) / 255f,
				(float)_alpha);
		}

		protected void updateAnimation()
		{
			if (_curAnim != null && _curAnim.delay > 0 && (_curAnim.looped || !finished))
			{
				_frameTimer += FlxG.elapsed;
				while (_frameTimer > _curAnim.delay)
				{
					_frameTimer = _frameTimer - _curAnim.delay;
					if (_curFrame == _curAnim.frames.Length - 1)
					{
						if (_curAnim.looped)
						{
							_curFrame = 0;
						}

						finished = true;
					}
					else
					{
						_curFrame++;
					}

					_curIndex = _curAnim.frames[_curFrame];
					dirty = true;
				}
			}
		}

		public void addAnimation(string Name, int[] Frames, double FrameRate = 0, bool Looped = true)
		{
			_animations.Add(new FlxAnim(Name, Frames, FrameRate, Looped));
		}

		public void play(string AnimName, bool Force = false)
		{
			if (!Force && _curAnim != null && AnimName == _curAnim.name && (_curAnim.looped || !finished))
			{
				return;
			}

			_curFrame = 0;
			_curIndex = 0;
			_frameTimer = 0;
			for (int i = 0; i < _animations.Count; i++)
			{
				if (_animations[i].name == AnimName)
				{
					_curAnim = _animations[i];
					if (_curAnim.delay <= 0)
					{
						finished = true;
					}
					else
					{
						finished = false;
					}

					_curIndex = _curAnim.frames[_curFrame];
					dirty = true;
					return;
				}
			}

			Debug.LogWarning("Flixel: no animation called \"" + AnimName + "\" on " + GetType().Name);
		}

		public void randomFrame()
		{
			_curAnim = null;
			_curIndex = (int)(FlxG.random() * (_pixels.width / frameWidth));
			dirty = true;
		}

		public void centerOffsets(bool AdjustPosition = false)
		{
			offset.x = (frameWidth - width) * 0.5;
			offset.y = (frameHeight - height) * 0.5;
			if (AdjustPosition)
			{
				x += offset.x;
				y += offset.y;
			}
		}

		public uint facing
		{
			get { return _facing; }
			set
			{
				if (_facing != value)
				{
					dirty = true;
				}

				_facing = value;
			}
		}

		public double alpha
		{
			get { return _alpha; }
			set
			{
				if (value > 1)
				{
					value = 1;
				}

				if (value < 0)
				{
					value = 0;
				}

				if (value == _alpha)
				{
					return;
				}

				_alpha = value;
				dirty = true;
			}
		}

		/// 0xRRGGBB tint; the top byte is ignored, as in Flixel.
		public uint color
		{
			get { return _color; }
			set
			{
				value &= 0x00ffffff;
				if (_color == value)
				{
					return;
				}

				_color = value;
				dirty = true;
			}
		}

		public int frame
		{
			get { return _curIndex; }
			set
			{
				_curAnim = null;
				_curIndex = value;
				dirty = true;
			}
		}
	}
}
