namespace Games.OdeToPixelDays
{
	/// <summary>
	/// A cloud: rushing up past Hans during the long fall, or drifting grey over the last
	/// scene. Ported from Cloud.as.
	/// </summary>
	public class Cloud : FlxSprite
	{
		private static string S_ = "Cloud_S_";

		private Hans player;
		private bool grey;

		public Cloud(Hans _player, bool _grey)
		{
			double random = double.NaN;
			loadGraphic(S_, false, true, 64, 32, false);
			player = _player;
			if (FlxG.random() > 0.5)
			{
				facing = LEFT;
			}
			else
			{
				facing = RIGHT;
			}

			grey = _grey;
			if (_grey)
			{
				velocity.x = FlxG.random() * 20;
				y = 30 + (FlxG.random() - 0.5) * 60;
				x = -FlxG.random() * 100 - 100;
				random = FlxG.random();
				scale = new FlxPoint(1 + random, 1 + random);
			}
			else
			{
				y = player.y + 320;
				x = FlxG.random() * 300;
				random = FlxG.random() * 3;
				scale = new FlxPoint(1 + random, 1 + random);
			}

			alpha = 0.5;
		}

		public override void update()
		{
			base.update();
			if (!grey && player.y > y + 320 && y < 9000)
			{
				kill();
			}

			if (grey && x > 3500 || x < -180)
			{
				kill();
			}
		}
	}
}
