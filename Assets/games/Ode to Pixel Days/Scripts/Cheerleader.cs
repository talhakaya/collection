namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The cheerleader Hans is chasing. Ported from Cheerleader.as.
	///
	/// She has no general behaviour - each level she appears in has its own little script,
	/// picked by whichLevel: wait until Hans gets close, walk off to a fixed x, fade out.
	/// </summary>
	public class Cheerleader : FlxSprite
	{
		private static string S_cheerleader = "Cheerleader_S_cheerleader";
		private static string S_cheerleader16 = "Cheerleader_S_cheerleader16";
		private static string S_cheerleader8 = "Cheerleader_S_cheerleader8";
		private static string S_cheerleader4 = "Cheerleader_S_cheerleader4";
		private static string S_cheerleader2 = "Cheerleader_S_cheerleader2";
		private static string Sfxwalk1 = "Cheerleader_Sfxwalk1";
		private static string Sfxwalk2 = "Cheerleader_Sfxwalk2";
		private static string Sfxwalk3 = "Cheerleader_Sfxwalk3";
		private static string Sfxwalk4 = "Cheerleader_Sfxwalk4";
		private static string Sfxwalk5 = "Cheerleader_Sfxwalk5";

		public bool isWalkin;
		public bool isLeft;
		public double randomNumber;
		public uint whichLevel;
		private Hans player;
		public bool gone;
		public int count;
		public int countForSfxwalk;
		private bool sonBolumBakti;

		public Cheerleader(uint _X, uint _Y, FlxPoint _scale, uint _level, Hans _player)
		{
			x = _X;
			y = _Y;
			scale = _scale;
			whichLevel = _level;
			player = _player;
			acceleration.y = 200;
			if (scale.x == 1)
			{
				loadGraphic(S_cheerleader, true, true, 20, 32, false);
				width = 8;
				height = 30;
				offset.x = 6;
				offset.y = 2;
				addAnimation("idle", new[] { 0 }, 12, true);
				addAnimation("walk", new[] { 8, 7, 6, 5, 4, 3, 2, 1 }, 8, true);
				addAnimation("talk", new[] { 10, 11, 10, 11, 11, 10, 0, 0, 0, 0, 0, 0, 0 }, 4, true);
				addAnimation("stare", new[] { 9, 9, 9, 9, 9, 9, 10, 11, 10, 10, 11, 11, 10, 11, 10, 11, 11, 10, 0, 0, 0, 0, 0, 0, 0 }, 4, false);
			}
			else if (scale.x == 2)
			{
				loadGraphic(S_cheerleader16, true, true, 10, 16, false);
				width = 8;
				height = 30;
				offset.x = 6;
				offset.y = -6;
				addAnimation("idle", new[] { 0 }, 12, true);
				addAnimation("walk", new[] { 1, 2, 3, 4, 5, 6 }, 6, true);
			}
			else if (scale.x == 4)
			{
				loadGraphic(S_cheerleader8, true, true, 6, 8, false);
				width = 8;
				height = 32;
				addAnimation("idle", new[] { 0 }, 12, true);
				addAnimation("walk", new[] { 1, 3, 2, 3 }, 4, true);
			}
			else if (scale.x == 8)
			{
				loadGraphic(S_cheerleader4, true, true, 4, 4, false);
				width = 16;
				height = 32;
				addAnimation("idle", new[] { 0 }, 12, true);
				addAnimation("walk", new[] { 1, 2 }, 2, true);
			}
			else if (scale.x == 16)
			{
				loadGraphic(S_cheerleader2, true, true, 1, 2, false);
				width = 16;
				height = 32;
				addAnimation("idle", new[] { 0, 1, 2, 3, 2, 1, 0 }, 12, true);
				addAnimation("walk", new[] { 0, 1, 2, 3, 2, 1, 0 }, 12, true);
			}

			if (scale.x > 3)
			{
				centerOffsets();
			}

			if (whichLevel == 1)
			{
				maxVelocity.x = 48;
			}
			else if (whichLevel == 7)
			{
				maxVelocity.x = 60;
			}
			else if (whichLevel == 11)
			{
				maxVelocity.x = 100;
			}
			else if (whichLevel == 15 || whichLevel == 24 || whichLevel == 34)
			{
				maxVelocity.x = 130;
			}
			else if (whichLevel == 23)
			{
				maxVelocity.x = 80;
			}
			else if (whichLevel == 30 || whichLevel == 31)
			{
				maxVelocity.x = 120;
			}

			drag.x = maxVelocity.x * 4;
			isWalkin = false;
			isLeft = false;
			if (whichLevel == 43)
			{
				play("talk");
			}
			else
			{
				play("idle");
			}

			gone = false;
			facing = LEFT;
			count = 0;
			countForSfxwalk = 0;
		}

		public override void update()
		{
			if ((whichLevel == 1 || whichLevel == 7) && !gone)
			{
				if (x > 284)
				{
					isWalkin = false;
					gone = true;
				}
				else if (player.x > x - 140)
				{
					isWalkin = true;
					isLeft = false;
				}
			}
			else if (whichLevel == 11 && !gone)
			{
				if (x > 424)
				{
					isWalkin = false;
					gone = true;
				}
				else if (player.x > x - 100)
				{
					isWalkin = true;
					isLeft = false;
				}
			}
			else if (whichLevel == 15 && !gone)
			{
				if (x > 560)
				{
					isWalkin = false;
					gone = true;
				}
				else if (player.x > x - 64)
				{
					isWalkin = true;
					isLeft = false;
				}
			}
			else if (whichLevel == 23 && !gone)
			{
				if (x < 220)
				{
					isWalkin = false;
				}
				else if (player.y > 475)
				{
					isWalkin = true;
					isLeft = true;
				}
			}
			else if (whichLevel == 24 && !gone)
			{
				if (x > 560)
				{
					isWalkin = false;
					gone = true;
				}
				else if (player.x > x - 120)
				{
					isWalkin = true;
					isLeft = false;
				}
			}
			else if (whichLevel == 30 && !gone)
			{
				if (x > 200)
				{
					isLeft = true;
					isWalkin = false;
					++count;
				}
				else if (player.x > 63)
				{
					isWalkin = true;
					isLeft = false;
				}
			}
			else if (whichLevel == 31 && !gone)
			{
				if (x > 576)
				{
					isWalkin = false;
					gone = true;
				}
				else if (player.x > x - 64)
				{
					isWalkin = true;
					isLeft = false;
				}
			}
			else if (whichLevel == 34 && !gone)
			{
				if (x > 480)
				{
					isWalkin = false;
				}
				else if (player.x > x - 130)
				{
					isWalkin = true;
					isLeft = false;
				}
			}
			else if (whichLevel == 43)
			{
				isWalkin = false;
				isLeft = false;
				facing = RIGHT;
				if (!sonBolumBakti && player.x > x - 28)
				{
					play("stare");
					sonBolumBakti = true;
					new FlxTimer().start(2, 1, talkAgain);
				}
				else if (!sonBolumBakti)
				{
					play("talk");
				}
			}

			if (gone && alpha > 0)
			{
				alpha -= 0.025;
			}
			else if (alpha == 0)
			{
				kill();
			}

			if (whichLevel != 43)
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

			if (isTouching(FlxObject.FLOOR) && isWalkin)
			{
				if (countForSfxwalk == 0)
				{
					if (scale.x == 1)
					{
						FlxG.play(Sfxwalk1);
					}
					else if (scale.x == 2)
					{
						FlxG.play(Sfxwalk2);
					}
					else if (scale.x == 4)
					{
						FlxG.play(Sfxwalk3);
					}
					else if (scale.x == 8)
					{
						FlxG.play(Sfxwalk4);
					}
					else if (scale.x == 16)
					{
						FlxG.play(Sfxwalk5);
					}

					countForSfxwalk = 0;
				}

				++countForSfxwalk;
				if (countForSfxwalk == 30)
				{
					countForSfxwalk = 0;
				}
			}
			else
			{
				countForSfxwalk = 0;
			}

			base.update();
		}

		private void talkAgain(FlxTimer a)
		{
			play("talk");
		}
	}
}
