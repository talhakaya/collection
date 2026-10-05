using System;
using System.Collections.Generic;
using System.Globalization;
using Collection.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Games.GarbagePeople
{
	/// <summary>
	/// A scene's camera (Phaser 3.16 Cameras.Scene2D.Camera): a scroll position, a zoom
	/// about the middle of the screen, optional bounds, and following a game object.
	///
	/// A point on screen is the middle of the screen plus zoom times (the point, less the
	/// scroll times the object's scroll factor, less the middle) - so an object with a
	/// scroll factor of 0 still moves with the zoom, as in Phaser.
	/// </summary>
	public class Camera
	{
		public double scrollX;
		public double scrollY;
		public double zoom = 1;
		public double width = PhaserGame.WIDTH;
		public double height = PhaserGame.HEIGHT;

		private GameObject follow;
		private double lerpX = 1;
		private double lerpY = 1;
		private bool useBounds;
		private double boundsX, boundsY, boundsWidth, boundsHeight;

		public Camera setZoom(double value)
		{
			zoom = value;
			return this;
		}

		public Camera setBounds(double x, double y, double width, double height)
		{
			useBounds = true;
			boundsX = x;
			boundsY = y;
			boundsWidth = width;
			boundsHeight = height;
			scrollX = clampX(scrollX);
			scrollY = clampY(scrollY);
			return this;
		}

		public Camera startFollow(GameObject target, bool roundPixels = false, double lerpX = 1, double lerpY = 1)
		{
			follow = target;
			this.lerpX = lerpX;
			this.lerpY = lerpY;
			scrollX = target.x - width * 0.5;
			scrollY = target.y - height * 0.5;
			if (useBounds)
			{
				scrollX = clampX(scrollX);
				scrollY = clampY(scrollY);
			}

			return this;
		}

		/// Camera.preRender: follow, then keep inside the bounds.
		internal void preRender()
		{
			if (follow != null)
			{
				scrollX += (follow.x - width * 0.5 - scrollX) * lerpX;
				scrollY += (follow.y - height * 0.5 - scrollY) * lerpY;
			}

			if (useBounds)
			{
				scrollX = clampX(scrollX);
				scrollY = clampY(scrollY);
			}
		}

		private double clampX(double x)
		{
			double displayWidth = width / zoom;
			double bx = boundsX + (displayWidth - width) / 2;
			double bw = Math.Max(bx, bx + boundsWidth - displayWidth);
			return x < bx ? bx : x > bw ? bw : x;
		}

		private double clampY(double y)
		{
			double displayHeight = height / zoom;
			double by = boundsY + (displayHeight - height) / 2;
			double bh = Math.Max(by, by + boundsHeight - displayHeight);
			return y < by ? by : y > bh ? bh : y;
		}

		internal Vector2 toScreen(double x, double y, double scrollFactorX, double scrollFactorY)
		{
			double cx = width * 0.5;
			double cy = height * 0.5;
			return new Vector2(
				(float)(cx + zoom * (x - scrollX * scrollFactorX - cx)),
				(float)(cy + zoom * (y - scrollY * scrollFactorY - cy)));
		}
	}

	/// <summary>
	/// A tween's end value: a number, or a string "+=n" / "-=n" meaning that much more or
	/// less than the value when the tween starts.
	/// </summary>
	public class TweenValue
	{
		public double value;
		public int relative;

		public static implicit operator TweenValue(double v)
		{
			return new TweenValue { value = v };
		}

		public static implicit operator TweenValue(string s)
		{
			s = s.Trim();
			int sign = s.StartsWith("+=") ? 1 : s.StartsWith("-=") ? -1 : 0;
			string number = (sign != 0 ? s.Substring(2) : s).Trim().Replace(',', '.');
			return new TweenValue { value = double.Parse(number, CultureInfo.InvariantCulture), relative = sign };
		}

		internal double end(double start)
		{
			return relative == 0 ? value : start + relative * value;
		}
	}

	/// What tweens.add takes, of what the game passes.
	public class TweenConfig
	{
		public GameObject targets;
		public TweenValue x;
		public TweenValue y;
		public TweenValue rotation;
		public TweenValue scaleX;
		public TweenValue scaleY;
		public TweenValue alpha;
		public string ease = "Power0";
		public double duration = 1000;
		public double delay;
		public int repeat;
		public double repeatDelay;
		public bool yoyo;
	}

	/// <summary>
	/// Phaser 3.16's TweenManager, for what the game does with it.
	///
	/// Each property of a tween runs on its own: it waits out the delay, then reads its
	/// start value - so a relative tween that starts later builds on whatever earlier ones
	/// left - eases to the end, back again if yoyo, and repeats after repeatDelay. Tweens
	/// on the same object and property all run; the one added last is written last.
	/// </summary>
	public class TweenManager
	{
		private class Track
		{
			public GameObject target;
			public string property;
			public TweenValue to;
			public Func<double, double> ease;
			public double duration;
			public double delayLeft;
			public double repeatDelay;
			public int repeatsLeft;
			public bool yoyo;
			public double start;
			public double end;
			public double elapsed;
			public int stage; // 0 waiting, 1 forward, 2 back, 3 between repeats, 4 done
		}

		private readonly List<Track> tracks = new List<Track>();

		public void add(TweenConfig config)
		{
			Func<double, double> ease = Ease(config.ease);
			addTrack(config, "x", config.x, ease);
			addTrack(config, "y", config.y, ease);
			addTrack(config, "rotation", config.rotation, ease);
			addTrack(config, "scaleX", config.scaleX, ease);
			addTrack(config, "scaleY", config.scaleY, ease);
			addTrack(config, "alpha", config.alpha, ease);
		}

		private void addTrack(TweenConfig config, string property, TweenValue to, Func<double, double> ease)
		{
			if (to == null || config.targets == null)
			{
				return;
			}

			tracks.Add(new Track
			{
				target = config.targets,
				property = property,
				to = to,
				ease = ease,
				duration = Math.Max(1, config.duration),
				delayLeft = config.delay,
				repeatDelay = config.repeatDelay,
				repeatsLeft = config.repeat,
				yoyo = config.yoyo,
			});
		}

		internal void update(double delta)
		{
			for (int i = 0; i < tracks.Count; i++)
			{
				step(tracks[i], delta);
			}

			tracks.RemoveAll(t => t.stage == 4);
		}

		internal void clear()
		{
			tracks.Clear();
		}

		private static void step(Track t, double delta)
		{
			while (delta > 0 && t.stage != 4)
			{
				switch (t.stage)
				{
					case 0:
					case 3:
					{
						double wait = t.stage == 0 ? t.delayLeft : t.repeatDelay;
						if (delta < wait - t.elapsed)
						{
							t.elapsed += delta;
							return;
						}

						delta -= wait - t.elapsed;
						t.elapsed = 0;
						t.start = get(t.target, t.property);
						t.end = t.to.end(t.start);
						t.stage = 1;
						break;
					}

					case 1:
					case 2:
					{
						double used = Math.Min(delta, t.duration - t.elapsed);
						t.elapsed += used;
						delta -= used;
						double progress = t.elapsed / t.duration;
						double v = t.stage == 1 ? t.ease(progress) : t.ease(1 - progress);
						set(t.target, t.property, t.start + (t.end - t.start) * v);
						if (t.elapsed >= t.duration)
						{
							t.elapsed = 0;
							if (t.stage == 1 && t.yoyo)
							{
								t.stage = 2;
							}
							else if (t.repeatsLeft != 0)
							{
								if (t.repeatsLeft > 0)
								{
									t.repeatsLeft--;
								}

								t.stage = 3;
							}
							else
							{
								t.stage = 4;
							}
						}

						break;
					}
				}
			}
		}

		private static double get(GameObject o, string property)
		{
			switch (property)
			{
				case "x": return o.x;
				case "y": return o.y;
				case "rotation": return o.rotation;
				case "scaleX": return o.scaleX;
				case "scaleY": return o.scaleY;
				default: return o.alpha;
			}
		}

		private static void set(GameObject o, string property, double value)
		{
			switch (property)
			{
				case "x": o.x = value; break;
				case "y": o.y = value; break;
				case "rotation": o.rotation = value; break;
				case "scaleX": o.scaleX = value; break;
				case "scaleY": o.scaleY = value; break;
				default: o.alpha = value; break;
			}
		}

		private static Func<double, double> Ease(string name)
		{
			switch (name)
			{
				case "Sine.easeInOut":
					return v => -0.5 * (Math.Cos(Math.PI * v) - 1);
				case "Linear":
				case "Power0":
					return v => v;
				default:
					Debug.LogWarning("GarbagePeople: ease '" + name + "' is not ported; using Linear.");
					return v => v;
			}
		}
	}

	/// What time.addEvent takes.
	public class TimerConfig
	{
		public double delay;
		public Action callback;
	}

	/// A one-shot Phaser TimerEvent.
	public class TimerEvent
	{
		internal double delay;
		internal double elapsed;
		internal Action callback;
		internal bool done;

		/// How far through its delay it is, 0 to 1.
		public double getProgress()
		{
			return delay > 0 ? Math.Min(1, elapsed / delay) : 1;
		}
	}

	/// A scene's Clock.
	public class Clock
	{
		private readonly List<TimerEvent> events = new List<TimerEvent>();

		public TimerEvent addEvent(TimerConfig config)
		{
			var e = new TimerEvent { delay = config.delay, callback = config.callback };
			events.Add(e);
			return e;
		}

		internal void update(double delta)
		{
			foreach (TimerEvent e in events.ToArray())
			{
				if (e.done)
				{
					continue;
				}

				e.elapsed += delta;
				if (e.elapsed >= e.delay)
				{
					e.elapsed = e.delay;
					e.done = true;
					if (e.callback != null)
					{
						e.callback();
					}
				}
			}

			events.RemoveAll(e => e.done);
		}

		internal void clear()
		{
			events.Clear();
		}
	}

	/// <summary>
	/// A keyboard key, as Phaser.Input.Keyboard.Key: isDown, and a "just down" flag set
	/// when it goes down and cleared when JustDown reads it.
	///
	/// Each key is an action in the game's input map, so gamepad buttons drive it too.
	/// </summary>
	public class Key
	{
		internal string action;
		public bool isDown;
		internal bool justDown;

		// In the collection: a key that was already held when a scene asked for it. In
		// Phaser a scene's new key is up until the next key-down event, so Space still held
		// from the dialogue before a level did not swing at once. It counts as up here too,
		// until it has been let go.
		internal bool heldFromBefore;

		internal void poll()
		{
			InputAction a = TaloketoInputManager.GetAction(action);
			bool down = a != null && a.IsPressed();
			if (heldFromBefore)
			{
				heldFromBefore = down;
				down = false;
			}

			if (down && !isDown)
			{
				justDown = true;
				if (a.activeControl != null)
				{
					PhaserInput.usingGamepad = a.activeControl.device is Gamepad;
				}
			}

			isDown = down;
		}
	}

	public class CursorKeys
	{
		public Key left;
		public Key right;
		public Key up;
		public Key down;
	}

	public static class KeyCodes
	{
		public const string SPACE = "SPACE";
		public const string Z = "Z";
		public const string X = "X";
	}

	/// <summary>
	/// The keyboard plugin a scene reaches as input.keyboard, and Phaser.Input.Keyboard's
	/// JustDown.
	///
	/// Also, not in Phaser: whether the last key pressed came from a gamepad, so prompts
	/// can name the pad's buttons.
	/// </summary>
	public class PhaserInput
	{
		public static bool usingGamepad;

		private readonly List<Key> keys = new List<Key>();

		public PhaserInput keyboard
		{
			get { return this; }
		}

		public Key addKey(string code)
		{
			var key = new Key { action = code };
			key.poll();
			key.heldFromBefore = key.isDown;
			key.isDown = false;
			key.justDown = false;
			keys.Add(key);
			return key;
		}

		public CursorKeys createCursorKeys()
		{
			return new CursorKeys
			{
				left = addKey("LEFT"),
				right = addKey("RIGHT"),
				up = addKey("UP"),
				down = addKey("DOWN"),
			};
		}

		public static bool JustDown(Key key)
		{
			if (key != null && key.justDown)
			{
				key.justDown = false;
				return true;
			}

			return false;
		}

		internal void poll()
		{
			for (int i = 0; i < keys.Count; i++)
			{
				keys[i].poll();
			}
		}

		internal void clear()
		{
			keys.Clear();
		}
	}

	/// <summary>
	/// One sound (Phaser's WebAudioSound). Phaser's sound manager belongs to the game, not
	/// a scene, so sounds play on across a change of scene until stopped.
	/// </summary>
	public class Sound
	{
		private static readonly List<Sound> playing = new List<Sound>();

		private readonly string key;
		private AudioSource source;
		private bool loop;

		internal Sound(string key)
		{
			this.key = key;
		}

		public Sound setLoop(bool value)
		{
			loop = value;
			if (source != null)
			{
				source.loop = value;
			}

			return this;
		}

		public void play()
		{
			if (source == null)
			{
				source = PhaserGame.NewAudioSource(key);
				source.clip = PhaserAssets.GetSound(key);
			}

			source.loop = loop;
			source.Play();
			if (!playing.Contains(this))
			{
				playing.Add(this);
			}
		}

		public void stop()
		{
			if (source != null)
			{
				source.Stop();
			}
		}

		/// Finished one-shots are let go.
		internal static void Tidy()
		{
			for (int i = playing.Count - 1; i >= 0; i--)
			{
				Sound s = playing[i];
				if (s.source == null || (!s.loop && !s.source.isPlaying))
				{
					if (s.source != null)
					{
						UnityEngine.Object.Destroy(s.source.gameObject);
						s.source = null;
					}

					playing.RemoveAt(i);
				}
			}
		}

		internal static void StopAll()
		{
			foreach (Sound s in playing)
			{
				if (s.source != null)
				{
					s.source.Stop();
				}
			}

			playing.Clear();
		}
	}

	/// The scene's sound plugin: sound.add.
	public class SoundPlugin
	{
		public Sound add(string key)
		{
			return new Sound(key);
		}
	}

	/// Phaser.Math, the parts used.
	public static class PhaserMath
	{
		private static readonly System.Random rng = new System.Random();

		/// A whole number from min to max, both included.
		public static int Between(int min, int max)
		{
			return rng.Next(min, max + 1);
		}

		public static double FloatBetween(double min, double max)
		{
			return min + rng.NextDouble() * (max - min);
		}

		public static double DistanceBetween(double x1, double y1, double x2, double y2)
		{
			double dx = x1 - x2;
			double dy = y1 - y2;
			return Math.Sqrt(dx * dx + dy * dy);
		}
	}
}
