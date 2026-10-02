namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The game's own timer, ported from TTimer.as. It counts game steps, not seconds, and
	/// instead of calling back it raises complete for exactly one step, which the owner
	/// polls: if (timer8.complete) nextLevel();
	///
	/// It only counts while it is a member of the state, since that is what updates it -
	/// Level adds the player's three timers a second after the level starts.
	/// </summary>
	public class TTimer : FlxBasic
	{
		public int count;
		private int upTo;
		public bool started;
		public bool complete;

		public TTimer()
		{
			reset();
		}

		public void start(int _countUpTo)
		{
			if (!started)
			{
				reset();
				started = true;
				upTo = _countUpTo;
			}
		}

		public void reset()
		{
			count = 0;
			started = false;
			complete = false;
			upTo = 0;
		}

		public override void update()
		{
			base.update();
			if (started)
			{
				if (count < upTo)
				{
					++count;
				}
				else
				{
					reset();
					complete = true;
				}
			}

			if (complete)
			{
				if (count == 0)
				{
					count = -1;
				}
				else if (count == -1)
				{
					reset();
				}
			}
		}
	}
}
