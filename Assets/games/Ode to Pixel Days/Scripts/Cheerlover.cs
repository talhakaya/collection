namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The cheerleader once she has shrunk to a single pixel column and tags along.
	/// Ported from Cheerlover.as.
	///
	/// Attached to Hans she bounces around him, jumping; "gone crazy" she drags behind, and
	/// near the end of that level she lets go, runs ahead and fades out.
	/// </summary>
	public class Cheerlover : FlxSprite
	{
		private static string S_cheerleader2 = "Cheerlover_S_cheerleader2";
		private static string Sfxjump = "Cheerlover_Sfxjump";

		public bool isWalkin;
		public bool isJumpin;
		public bool isLeft;
		public bool fading;
		private double randomNumber;
		private double count;
		private double count2;
		public int countForSfxwalk;
		public int whichLevel;
		private Hans player;
		public bool firstStateChange;
		public bool isAttachedToHans;
		public bool isGoneCrazy;
		public bool isOnLeftOfHans;
		public bool isPlayerWalkinRight;

		public Cheerlover(int _X, int _Y, FlxPoint _scale, int _level, Hans _player, bool _attached, bool _crazy)
		{
			x = _X;
			y = _Y;
			scale = _scale;
			whichLevel = _level;
			player = _player;
			isAttachedToHans = _attached;
			isGoneCrazy = _crazy;
			fading = false;
			loadGraphic(S_cheerleader2, true, true, 1, 2, false);
			width = 16;
			height = 32;
			addAnimation("idle", new[] { 0, 1, 2, 3, 2, 1, 0 }, 6, true);
			play("idle");
			centerOffsets();
			maxVelocity.x = 170;
			maxVelocity.y = 200;
			drag.x = maxVelocity.x * 4;
			acceleration.y = 400;
			isWalkin = false;
			isJumpin = false;
			isLeft = false;
			play("idle");
			count = 0;
			count2 = 0;
			isOnLeftOfHans = false;
			if (isAttachedToHans && isGoneCrazy)
			{
				maxVelocity.x = 100;
			}
		}

		public override void update()
		{
			randomNumber = FlxG.random();
			if (count < 21)
			{
				++count;
			}
			else
			{
				count = 0;
			}

			if (count2 < 41)
			{
				++count2;
			}
			else
			{
				count2 = 0;
			}

			if (!isAttachedToHans && !isGoneCrazy && firstStateChange)
			{
				firstStateChange = false;
				isAttachedToHans = true;
			}

			if (!isAttachedToHans && !isGoneCrazy)
			{
				isWalkin = false;
			}
			else if (isAttachedToHans && !isGoneCrazy)
			{
				isWalkin = true;
				if (!isOnLeftOfHans)
				{
					if (count == 0)
					{
						isLeft = true;
					}
				}
				else if (count == 0)
				{
					isLeft = false;
				}

				if (count2 == 0)
				{
					isJumpin = true;
				}
			}
			else if (isAttachedToHans && isGoneCrazy)
			{
				if (!isOnLeftOfHans)
				{
					if (count == 0)
					{
						isWalkin = false;
					}
				}
				else if (count == 0)
				{
					isWalkin = true;
					isLeft = false;
				}
			}
			else if (!isAttachedToHans && isGoneCrazy)
			{
				maxVelocity.x = 170;
				isWalkin = true;
				isLeft = false;
				if (x > 2496)
				{
					isWalkin = false;
					fading = true;
					x = 2496;
				}
			}

			if (x > 2200)
			{
				isAttachedToHans = false;
			}

			if (isWalkin)
			{
				if (isLeft)
				{
					if (isPlayerWalkinRight)
					{
						acceleration.x = 0;
					}
					else
					{
						acceleration.x = -maxVelocity.x * 4;
					}
				}
				else
				{
					acceleration.x = maxVelocity.x * 4;
				}
			}
			else
			{
				acceleration.x = 0;
			}

			if (isJumpin && isTouching(FLOOR))
			{
				isJumpin = false;
				velocity.y = -maxVelocity.y * 0.7;
				FlxG.play(Sfxjump);
			}

			if (fading && alpha > 0)
			{
				alpha -= 0.025;
			}
			else if (alpha <= 0)
			{
				kill();
			}

			base.update();
		}
	}
}
