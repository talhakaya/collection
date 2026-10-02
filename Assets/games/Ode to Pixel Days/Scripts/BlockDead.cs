using System;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// A platform that gives way. Ported from BlockDead.as.
	///
	/// Half a second after Hans stands on it, it drops. Pulling its lever sends every fallen
	/// one back up to where it started.
	/// </summary>
	public class BlockDead : FlxSprite
	{
		private static string S_duz = "BlockDead_S_duz";
		private static string S_yan = "BlockDead_S_yan";
		private static string Sfx = "BlockDead_Sfx";

		public bool touched;
		public int count;
		public bool usesLever;
		public Lever2 lever;
		private bool leverIsOpen;
		private double initialY;
		public bool rising;

		public BlockDead(double _X, double _Y, FlxPoint _scale, bool _yan, bool _small, bool _usesLever)
		{
			count = 0;
			touched = false;
			x = _X;
			y = _Y;
			initialY = _Y;
			scale = _scale;
			usesLever = _usesLever;
			rising = false;
			int _color = (int)(Math.Floor(1 + 3 * FlxG.random()));
			if (_small)
			{
				if (_yan)
				{
					loadGraphic(S_yan, true, false, 12, 4, false);
					width = 12 * scale.x;
					height = 4 * scale.x;
				}
				else
				{
					loadGraphic(S_duz, true, false, 4, 12, false);
					width = 4 * scale.x;
					height = 12 * scale.x;
				}

				centerOffsets();
			}

			addAnimation("red", new[] { 0 }, 1, true);
			addAnimation("green", new[] { 1 }, 1, true);
			addAnimation("blue", new[] { 2 }, 1, true);
			if (_color == 1)
			{
				play("red");
			}
			else if (_color == 2)
			{
				play("green");
			}
			else
			{
				play("blue");
			}

			immovable = true;
		}

		public override void update()
		{
			base.update();
			if (count < 30 && touched)
			{
				++count;
			}

			if (count == 30)
			{
				acceleration.y = 200;
				++count;
			}

			if (leverIsOpen != lever.isOpen)
			{
				rising = true;
			}

			leverIsOpen = lever.isOpen;
			if (rising)
			{
				if (acceleration.y > 0)
				{
					acceleration.y = 0;
				}
				else if (acceleration.y == 0)
				{
					if (y > initialY)
					{
						velocity.y = -500;
					}
					else
					{
						count = 0;
						velocity.y = 0;
						y = initialY;
						rising = false;
						touched = false;
						FlxG.play(Sfx);
					}
				}
			}

			if (y > initialY + 720)
			{
				y = initialY + 720;
			}

			if (isTouching(FlxObject.FLOOR))
			{
				velocity.y = 0;
			}
		}

		public override void kill()
		{
			touched = true;
		}
	}
}
