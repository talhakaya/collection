using System;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// A crate that falls from above. Ported from BlockFalling.as.
	///
	/// It can be pushed along the floor, scraping as it goes; the level decides what happens
	/// when it lands on Hans or the boss.
	/// </summary>
	public class BlockFalling : FlxSprite
	{
		private static string S_duz = "BlockFalling_S_duz";
		private static string S_yan = "BlockFalling_S_yan";
		private static string Sfx = "BlockFalling_Sfx";

		public bool touchedFloor;
		private bool surukleniyor;
		private int count;

		public BlockFalling(double _X, double _Y, FlxPoint _scale, bool _yan, bool _small)
		{
			x = _X;
			y = _Y;
			scale = _scale;
			count = 0;
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

			acceleration.y = 600;
			drag.x = 400;
			touchedFloor = false;
			surukleniyor = false;
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
		}

		public override void update()
		{
			base.update();
			if (velocity.y > 100)
			{
				touchedFloor = false;
			}
			else if (velocity.y == 0)
			{
				touchedFloor = true;
			}

			if (isTouching(FlxObject.FLOOR) && (velocity.x > 50 || velocity.x < -50))
			{
				if (count == 0)
				{
					FlxG.play(Sfx);
				}

				++count;
				if (count == 6)
				{
					count = 0;
				}
			}
		}
	}
}
