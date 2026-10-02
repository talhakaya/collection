namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The blue blob that rains down in Level 3's second half. Ported from MonsterGelBlue.as.
	///
	/// It falls, kills Hans outright if it lands on him, and a second after reaching the
	/// floor it is gone.
	/// </summary>
	public class MonsterGelBlue : Monster
	{
		private static string S_monstergel = "MonsterGelBlue_S_monstergel";

		private bool hasJumped;
		private FlxTimer timer;

		public MonsterGelBlue(double _X, double _Y, FlxPoint _scale)
		{
			x = _X;
			y = _Y;
			scale = _scale;
			loadGraphic(S_monstergel, true, true, 8, 8, false);
			maxVelocity.x = 30;
			maxVelocity.y = 200;
			acceleration.y = 200;
			drag.x = maxVelocity.x * 4;
			if (scale.x == 1)
			{
				width = 6;
				height = 5;
				offset.x = 1;
				offset.y = 3;
			}
			else if (scale.x == 2)
			{
				width = 14;
				height = 12;
				offset.x = -3;
				offset.y = 0;
			}
			else if (scale.x == 4)
			{
				width = 28;
				height = 24;
				offset.x = -10;
				offset.y = -4;
			}
			else if (scale.x == 8)
			{
				width = 64;
				height = 48;
				offset.x = -28;
				offset.y = -12;
			}

			addAnimation("idle", new[] { 0 }, 2, true);
			addAnimation("walk", new[] { 0, 1 }, 3, true);
			addAnimation("die", new[] { 2, 3, 4, 5 }, 6, false);
			isDead = false;
			isDeadOnce = false;
			isWalkin = false;
			hasJumped = false;
			if (FlxG.random() > 0.5)
			{
				isLeft = true;
			}
			else
			{
				isLeft = false;
			}
		}

		public override void update()
		{
			if (!isDead)
			{
				if (isTouching(FlxObject.FLOOR))
				{
					isDead = true;
					timer = new FlxTimer();
					timer.start(1, 1, die);
					velocity.x = 0;
				}

				if (isWalkin)
				{
					play("walk");
					if (isLeft)
					{
						acceleration.x = -maxVelocity.x * 4;
					}
					else
					{
						acceleration.x = maxVelocity.x * 4;
					}
				}
				else
				{
					acceleration.x = 0;
					play("idle");
				}
			}
			else if (!isDeadOnce)
			{
				play("die");
				acceleration.x = 0;
				isDeadOnce = true;
			}

			base.update();
		}

		private void die(FlxTimer e)
		{
			e.destroy();
			kill();
		}
	}
}
