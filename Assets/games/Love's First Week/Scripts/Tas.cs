namespace Games.LovesFirstWeek
{
	/// <summary>
	/// The stone. Ported from Tas.as.
	///
	/// It cannot be pushed, only kicked: it then flies in a straight line at full speed, and
	/// once something has slowed it, it drops and comes to rest where it lands.
	/// </summary>
	public class Tas : FlxSprite
	{
		private static string tas = "Tas_tas";
		private static string ses = "Tas_ses";

		public const double SPEED = 300;
		public int nazCount = 1;
		public bool move;
		public bool sesCaldi;
		public int sesCount = -1;

		public Tas(double _X, double _Y)
		{
			x = _X;
			y = _Y;
			immovable = true;
			move = false;
			drag.x = 0;
			maxVelocity.y = 500;
			maxVelocity.x = 500;
			loadGraphic(tas, false, false, 32, 32, false);
			width = 24;
			height = 28;
			offset.x = 4;
			offset.y = 4;
		}

		public override void update()
		{
			base.update();
			if (!immovable && velocity.x * velocity.x + velocity.y * velocity.y < SPEED * SPEED / 4 && !move)
			{
				velocity.x = 0;
				acceleration.y = 600;
				velocity.y = 20;
				move = true;
			}
			else if (!immovable && velocity.x * velocity.x + velocity.y * velocity.y < SPEED && isTouching(FLOOR))
			{
				makeImmovable();
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

		public void tekmelen(string direction)
		{
			if (!sesCaldi && !(direction == "left" && velocity.x == -SPEED || direction == "right" && velocity.x == SPEED || direction == "up" && velocity.y == -SPEED || direction == "down" && velocity.y == SPEED))
			{
				FlxG.play(ses, 1, false);
				sesCaldi = true;
				sesCount = 20;
			}

			velocity.x = 0;
			velocity.y = 0;
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

			immovable = false;
		}

		public void makeImmovable()
		{
			immovable = true;
			move = false;
			velocity.x = 0;
			velocity.y = 0;
			acceleration.y = 0;
		}
	}
}
