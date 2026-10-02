using System;
using System.Collections.Generic;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// org.flixel.FlxG: the global everything in a Flixel game reaches for - the current
	/// state, the camera, input, sound, and the collision entry points.
	///
	/// Static, as in Flixel, and reset by FlxGame when the game starts so nothing carries
	/// over from the last time it was played.
	/// </summary>
	public static class FlxG
	{
		/// Seconds per game step. Flixel's step is a whole number of milliseconds
		/// (1000 / 60 truncated to 16), so this is 0.016, not 1/60.
		public static double elapsed;
		public static double timeScale;

		public static int width;
		public static int height;

		public static FlxRect worldBounds;
		public static uint worldDivisions;

		public static FlxSound music;
		public static FlxKeyboard keys;
		public static FlxMouse mouse;
		public static FlxCamera camera;
		public static List<FlxCamera> cameras;

		public static bool mute;

		internal static FlxGame _game;

		private static double _volume;
		private static uint _bgColor;
		private static readonly List<FlxSound> sounds = new List<FlxSound>();
		private static readonly Random rng = new Random();

		public static FlxState state
		{
			get { return _game != null ? _game._state : null; }
		}

		/// ActionScript's Math.random(): 0 <= n < 1.
		public static double random()
		{
			return rng.NextDouble();
		}

		public static void switchState(FlxState State)
		{
			_game._requestedState = State;
		}

		/// A fresh instance of whatever the current state is - how a level restarts.
		public static void resetState()
		{
			_game._requestedState = (FlxState)Activator.CreateInstance(_game._state.GetType());
		}

		/// 0xAARRGGBB. The game sets this constantly: it is the sky, and it flashes dark
		/// when Hans is hurt.
		public static uint bgColor
		{
			get { return camera == null ? _bgColor : camera.bgColor; }
			set
			{
				_bgColor = value;
				if (cameras == null)
				{
					return;
				}

				for (int i = 0; i < cameras.Count; i++)
				{
					cameras[i].bgColor = value;
				}
			}
		}

		public static double volume
		{
			get { return _volume; }
			set
			{
				_volume = value;
				if (_volume < 0)
				{
					_volume = 0;
				}
				else if (_volume > 1)
				{
					_volume = 1;
				}

				if (music != null)
				{
					music.updateTransform();
				}

				for (int i = 0; i < sounds.Count; i++)
				{
					sounds[i].updateTransform();
				}
			}
		}

		/// Music loops, is not restarted by a change of state, and there is one of it.
		public static void playMusic(string Music, double Volume = 1.0)
		{
			if (music == null)
			{
				music = new FlxSound();
			}
			else if (music.active)
			{
				music.stop();
			}

			music.loadEmbedded(Music, true);
			music.volume = Volume;
			music.survive = true;
			music.play();
		}

		public static FlxSound play(string EmbeddedSound, double Volume = 1.0, bool Looped = false, bool AutoDestroy = true)
		{
			// Reuse a finished one-shot rather than growing the pile.
			FlxSound sound = null;
			for (int i = 0; i < sounds.Count; i++)
			{
				if (sounds[i].done)
				{
					sound = sounds[i];
					break;
				}
			}

			if (sound == null)
			{
				sound = new FlxSound();
				sounds.Add(sound);
			}

			sound.loadEmbedded(EmbeddedSound, Looped, AutoDestroy);
			sound.volume = Volume;
			sound.play();
			return sound;
		}

		/// On a change of state: everything but sounds marked to survive, which in
		/// practice means everything but the music.
		internal static void destroySounds(bool ForceDestroy = false)
		{
			if (music != null && (ForceDestroy || !music.survive))
			{
				music.destroy();
				music = null;
			}

			for (int i = sounds.Count - 1; i >= 0; i--)
			{
				if (ForceDestroy || !sounds[i].survive)
				{
					sounds[i].destroy();
					sounds.RemoveAt(i);
				}
			}
		}

		internal static void resetCameras()
		{
			if (cameras != null)
			{
				for (int i = 0; i < cameras.Count; i++)
				{
					cameras[i].destroy();
				}
			}

			camera = new FlxCamera(0, 0, width, height);
			cameras = new List<FlxCamera> { camera };
		}

		internal static void resetInput()
		{
			keys.reset();
			mouse.reset();
		}

		internal static void updateCameras()
		{
			for (int i = 0; i < cameras.Count; i++)
			{
				FlxCamera cam = cameras[i];
				if (cam != null && cam.exists && cam.active)
				{
					cam.update();
				}
			}
		}

		/// FlxG.reset(), plus the parts of FlxG.init() that matter here.
		internal static void init(FlxGame Game, int Width, int Height)
		{
			_game = Game;
			width = Width;
			height = Height;

			mute = false;
			_volume = 0.5;
			sounds.Clear();
			music = null;

			keys = new FlxKeyboard();
			mouse = new FlxMouse();

			_bgColor = 0xff000000;
			cameras = null;
			resetCameras();

			timeScale = 1.0;
			elapsed = 0;
			worldBounds = new FlxRect(-10, -10, width + 20, height + 20);
			worldDivisions = 6;
		}

		internal static void shutdown()
		{
			destroySounds(true);
			FlxTimerManager.clear();
			camera = null;
			cameras = null;
			_game = null;
		}

		/// <summary>
		/// Calls NotifyCallback for every overlapping pair between the two objects or
		/// groups (or within the first, if there is no second). With a ProcessCallback, a
		/// pair only counts if that returns true - collide passes FlxObject.separate, so a
		/// pair counts if it actually had to be pushed apart.
		/// </summary>
		public static bool overlap(FlxBasic ObjectOrGroup1 = null, FlxBasic ObjectOrGroup2 = null, Action<FlxObject, FlxObject> NotifyCallback = null, Func<FlxObject, FlxObject, bool> ProcessCallback = null)
		{
			if (ObjectOrGroup1 == null)
			{
				ObjectOrGroup1 = FlxG.state;
			}

			if (ObjectOrGroup2 == ObjectOrGroup1)
			{
				ObjectOrGroup2 = null;
			}

			FlxQuadTree.divisions = FlxG.worldDivisions;
			var quadTree = new FlxQuadTree(FlxG.worldBounds.x, FlxG.worldBounds.y, FlxG.worldBounds.width, FlxG.worldBounds.height);
			quadTree.load(ObjectOrGroup1, ObjectOrGroup2, NotifyCallback, ProcessCallback);
			bool result = quadTree.execute();
			quadTree.destroy();
			return result;
		}

		public static bool collide(FlxBasic ObjectOrGroup1 = null, FlxBasic ObjectOrGroup2 = null, Action<FlxObject, FlxObject> NotifyCallback = null)
		{
			return overlap(ObjectOrGroup1, ObjectOrGroup2, NotifyCallback, FlxObject.separate);
		}

		// The game's callbacks name the types they expect (a Hans and a Monster, a tilemap
		// and a falling block), which ActionScript coerced at the call. C# cannot infer
		// those from a method group, so call sites spell them out: overlap<Hans, Monster>.
		//
		// A pair of the wrong types is skipped. In Flash the coercion threw, which the
		// release player swallowed - the callback did not run there either.

		public static bool overlap<T1, T2>(FlxBasic ObjectOrGroup1, FlxBasic ObjectOrGroup2, Action<T1, T2> NotifyCallback)
			where T1 : FlxObject where T2 : FlxObject
		{
			return overlap(ObjectOrGroup1, ObjectOrGroup2, (a, b) => { if (a is T1 && b is T2) NotifyCallback((T1)a, (T2)b); }, null);
		}

		public static bool collide<T1, T2>(FlxBasic ObjectOrGroup1, FlxBasic ObjectOrGroup2, Action<T1, T2> NotifyCallback)
			where T1 : FlxObject where T2 : FlxObject
		{
			return overlap(ObjectOrGroup1, ObjectOrGroup2, (a, b) => { if (a is T1 && b is T2) NotifyCallback((T1)a, (T2)b); }, FlxObject.separate);
		}
	}
}
