namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The small hopping blob. Ported from MonsterGel.as.
	///
	/// Wanders like the goomba and jumps at random. At the first size Hans kills it by
	/// touching it at all; later it has to be jumped on.
	/// </summary>
	public class MonsterGel : Monster
	{
		private static string S_monstergel = "MonsterGel_S_monstergel";
		private static string Sfxdie = "MonsterGel_Sfxdie";
		private static string Sfxjump = "MonsterGel_Sfxjump";

		public MonsterGel(double _X, double _Y, FlxPoint _scale)
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
				randomNumber = FlxG.random();
				if (!isWalkin)
				{
					if (randomNumber < 0.01)
					{
						isWalkin = true;
					}
				}
				else if (randomNumber < 0.006)
				{
					isWalkin = false;
				}
				else if (randomNumber < 0.013 || velocity.x == 0)
				{
					isLeft = !isLeft;
				}

				randomNumber = FlxG.random();
				if (randomNumber < 0.006)
				{
					isJumpin = true;
				}
			}

			if (!isDead)
			{
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

				if (isJumpin && isTouching(FlxObject.FLOOR))
				{
					isJumpin = false;
					velocity.y = -maxVelocity.y / 2;
					FlxG.play(Sfxjump);
				}
			}
			else if (!isDeadOnce)
			{
				play("die");
				acceleration.x = 0;
				isDeadOnce = true;
				FlxG.play(Sfxdie);
			}

			base.update();
		}
	}
}
