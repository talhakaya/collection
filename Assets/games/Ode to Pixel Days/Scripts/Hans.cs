namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The player. Ported from Hans.as.
	///
	/// The level's scale is the game's whole idea: as Hans shrinks the world, everything is
	/// drawn from a smaller sheet scaled up - 16x32 at scale 1, down to a single pixel at
	/// 32 - and each size has its own animations, collision box, top speed and sounds.
	///
	/// Movement is Flixel's: acceleration towards four times the top speed, the same again
	/// as drag, and a jump that is one impulse plus a small extra push for each of the next
	/// twelve steps the key stays held.
	/// </summary>
	public class Hans : FlxSprite
	{
		private static string S_hans = "Hans_S_hans";
		private static string S_hans16 = "Hans_S_hans16";
		private static string S_hans8 = "Hans_S_hans8";
		private static string S_hans4 = "Hans_S_hans4";
		private static string S_hans2 = "Hans_S_hans2";
		private static string S_hans1 = "Hans_S_hans1";
		private static string S_dusme = "Hans_S_dusme";
		private static string Sfxfloor1 = "Hans_Sfxfloor1";
		private static string Sfxfloor2 = "Hans_Sfxfloor2";
		private static string Sfxfloor3 = "Hans_Sfxfloor3";
		private static string Sfxfloor4 = "Hans_Sfxfloor4";
		private static string Sfxfloor5 = "Hans_Sfxfloor5";
		private static string Sfxhurt1 = "Hans_Sfxhurt1";
		private static string Sfxhurt2 = "Hans_Sfxhurt2";
		private static string Sfxhurt3 = "Hans_Sfxhurt3";
		private static string Sfxhurt4 = "Hans_Sfxhurt4";
		private static string Sfxhurt5 = "Hans_Sfxhurt5";
		private static string Sfxjump1 = "Hans_Sfxjump1";
		private static string Sfxjump2 = "Hans_Sfxjump2";
		private static string Sfxjump3 = "Hans_Sfxjump3";
		private static string Sfxjump4 = "Hans_Sfxjump4";
		private static string Sfxjump5 = "Hans_Sfxjump5";
		private static string Sfxwalk1 = "Hans_Sfxwalk1";
		private static string Sfxwalk2 = "Hans_Sfxwalk2";
		private static string Sfxwalk3 = "Hans_Sfxwalk3";
		private static string Sfxwalk4 = "Hans_Sfxwalk4";
		private static string Sfxwalk5 = "Hans_Sfxwalk5";

		public bool interact;
		private uint count;
		private int countFlash;
		public bool fading;
		public bool fadeIn;
		public double hp;
		private bool hpDecreased;
		public TTimer timer;
		public TTimer timer2;
		public TTimer timer3;
		public int jumpThrottle;
		private int jumpThrottleMax;
		public bool moveEnable;
		public bool jumpEnable;
		public bool lastLevel;
		private FlxPoint initialScale;
		private bool born;
		public bool walkingSound;
		public bool smaller;
		public bool dontAnimate;
		private bool floortouch;
		private bool playfloor;
		private bool playjump;
		private int countForJump;

		public Hans(double _X, double _Y, FlxPoint _scale)
		{
			dontAnimate = false;
			playfloor = false;
			playjump = false;
			walkingSound = true;
			born = true;
			x = _X;
			y = _Y;
			scale = _scale;
			initialScale = _scale;
			if (scale.x == 1)
			{
				loadGraphic(S_hans, true, true, 16, 32, false);
				addAnimation("stand", new[] { 0 }, 12, true);
				addAnimation("run", new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 12, true);
				addAnimation("jump", new[] { 9 }, 12, true);
				addAnimation("fall", new[] { 10, 11 }, 6, true);
				maxVelocity.x = 100;
			}
			else if (scale.x == 2)
			{
				loadGraphic(S_hans16, true, true, 10, 16, false);
				addAnimation("stand", new[] { 0 }, 12, true);
				addAnimation("run", new[] { 1, 2, 3, 4 }, 6, true);
				addAnimation("jump", new[] { 6 }, 12, true);
				addAnimation("fall", new[] { 5 }, 6, true);
				maxVelocity.x = 110;
			}
			else if (scale.x == 4)
			{
				loadGraphic(S_hans8, true, true, 5, 8, false);
				addAnimation("stand", new[] { 0 }, 12, true);
				addAnimation("run", new[] { 1, 2, 3, 2 }, 6, true);
				addAnimation("jump", new[] { 4 }, 12, true);
				addAnimation("fall", new[] { 5 }, 6, true);
				maxVelocity.x = 120;
			}
			else if (scale.x == 8)
			{
				loadGraphic(S_hans4, true, true, 4, 4, false);
				addAnimation("stand", new[] { 0 }, 12, true);
				addAnimation("run", new[] { 0 }, 4, true);
				addAnimation("jump", new[] { 1 }, 12, true);
				addAnimation("fall", new[] { 0 }, 6, true);
				maxVelocity.x = 130;
			}
			else if (scale.x == 16)
			{
				loadGraphic(S_hans2, true, true, 1, 2, false);
				addAnimation("stand", new[] { 0, 1, 2, 3, 2, 1, 0 }, 6, true);
				addAnimation("run", new[] { 0, 1, 2, 3, 2, 1, 0 }, 6, true);
				addAnimation("jump", new[] { 0, 1, 2, 3, 2, 1, 0 }, 6, true);
				addAnimation("fall", new[] { 0, 1, 2, 3, 2, 1, 0 }, 6, true);
				maxVelocity.x = 140;
			}
			else if (scale.x == 32)
			{
				loadGraphic(S_hans1, true, true, 1, 1, false);
				addAnimation("stand", new[] { 0, 1, 2, 3, 2, 1, 0 }, 6, true);
				addAnimation("run", new[] { 0, 1, 2, 3, 2, 1, 0 }, 6, true);
				addAnimation("jump", new[] { 0, 1, 2, 3, 2, 1, 0 }, 6, true);
				addAnimation("fall", new[] { 0, 1, 2, 3, 2, 1, 0 }, 6, true);
				maxVelocity.x = 150;
			}

			maxVelocity.y = 305;
			acceleration.y = 600;
			jumpThrottleMax = 12;
			jumpThrottle = 0;
			drag.x = maxVelocity.x * 4;
			if (scale.x == 1)
			{
				width = 8;
				height = 28;
				offset.x = 4;
				offset.y = 4;
			}
			else if (scale.x == 2)
			{
				width = 12;
				height = 32;
				offset.x = 0;
				offset.y = -8;
			}
			else if (scale.x == 4)
			{
				width = 12;
				height = 32;
				offset.x = -4;
				offset.y = -12;
			}
			else if (scale.x == 8)
			{
				width = 16;
				height = 32;
				offset.x = -6;
				offset.y = -14;
			}
			else if (scale.x == 16)
			{
				width = 16;
				height = 32;
				centerOffsets();
			}
			else if (scale.x == 32)
			{
				width = 32;
				height = 32;
				offset.x = -17;
				offset.y = -15;
			}

			interact = false;
			fading = false;
			fadeIn = false;
			hp = 2;
			hpDecreased = false;
			moveEnable = true;
			jumpEnable = true;
			lastLevel = false;
			timer = new TTimer();
			timer2 = new TTimer();
			timer3 = new TTimer();
			floortouch = true;
			countForJump = 10;
		}

		public override void update()
		{
			base.update();
			if (playfloor && walkingSound && floortouch && isTouching(FlxObject.FLOOR))
			{
				playfloor = false;
				if (scale.x == 1)
				{
					FlxG.play(Sfxfloor1);
				}
				else if (scale.x == 2)
				{
					FlxG.play(Sfxfloor2);
				}
				else if (scale.x == 4)
				{
					FlxG.play(Sfxfloor3);
				}
				else if (scale.x == 8)
				{
					FlxG.play(Sfxfloor4);
				}
				else if (scale.x == 16)
				{
					FlxG.play(Sfxfloor5);
				}
			}

			if (countForJump < 10)
			{
				++countForJump;
			}

			if (walkingSound && playjump && velocity.y < 0 && countForJump == 10)
			{
				playjump = false;
				countForJump = 0;
				if (scale.x == 1)
				{
					FlxG.play(Sfxjump1);
				}
				else if (scale.x == 2)
				{
					FlxG.play(Sfxjump2);
				}
				else if (scale.x == 4)
				{
					FlxG.play(Sfxjump3);
				}
				else if (scale.x == 8)
				{
					FlxG.play(Sfxjump4);
				}
				else if (scale.x == 16)
				{
					FlxG.play(Sfxjump5);
				}
			}

			if (walkingSound && !floortouch && isTouching(FlxObject.FLOOR))
			{
				playfloor = true;
			}

			floortouch = isTouching(FlxObject.FLOOR);
			if (walkingSound && !playfloor && !playjump)
			{
				if (isTouching(FlxObject.FLOOR) && (FlxG.keys.LEFT || FlxG.keys.A || FlxG.keys.RIGHT || FlxG.keys.D) && velocity.x != 0)
				{
					if (count == 10)
					{
						if (scale.x == 1)
						{
							FlxG.play(Sfxwalk1);
						}
					}

					if (count == 0)
					{
						if (scale.x == 2)
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

			if (lastLevel)
			{
				if (born && scale.x > 1)
				{
					maxVelocity.x = 100;
				}

				if (scale.x > 1)
				{
					scale.x -= (initialScale.x - 1) * 0.05;
					scale.y = scale.x;
				}
				else
				{
					scale = new FlxPoint(1, 1);
					initialScale = scale;
				}
			}

			acceleration.x = 0;
			if ((FlxG.keys.LEFT || FlxG.keys.A) && moveEnable)
			{
				facing = FlxObject.LEFT;
				acceleration.x = -maxVelocity.x * 4;
			}

			if ((FlxG.keys.RIGHT || FlxG.keys.D) && moveEnable)
			{
				facing = FlxObject.RIGHT;
				acceleration.x = maxVelocity.x * 4;
			}

			if (isTouching(FlxObject.FLOOR))
			{
				jumpThrottle = 0;
				if ((FlxG.keys.UP || FlxG.keys.W) && moveEnable && jumpEnable)
				{
					velocity.y = -maxVelocity.y * 0.4;
					if (walkingSound)
					{
						playjump = true;
					}
				}
			}

			if ((FlxG.keys.UP || FlxG.keys.W) && jumpThrottle < jumpThrottleMax && moveEnable && jumpEnable && velocity.y < 0)
			{
				++jumpThrottle;
				velocity.y -= maxVelocity.y * 0.043;
			}

			if (FlxG.keys.justPressed("SPACE") && moveEnable)
			{
				interact = true;
			}
			else
			{
				interact = false;
			}

			if (!dontAnimate)
			{
				if (velocity.y < 0)
				{
					play("jump");
				}
				else if (velocity.y > 0)
				{
					play("fall");
				}
				else if (velocity.x == 0)
				{
					play("stand");
				}
				else
				{
					play("run");
				}
			}

			if (fading && alpha > 0)
			{
				alpha -= 0.02;
			}

			if (born)
			{
				if (alpha < 1)
				{
					alpha += 0.02;
				}
				else
				{
					born = false;
				}
			}

			if (fadeIn)
			{
				if (alpha == 1)
				{
					alpha = 0;
				}
				else if (alpha < 0.98)
				{
					alpha += 0.02;
				}
				else
				{
					alpha = 1;
					fadeIn = false;
				}
			}

			if (timer.complete)
			{
				getVulnerable();
			}

			if (timer2.complete)
			{
				invulnerable();
			}

			if (timer3.complete)
			{
				getWell();
			}
		}

		public void getHurt()
		{
			if (!hpDecreased)
			{
				if (hp == 2)
				{
					if (scale.x == 1)
					{
						FlxG.play(Sfxhurt1);
					}
					else if (scale.x == 2)
					{
						FlxG.play(Sfxhurt2);
					}
					else if (scale.x == 4)
					{
						FlxG.play(Sfxhurt3);
					}
					else if (scale.x == 8)
					{
						FlxG.play(Sfxhurt4);
					}
					else if (scale.x == 16)
					{
						FlxG.play(Sfxhurt5);
					}
				}

				if (hp > 0)
				{
					--hp;
				}

				hpDecreased = true;
				timer.start(120);
				timer2.start(6);
				countFlash = 20;
			}

			if (hp == 0)
			{
				fading = true;
				moveEnable = false;
			}
		}

		private void getVulnerable()
		{
			hpDecreased = false;
			timer3.start(120);
		}

		private void getWell()
		{
			hp = 2;
		}

		private void invulnerable()
		{
			if (countFlash > 0)
			{
				if (!fading)
				{
					if (alpha == 0)
					{
						alpha = 1;
					}
					else
					{
						alpha = 0;
					}
				}

				--countFlash;
				timer2.start(6);
			}
		}

		public void gokyuzundenDus()
		{
			loadGraphic(S_dusme, true, true, 32, 32, false);
			width = 8;
			height = 28;
			offset.x = 12;
			offset.y = 4;
			addAnimation("fall0", new[] { 0, 1 }, 6, true);
			addAnimation("fall1", new[] { 2, 3 }, 6, true);
			addAnimation("fall2", new[] { 4, 5 }, 6, true);
			addAnimation("fall3", new[] { 6, 7 }, 6, true);
			dontAnimate = true;
		}

		public void fall0()
		{
			play("fall0");
		}

		public void fall1()
		{
			play("fall1");
		}

		public void fall2()
		{
			play("fall2");
		}

		public void fall3()
		{
			play("fall3");
		}
	}
}
