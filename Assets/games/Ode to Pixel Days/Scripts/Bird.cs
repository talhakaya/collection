namespace Games.OdeToPixelDays
{
	/// <summary>
	/// One bird of the flocks that cross the sky during the long fall. Ported from Bird.as.
	/// </summary>
	public class Bird : FlxSprite
	{
		private static string S_ = "Bird_S_";

		public Bird(double _x, double _y)
		{
			x = _x + (FlxG.random() - 0.5) * 100;
			y = _y + (FlxG.random() - 0.5) * 100;
			loadGraphic(S_, true, true, 6, 6, false);
			addAnimation("fly", new[] { 0, 1, 2 }, 6, true);
			addAnimation("fly2", new[] { 2, 0, 1 }, 7, true);
			addAnimation("fly3", new[] { 1, 2, 0 }, 5, true);
			double random = FlxG.random();
			if (random < 0.34)
			{
				play("fly");
			}
			else if (random < 0.67)
			{
				play("fly2");
			}
			else
			{
				play("fly3");
			}

			double velocityX = 200 + FlxG.random() * 60;
			double velocityY = (FlxG.random() - 0.5) * 60;
			if (x < 160)
			{
				velocity.x = velocityX;
				facing = LEFT;
			}
			else
			{
				velocity.x = -velocityX;
				facing = RIGHT;
			}

			velocity.y = velocityY;
		}

		public override void update()
		{
			if (x < -20 && facing == RIGHT)
			{
				kill();
			}
			else if (x > 340 && facing == LEFT)
			{
				kill();
			}
		}
	}
}
