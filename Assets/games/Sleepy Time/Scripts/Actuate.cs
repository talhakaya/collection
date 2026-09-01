using System.Collections.Generic;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// Stands in for motion.Actuate, reduced to what Sleepy Time asks of it: tween a
	/// FlashObject's x and/or y over a number of seconds, and stop whatever is already
	/// tweening a target. That is the whole surface - the hand's swing, and the screens
	/// sliding in and out.
	///
	/// Actuate's default easing is Expo.easeOut, so that is the curve here.
	///
	/// Driven from real time rather than GameManager.dt, because Actuate was: it ran on its
	/// own ENTER_FRAME and kept going while GameManager.paused froze dt.
	/// </summary>
	public static class Actuate
	{
		private class Tween
		{
			public FlashObject target;
			public float duration;
			public float elapsed;
			public bool tweenX;
			public bool tweenY;
			public float fromX;
			public float fromY;
			public float toX;
			public float toY;
		}

		private static readonly List<Tween> tweens = new List<Tween>();

		public static void tween(FlashObject target, float seconds, float? x = null, float? y = null)
		{
			if (target == null)
			{
				return;
			}

			Tween t = new Tween
			{
				target = target,
				duration = seconds,
				elapsed = 0f,
				tweenX = x.HasValue,
				tweenY = y.HasValue,
				fromX = target.x,
				fromY = target.y,
				toX = x ?? target.x,
				toY = y ?? target.y,
			};

			tweens.Add(t);
		}

		public static void stop(FlashObject target)
		{
			for (int i = tweens.Count - 1; i >= 0; i--)
			{
				if (tweens[i].target == target)
				{
					tweens.RemoveAt(i);
				}
			}
		}

		/// Dropped when a game is left, so a tween can't outlive the objects it points at.
		public static void reset()
		{
			tweens.Clear();
		}

		public static void update()
		{
			float dt = Time.unscaledDeltaTime;

			for (int i = tweens.Count - 1; i >= 0; i--)
			{
				Tween t = tweens[i];
				if (t.target == null)
				{
					tweens.RemoveAt(i);
					continue;
				}

				t.elapsed += dt;
				float progress = t.duration <= 0f ? 1f : Mathf.Clamp01(t.elapsed / t.duration);
				float eased = ExpoEaseOut(progress);

				if (t.tweenX)
				{
					t.target.x = Mathf.LerpUnclamped(t.fromX, t.toX, eased);
				}

				if (t.tweenY)
				{
					t.target.y = Mathf.LerpUnclamped(t.fromY, t.toY, eased);
				}

				if (progress >= 1f)
				{
					tweens.RemoveAt(i);
				}
			}
		}

		private static float ExpoEaseOut(float t)
		{
			return t >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
		}
	}
}
