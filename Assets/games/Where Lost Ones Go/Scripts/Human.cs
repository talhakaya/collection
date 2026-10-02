using System;

namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// A figure. Ported from Human_2.as (class Human, over the library symbol human).
	///
	/// In the red sea they lie on their sides, trembling, and sink away once the mouse
	/// touches them. Standing (the later scene) they tremble too, and once touched they
	/// back away from the player, shedding grey particles towards it.
	/// </summary>
	public class Human : human
	{
		public double radius = 20;
		public bool dieAlready;
		public double speed = 1.5;
		public double speedInBlood = 1.5;
		public bool inBlood;
		public double shakingAngle = 5;
		private bool lyingToRight;

		public Human(double _x, double _y, bool _inBlood)
		{
			x = _x;
			y = _y;
			addEventListener(Event.ENTER_FRAME, enterFrameHandler);
			inBlood = _inBlood;
			if (inBlood)
			{
				scaleX = 2;
				scaleY = 2;
			}
			else
			{
				scaleX = 3;
				scaleY = 3;
			}

			if (Flash.random() > 0.5)
			{
				lyingToRight = true;
			}
		}

		public void enterFrameHandler()
		{
			if (Game.STATE == 0 || x < -2 * radius || x > 640 + 2 * radius || y < -2 * radius || y > 360 + 2 * radius)
			{
				if (parent != null)
				{
					removeEventListener(Event.ENTER_FRAME, enterFrameHandler);
					parent.removeChild(this);
				}
			}

			if (Game.instance.player != null)
			{
				if (!inBlood)
				{
					double angleToPlayer = Math.Atan2(Game.instance.player.globalPosition().y - y, Game.instance.player.globalPosition().x - x);
					if (!dieAlready)
					{
						if (Main.distanceBetweenMouseCloserThanRadius(radius, this))
						{
							dieAlready = true;
						}
					}
					else
					{
						x -= speed * Math.Cos(angleToPlayer);
						y -= speed * Math.Sin(angleToPlayer);
						x += -1 + 2 * Flash.random();
						y += -1 + 2 * Flash.random();
						alpha = 0.5 + 0.5 * Flash.random();
						Point globalPos = localToGlobal(new Point(0, -8));
						ScreenTextHandler.createRandomDirectionParticle(1, globalPos.x, globalPos.y, 10, 4286611584, new Point(10 * Math.Cos(angleToPlayer), 10 * Math.Sin(angleToPlayer)), true);
					}
				}
				else if (!dieAlready)
				{
					x += -2 + 4 * Flash.random();
					y += -2 + 4 * Flash.random();
					alpha = 0.5 + 0.5 * Flash.random();
					if (Main.distanceBetweenMouseCloserThanRadius(radius, this))
					{
						dieAlready = true;
					}

					if (lyingToRight)
					{
						rotation = 90 - shakingAngle + 2 * shakingAngle * Flash.random();
					}
					else
					{
						rotation = 270 - shakingAngle + 2 * shakingAngle * Flash.random();
					}
				}
				else
				{
					y += 1;
					alpha -= 0.01;
				}
			}
		}
	}
}
