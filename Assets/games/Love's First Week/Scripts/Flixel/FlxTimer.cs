using System;
using System.Collections.Generic;

namespace Games.LovesFirstWeek
{
	/// <summary>
	/// org.flixel.FlxTimer: calls back after Time seconds, Loops times (0 = forever).
	///
	/// Timers are not members of a state. Starting one registers it with a manager that
	/// FlxGame updates every step, before the state, and that is emptied when the state
	/// changes - so a timer never fires into a state that has gone.
	/// </summary>
	public class FlxTimer
	{
		public double time;
		public uint loops;
		public bool paused;
		public bool finished;

		protected Action<FlxTimer> _callback;
		protected double _timeCounter;
		protected uint _loopsCounter;

		public FlxTimer()
		{
			time = 0;
			loops = 0;
			_callback = null;
			_timeCounter = 0;
			_loopsCounter = 0;

			paused = false;
			finished = false;
		}

		public void destroy()
		{
			stop();
			_callback = null;
		}

		public void update()
		{
			_timeCounter += FlxG.elapsed;
			while (_timeCounter >= time && !paused && !finished)
			{
				_timeCounter -= time;

				_loopsCounter++;
				if (loops > 0 && _loopsCounter >= loops)
				{
					stop();
				}

				if (_callback != null)
				{
					_callback(this);
				}
			}
		}

		public FlxTimer start(double Time = 1, uint Loops = 1, Action<FlxTimer> Callback = null)
		{
			FlxTimerManager.add(this);

			if (paused)
			{
				paused = false;
				return this;
			}

			paused = false;
			finished = false;
			time = Time;
			loops = Loops;
			_callback = Callback;
			_timeCounter = 0;
			_loopsCounter = 0;
			return this;
		}

		public void stop()
		{
			finished = true;
			FlxTimerManager.remove(this);
		}

		public double timeLeft
		{
			get { return time - _timeCounter; }
		}

		public double progress
		{
			get { return time > 0 ? _timeCounter / time : 0; }
		}
	}

	/// org.flixel.plugin.TimerManager.
	public static class FlxTimerManager
	{
		private static readonly List<FlxTimer> timers = new List<FlxTimer>();

		/// Newest first, as the source iterates - and by index, since a callback may start
		/// or stop timers while this runs.
		public static void update()
		{
			int i = timers.Count - 1;
			while (i >= 0)
			{
				if (i < timers.Count)
				{
					FlxTimer timer = timers[i];
					if (timer != null && !timer.paused && !timer.finished && timer.time > 0)
					{
						timer.update();
					}
				}

				i--;
			}
		}

		/// The source pushes without checking, so a restarted timer is listed twice and
		/// runs twice as fast until it stops. Kept: it is how the game's timers behave.
		public static void add(FlxTimer Timer)
		{
			timers.Add(Timer);
		}

		public static void remove(FlxTimer Timer)
		{
			int index = timers.IndexOf(Timer);
			if (index >= 0)
			{
				timers.RemoveAt(index);
			}
		}

		public static void clear()
		{
			int i = timers.Count - 1;
			while (i >= 0)
			{
				if (i < timers.Count)
				{
					FlxTimer timer = timers[i];
					if (timer != null)
					{
						timer.destroy();
					}
				}

				i--;
			}

			timers.Clear();
		}
	}
}
