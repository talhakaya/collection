using System;
using System.Collections.Generic;

namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// flash.utils.Timer: fires its listener every delay milliseconds, repeatCount times
	/// (0 for ever). Runs on frame time; see FlashPlayer.
	/// </summary>
	public class FlashTimer
	{
		private static readonly List<FlashTimer> running = new List<FlashTimer>();

		public double delay;
		public int repeatCount;
		public int currentCount;

		private double elapsed;
		private readonly List<Action> listeners = new List<Action>();

		public FlashTimer(double delay, int repeatCount = 0)
		{
			this.delay = delay;
			this.repeatCount = repeatCount;
		}

		public void addEventListener(string type, Action listener)
		{
			if (!listeners.Contains(listener))
			{
				listeners.Add(listener);
			}
		}

		public void removeEventListener(string type, Action listener)
		{
			listeners.Remove(listener);
		}

		public void start()
		{
			if (!running.Contains(this))
			{
				running.Add(this);
			}
		}

		public void stop()
		{
			running.Remove(this);
		}

		public void reset()
		{
			stop();
			currentCount = 0;
			elapsed = 0;
		}

		internal static void Advance(double seconds)
		{
			foreach (FlashTimer timer in running.ToArray())
			{
				if (!running.Contains(timer))
				{
					continue;
				}

				timer.elapsed += seconds * 1000;
				while (timer.elapsed >= timer.delay && running.Contains(timer))
				{
					timer.elapsed -= timer.delay;
					timer.currentCount++;
					if (timer.repeatCount > 0 && timer.currentCount >= timer.repeatCount)
					{
						running.Remove(timer);
					}

					foreach (Action listener in timer.listeners.ToArray())
					{
						listener();
					}
				}
			}
		}

		internal static void Reset()
		{
			running.Clear();
		}
	}

	public static class TimerEvent
	{
		public const string TIMER = "timer";
	}

	/// An easing function, in the (time, begin, change, duration) form Flash used.
	public delegate double Ease(double t, double b, double c, double d);

	/// com.greensock.easing.Cubic.
	public static class Cubic
	{
		public static double easeIn(double t, double b, double c, double d)
		{
			t /= d;
			return c * t * t * t + b;
		}

		public static double easeOut(double t, double b, double c, double d)
		{
			t = t / d - 1;
			return c * (t * t * t + 1) + b;
		}
	}

	/// What a TweenLite.to call can ask for, of what the game asks for.
	public class TweenVars
	{
		public double? x;
		public double? y;
		public double? scaleX;
		public double? scaleY;
		public Ease ease;
		public Action onComplete;
	}

	/// <summary>
	/// com.greensock.TweenLite (v11), the part the game uses: TweenLite.to and
	/// killTweensOf, on a display object's position and scale.
	///
	/// Two of its defaults matter here. The ease, when none is given, is a quadratic ease
	/// out. And a new tween of an object stops every tween already running on that object
	/// (overwrite mode 1, "all immediate").
	/// </summary>
	public class TweenLite
	{
		private static readonly List<TweenLite> active = new List<TweenLite>();

		private readonly DisplayObject target;
		private readonly double duration;
		private readonly TweenVars vars;
		private readonly Ease ease;
		private double time;
		private bool started;
		private double x0, y0, sx0, sy0;

		private TweenLite(DisplayObject target, double duration, TweenVars vars)
		{
			this.target = target;
			this.duration = duration;
			this.vars = vars;
			ease = vars.ease ?? defaultEase;
		}

		/// TweenLite.defaultEase: Quad.easeOut.
		private static double defaultEase(double t, double b, double c, double d)
		{
			t /= d;
			return -c * t * (t - 2) + b;
		}

		public static TweenLite to(DisplayObject target, double duration, TweenVars vars)
		{
			killTweensOf(target);
			var tween = new TweenLite(target, duration, vars);
			active.Add(tween);
			return tween;
		}

		public static void killTweensOf(DisplayObject target)
		{
			active.RemoveAll(t => t.target == target);
		}

		internal static void Advance(double seconds)
		{
			foreach (TweenLite tween in active.ToArray())
			{
				if (active.Contains(tween))
				{
					tween.advance(seconds);
				}
			}
		}

		internal static void Reset()
		{
			active.Clear();
		}

		private void advance(double seconds)
		{
			if (!started)
			{
				// Start values are read when the tween first runs, as TweenLite does.
				started = true;
				x0 = target.x;
				y0 = target.y;
				sx0 = target.scaleX;
				sy0 = target.scaleY;
			}

			time = Math.Min(duration, time + seconds);
			double t = duration > 0 ? time : 1;
			double d = duration > 0 ? duration : 1;
			if (vars.x.HasValue) target.x = ease(t, x0, vars.x.Value - x0, d);
			if (vars.y.HasValue) target.y = ease(t, y0, vars.y.Value - y0, d);
			if (vars.scaleX.HasValue) target.scaleX = ease(t, sx0, vars.scaleX.Value - sx0, d);
			if (vars.scaleY.HasValue) target.scaleY = ease(t, sy0, vars.scaleY.Value - sy0, d);

			if (time >= duration)
			{
				active.Remove(this);
				if (vars.onComplete != null)
				{
					vars.onComplete();
				}
			}
		}
	}
}
