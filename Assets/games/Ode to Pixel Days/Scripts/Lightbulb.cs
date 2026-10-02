namespace Games.OdeToPixelDays
{
	/// <summary>
	/// One of the four lights of the boss room's memory game. Ported from Lightbulb.as.
	///
	/// It lights for a second - when the pattern is being shown, or when the lever under it is
	/// pulled - and justOpened is up for the one step in which the lever lit it, which is
	/// how the PatternMatcher reads the player's answer.
	/// </summary>
	public class Lightbulb : FlxSprite
	{
		private static string S_ = "Lightbulb_S_";
		private static string Sfx1 = "Lightbulb_Sfx1";
		private static string Sfx2 = "Lightbulb_Sfx2";
		private static string Sfx3 = "Lightbulb_Sfx3";
		private static string Sfx4 = "Lightbulb_Sfx4";

		public Lever2 lever;
		public bool leverIsOpen;
		public bool isOpen;
		public bool justOpened;
		private FlxTimer _timer;
		private int clr;

		public Lightbulb(double _X, double _Y, FlxPoint _scale, Lever2 _lever, int _color)
		{
			x = _X;
			y = _Y;
			scale = _scale;
			lever = _lever;
			loadGraphic(S_, true, false, 48, 48, false);
			clr = _color;
			if (_color == 1)
			{
				addAnimation("0", new[] { 0 }, 1, false);
				addAnimation("1", new[] { 1 }, 1, false);
			}
			else if (_color == 2)
			{
				addAnimation("0", new[] { 2 }, 1, false);
				addAnimation("1", new[] { 3 }, 1, false);
			}
			else if (_color == 3)
			{
				addAnimation("0", new[] { 4 }, 1, false);
				addAnimation("1", new[] { 5 }, 1, false);
			}
			else
			{
				addAnimation("0", new[] { 6 }, 1, false);
				addAnimation("1", new[] { 7 }, 1, false);
			}

			play("0");
			leverIsOpen = lever.isOpen;
			isOpen = false;
			justOpened = false;
			_timer = new FlxTimer();
		}

		public override void update()
		{
			base.update();
			if (justOpened)
			{
				justOpened = false;
			}

			if (leverIsOpen != lever.isOpen && !isOpen)
			{
				light();
				justOpened = true;
			}

			leverIsOpen = lever.isOpen;
		}

		public void light()
		{
			isOpen = true;
			play("1");
			_timer.start(1, 1, close);
			if (clr == 1)
			{
				FlxG.play(Sfx1);
			}
			else if (clr == 2)
			{
				FlxG.play(Sfx2);
			}
			else if (clr == 3)
			{
				FlxG.play(Sfx3);
			}
			else if (clr == 4)
			{
				FlxG.play(Sfx4);
			}
		}

		private void close(FlxTimer _t)
		{
			isOpen = false;
			play("0");
		}
	}
}
