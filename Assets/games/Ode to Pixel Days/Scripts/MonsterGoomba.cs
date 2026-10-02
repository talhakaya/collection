namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The walking mushroom-ish monster. Ported from MonsterGoomba.as.
	///
	/// It wanders: mostly standing, now and then deciding to walk, turn round or stop, and
	/// turning when it runs into something. With simpleWalk it just paces back and forth.
	/// Jumped on, it dies - until the world gets coarse enough that Hans only bounces off.
	/// </summary>
	public class MonsterGoomba : Monster
	{
		private static string S_monstergoomba = "MonsterGoomba_S_monstergoomba";
		private static string S_monstergoomba1 = "MonsterGoomba_S_monstergoomba1";
		private static string Sfxdie = "MonsterGoomba_Sfxdie";
		private static string Sfx = "MonsterGoomba_Sfx";

		public bool simpleWalk;

		public MonsterGoomba(double _X, double _Y, FlxPoint _scale)
		{
			x = _X;
			y = _Y;
			scale = _scale;
			if (scale.x == 1)
			{
				loadGraphic(S_monstergoomba1, true, true, 16, 16, false);
			}
			else
			{
				loadGraphic(S_monstergoomba, true, true, 16, 16, false);
			}

			maxVelocity.x = 20;
			maxVelocity.y = 155;
			acceleration.y = 200;
			drag.x = maxVelocity.x * 4;
			if (scale.x == 1)
			{
				width = 10;
				height = 12;
				offset.x = 3;
				offset.y = 4;
			}
			else if (scale.x == 2)
			{
				width = 20;
				height = 24;
				offset.x = -2;
				offset.y = 0;
			}
			else if (scale.x == 4)
			{
				width = 40;
				height = 48;
				offset.x = -8;
				offset.y = -8;
			}
			else if (scale.x == 8)
			{
				width = 128;
				height = 128;
				offset.x = 0;
				offset.y = 0;
			}
			else if (scale.x == 16)
			{
				width = 256;
				height = 256;
				offset.x = 0;
				offset.y = 0;
			}
			else if (scale.x == 32)
			{
				width = 512;
				height = 512;
				offset.x = 0;
				offset.y = 0;
			}

			addAnimation("idle", new[] { 0, 1, 0, 0 }, 4, true);
			addAnimation("walk", new[] { 2, 3, 10, 4, 5, 11 }, 4, true);
			addAnimation("die", new[] { 6, 7, 8, 9 }, 6, false);
			addAnimation("getHit", new[] { 6 }, 6, false);
			play("idle");
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

			simpleWalk = false;
		}

		public override void update()
		{
			if (!isDead)
			{
				if (!simpleWalk)
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
				}
				else
				{
					isWalkin = true;
					if (velocity.x == 0)
					{
						isLeft = !isLeft;
					}
				}

				if (FlxG.random() < 0.002)
				{
					FlxG.play(Sfx);
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
						facing = LEFT;
					}
					else
					{
						acceleration.x = maxVelocity.x * 4;
						facing = RIGHT;
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
				FlxG.play(Sfxdie);
			}

			base.update();
		}
	}
}
