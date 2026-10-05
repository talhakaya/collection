namespace Games.OdeToPixelDays
{
	/// <summary>
	/// An invisible strip the height of the screen. Ported from NarratorTouch.as. Walking
	/// into it reveals its narrator - or, as a lineIncrementer, moves that narrator on to
	/// its next line. Either way only once.
	/// </summary>
	public class NarratorTouch : FlxSprite
	{
		private bool touched;
		private Narrator narrator;
		public bool lineIncrementer;

		public NarratorTouch(double _X, double _Y, Narrator _narrator)
		{
			x = _X;
			y = _Y;
			narrator = _narrator;
			width = 32;
			height = 240;
			touched = false;
			alpha = 0;
			lineIncrementer = false;
		}

		public override void kill()
		{
			if (!touched)
			{
				touched = true;
				if (lineIncrementer)
				{
					++narrator.line;
				}
				else
				{
					narrator.kill();
				}
			}
		}
	}

	/// The "press space" prompt of the first level. Ported from TutoSpace.as.
	///
	/// In the collection: the space bar that was drawn in the picture, going down and up,
	/// is the collection's glyph for the SPACE action now (see FlxPrompt). The three frames
	/// are all the bare ellipse.
	public class TutoSpace : FlxPrompt
	{
		private static string S_tutoSpace = "TutoSpace_S_tutoSpace";

		public TutoSpace(double _x, double _y) : base(_x, _y, null, "{SPACE}", 14)
		{
			loadGraphic(S_tutoSpace, true, false, 48, 24, false);
			addAnimation("0", new[] { 0, 1, 2 }, 1, true);
			play("0");
		}
	}

	/// A speck of dust thrown up when a block lands. Ported from ParticleBlack.as.
	public class ParticleBlack : FlxSprite
	{
		private int count;

		public ParticleBlack(double _X, double _Y)
		{
			x = _X + 16 * (FlxG.random() - 0.5);
			y = _Y;
			acceleration.y = 100;
			makeGraphic(4, 4, 4278190080);
			velocity.x = 50 * (FlxG.random() - 0.5);
			velocity.y = -50;
			count = 0;
		}

		public override void update()
		{
			base.update();
			++count;
			if (count >= 60)
			{
				kill();
			}
		}
	}
}
