namespace Games.LovesFirstWeek
{
	/// <summary>
	/// Either of the two characters - Naz or, with isNaz false, Talha. Ported from Naz.as.
	///
	/// The level does the walking and jumping; this class is the kicks. Kicking (tekme)
	/// freezes the character for half a second, and the kick itself lands part-way
	/// through. Being kicked (tekmelen) sends them off in a straight line, weightless,
	/// until they have all but stopped.
	/// </summary>
	public class Naz : FlxSprite
	{
		private static string naz = "Naz_naz";
		private static string talha = "Naz_talha";
		private static string talhatekme = "Naz_talhatekme";
		private static string naztekme = "Naz_naztekme";
		private static string naztekmelen = "Naz_naztekmelen";
		private static string talhatekmelen = "Naz_talhatekmelen";

		public const double SPEED = 120;
		public bool movable = true;
		public int jumpThrottle = 0;
		public int jumpThrottleMax = 14;
		public int moveCount = 0;
		public int tekmeCount = -1;
		public bool tekmeAtiyor;
		public bool tekmeAtti;
		public bool kapida;
		public bool kapidaTek;
		public bool sonBolum;
		public bool isNaz;
		public bool sesCaldi;
		public int sesCount = -1;

		public Naz(bool byNaz, double _X, double _Y)
		{
			x = _X;
			y = _Y;
			maxVelocity.y = 490;
			maxVelocity.x = SPEED;
			isNaz = byNaz;
			if (isNaz)
			{
				loadGraphic(naz, true, true, 32, 32, true);
			}
			else
			{
				loadGraphic(talha, true, true, 32, 32, true);
			}

			width = 16;
			height = 30;
			offset.y = 2;
			offset.x = 8;
			addAnimation("idle", new[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 1, 1, 1, 2, 3, 3, 3, 3 }, 6, true);
			addAnimation("walk", new[] { 4, 5, 6, 7 }, 6, true);
			addAnimation("up", new[] { 10, 11 }, 3, true);
			addAnimation("down", new[] { 12, 13 }, 3, true);
			addAnimation("tekme1", new[] { 8 }, 2, true);
			addAnimation("tekme2", new[] { 9 }, 2, true);
			addAnimation("op", new[] { 14, 15 }, 1, false);
			addAnimation("havada", new[] { 16, 17, 18, 19 }, 6, true);
			addAnimation("otur1", new[] { 20, 20, 20, 20, 20, 20, 20, 20, 21, 21, 21, 21, 22, 21, 21, 21, 22, 23, 23, 23, 23 }, 6, true);
			addAnimation("otur2", new[] { 20, 20, 21, 21, 21, 21, 22, 21, 21, 21, 22, 23, 23, 23, 23, 20, 20, 20, 20, 20, 20 }, 6, true);
			play("idle");
		}

		public override void update()
		{
			if (!sonBolum)
			{
				base.update();
				if (movable)
				{
					acceleration.y = 700;
					drag.x = maxVelocity.x * 4;
				}
				else if (!kapidaTek)
				{
					acceleration.y = 0;
					drag.x = 0;
					if (moveCount > 0)
					{
						--moveCount;
					}
					else if (moveCount == 0)
					{
						movable = true;
						tekmeAtiyor = false;
					}

					if (moveCount == -1 && velocity.x * velocity.x + velocity.y * velocity.y < SPEED)
					{
						movable = true;
						moveCount = 0;
					}
				}
				else
				{
					acceleration.y = 700;
					drag.x = maxVelocity.x * 4;
					velocity.x = 0;
				}

				if (tekmeCount > 0)
				{
					--tekmeCount;
				}
				else if (tekmeCount == 0)
				{
					tekmeAtiyor = true;
					tekmeAtti = true;
					play("tekme2");
					tekmeCount = -1;
					if (isNaz)
					{
						FlxG.play(naztekme, 1, false, true);
					}
					else
					{
						FlxG.play(talhatekme, 1, false, true);
					}
				}

				if (sesCaldi)
				{
					if (sesCount == 0)
					{
						sesCaldi = false;
					}

					--sesCount;
				}
			}
		}

		public void makeImmovable(int count)
		{
			moveCount = count;
			movable = false;
		}

		public void tekme()
		{
			tekmeCount = 15;
			makeImmovable(30);
			velocity.x = 0;
			velocity.y = 0;
			acceleration.x = 0;
			play("tekme1");
		}

		public void tekmelen(string direction)
		{
			velocity.x = 0;
			velocity.y = 0;
			acceleration.x = 0;
			if (!sesCaldi && !(direction == "left" && velocity.x == -SPEED || direction == "right" && velocity.x == SPEED || direction == "up" && velocity.y == -SPEED || direction == "down" && velocity.y == SPEED))
			{
				if (isNaz)
				{
					FlxG.play(naztekmelen, 1, false);
				}
				else
				{
					FlxG.play(talhatekmelen, 1, false);
				}

				sesCaldi = true;
				sesCount = 20;
			}

			if (direction == "left")
			{
				velocity.x = -SPEED;
			}
			else if (direction == "right")
			{
				velocity.x = SPEED;
			}
			else if (direction == "up")
			{
				velocity.y = -SPEED;
			}
			else if (direction == "down")
			{
				velocity.y = SPEED;
			}

			play("havada");
			makeImmovable(-1);
		}

		public void op()
		{
			play("op");
			velocity.x = 0;
			acceleration.x = 0;
			velocity.y = 0;
		}
	}
}
