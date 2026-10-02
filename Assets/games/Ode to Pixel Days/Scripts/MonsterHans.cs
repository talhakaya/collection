namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The other Hans: the chaser of levels 36 to 38 and the boss of 39. Ported from
	/// MonsterHans.as.
	///
	/// Each level gets its own behaviour, chosen by the level number it is given - where it
	/// starts chasing, how fast, and the stretches of floor where it has to jump. As the boss
	/// it pulses between solid and see-through, fainter with every hit it has taken.
	/// </summary>
	public class MonsterHans : Monster
	{
		private static string S_ = "MonsterHans_S_";
		private static string Sfxfloor = "MonsterHans_Sfxfloor";
		private static string Sfxjump = "MonsterHans_Sfxjump";
		private static string Sfxwalk = "MonsterHans_Sfxwalk";
		private static string Sfxhurt = "MonsterHans_Sfxhurt";

		private int whichlevel;
		private Hans hans;
		public bool fading;
		public int hp;
		private double alpha2fade;
		private bool fadingOut;
		private bool istouchingfloor;
		private int count;

		public MonsterHans(double _X, double _Y, FlxPoint _scale, int _level, Hans _player)
		{
			x = _X;
			y = _Y;
			scale = _scale;
			whichlevel = _level;
			hans = _player;
			loadGraphic(S_, true, true, 16, 32, false);
			addAnimation("stand", new[] { 0 }, 12, true);
			addAnimation("run", new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 12, true);
			addAnimation("jump", new[] { 9 }, 12, true);
			addAnimation("fall", new[] { 10, 11 }, 6, true);
			if (whichlevel == 36)
			{
				maxVelocity.x = 110;
			}
			else if (whichlevel == 37)
			{
				maxVelocity.x = 140;
			}
			else if (whichlevel == 38)
			{
				maxVelocity.x = 70;
			}
			else if (whichlevel == 39)
			{
				maxVelocity.x = 100;
			}

			maxVelocity.y = 400;
			acceleration.y = 400;
			drag.x = maxVelocity.x * 4;
			width = 24;
			height = 64;
			centerOffsets();
			isDead = false;
			isDeadOnce = false;
			istouchingfloor = false;
			isWalkin = false;
			if (FlxG.random() > 0.5)
			{
				isLeft = true;
			}
			else
			{
				isLeft = false;
			}

			if (whichlevel == 39)
			{
				hp = 6;
				fadingOut = true;
			}

			count = 0;
		}

		public override void update()
		{
			if (!isDead)
			{
				if (whichlevel == 36)
				{
					if (hans.x > 450)
					{
						if (hans.x > x + 32)
						{
							isWalkin = true;
							isLeft = false;
						}
						else if (x > hans.x + 32)
						{
							isWalkin = true;
							isLeft = true;
						}
						else
						{
							isWalkin = false;
							if (hans.x > x)
							{
								isLeft = false;
							}
							else
							{
								isLeft = true;
							}
						}

						if (x > 400 && x < 410 || x > 560 && x < 570)
						{
							isJumpin = true;
						}
					}
					else if (x < 440)
					{
						isWalkin = false;
					}
				}
				else if (whichlevel == 37)
				{
					if (hans.x > x + 12)
					{
						isWalkin = true;
						isLeft = false;
					}
					else if (x > hans.x + 12)
					{
						isWalkin = true;
						isLeft = true;
					}
					else
					{
						isWalkin = false;
						if (hans.x > x)
						{
							isLeft = false;
						}
						else
						{
							isLeft = true;
						}
					}

					if ((x > 200 && x < 210 || x > 304 && x < 312 || x > 392 && x < 416 || x > 488 && x < 502 || x > 600 && x < 624 || x > 720 && x < 744) && (hans.x < x + 64 && x < hans.x + 64))
					{
						isJumpin = true;
					}
				}
				else if (whichlevel == 38)
				{
					isWalkin = true;
					isLeft = false;
				}
				else if (whichlevel == 39)
				{
					if (hans.x > 776)
					{
						if (isWalkin && (velocity.x == 0 || hans.y < y - 32))
						{
							if (hp > 2)
							{
								isJumpin = true;
							}
						}

						if (y >= hans.y - 64)
						{
							if (x > hans.x + 4)
							{
								isWalkin = true;
								isLeft = true;
							}
							else if (x < hans.x - 4)
							{
								isWalkin = true;
								isLeft = false;
							}
							else
							{
								isWalkin = false;
							}
						}
						else
						{
							isWalkin = true;
							if (velocity.x == 0)
							{
								if (x > hans.x)
								{
									isLeft = true;
								}
								else
								{
									isLeft = false;
								}
							}
						}
					}

					if (hp == 6)
					{
						alpha2fade = 1;
					}
					else if (hp == 5)
					{
						alpha2fade = 0.8;
					}
					else if (hp == 4)
					{
						alpha2fade = 0.6;
					}
					else if (hp == 3)
					{
						alpha2fade = 0.4;
					}
					else if (hp == 2)
					{
						alpha2fade = 0.2;
					}
					else if (hp == 1)
					{
						alpha2fade = 0;
					}

					if (fadingOut)
					{
						if (alpha <= alpha2fade)
						{
							fadingOut = false;
							alpha = alpha2fade;
						}
						else
						{
							alpha -= (1 - alpha2fade) / 20;
						}
					}
					else if (alpha >= 1)
					{
						fadingOut = true;
						alpha = 1;
					}
					else
					{
						alpha += (1 - alpha2fade) / 20;
					}
				}

				if (isTouching(FlxObject.FLOOR) && isWalkin && velocity.x != 0)
				{
					if (count == 10)
					{
						FlxG.play(Sfxwalk);
					}

					++count;
					if (count == 20)
					{
						count = 0;
					}
				}
				else
				{
					count = 0;
				}
			}

			if (fading && alpha > 0)
			{
				alpha -= 0.02;
			}

			if (!isDead)
			{
				if (isWalkin)
				{
					play("run");
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
					play("stand");
				}

				if (isLeft)
				{
					facing = FlxObject.LEFT;
				}
				else
				{
					facing = FlxObject.RIGHT;
				}

				if (isJumpin && isTouching(FlxObject.FLOOR))
				{
					isJumpin = false;
					velocity.y = -maxVelocity.y / 2;
					FlxG.play(Sfxjump);
				}

				if (velocity.y > 0)
				{
					play("fall");
				}
				else if (velocity.y < 0)
				{
					play("jump");
				}
			}
			else if (!isDeadOnce)
			{
				acceleration.x = 0;
				isDeadOnce = true;
			}

			if (!istouchingfloor && isTouching(FlxObject.FLOOR))
			{
				FlxG.play(Sfxfloor);
			}

			istouchingfloor = isTouching(FlxObject.FLOOR);
			base.update();
		}
	}
}
