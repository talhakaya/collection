namespace Games.OdeToPixelDays
{
	/// <summary>
	/// A drifting cloud, for the title screen and Level 41. Ported from Cloud2.as.
	/// </summary>
	public class Cloud2 : FlxSprite
	{
		private static string S_ = "Cloud2_S_";

		private double maxX;

		public Cloud2(double _y, int _levelwidth)
		{
			double random = double.NaN;
			loadGraphic(S_, false, true, 64, 32, false);
			if (FlxG.random() > 0.5)
			{
				facing = LEFT;
			}
			else
			{
				facing = RIGHT;
			}

			velocity.x = FlxG.random() * 20;
			x = -FlxG.random() * 100 - 100;
			y = _y;
			random = FlxG.random();
			scale = new FlxPoint(1 + random, 1 + random);
			alpha = 0.7;
			maxX = _levelwidth * 8 + 180;
		}

		public override void update()
		{
			base.update();
			if (x > maxX || x < -180)
			{
				kill();
			}
		}
	}
}
