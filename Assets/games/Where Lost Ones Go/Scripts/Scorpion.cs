using System;

namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// A scorpion. Ported from Scorpion_2.as (class Scorpion, over the library symbol
	/// scorpion).
	///
	/// It turns towards the player and crawls at it, trembling, and stops when it gets
	/// there - drawing blood. Touching one of the mouse's drops sends it back the other
	/// way, twice as fast. Game sets the two touch flags each frame; they are cleared again
	/// at the end of this one.
	/// </summary>
	public class Scorpion : scorpion
	{
		public double radius = 20;
		public bool dieAlready;
		public bool touchingPlayer;
		public double speed = 1.5;

		public Scorpion(double _x, double _y, double _speed)
		{
			x = _x;
			y = _y;
			speed = _speed;
			Game.instance.collisionCheckList.Add(this);
			addEventListener(Event.ENTER_FRAME, enterFrameHandler);
			scaleX = 2;
			scaleY = 2;
		}

		public void enterFrameHandler()
		{
			x += -1 + 2 * Flash.random();
			y += -1 + 2 * Flash.random();
			alpha = 0.5 + 0.5 * Flash.random();
			if (Game.instance.player != null)
			{
				rotation = Math.Atan2(Game.instance.player.globalPosition().y - y, Game.instance.player.globalPosition().x - x) * 180 / Math.PI;
				if (!dieAlready)
				{
					if (!touchingPlayer)
					{
						x += speed * Math.Cos(rotation * Math.PI / 180);
						y += speed * Math.Sin(rotation * Math.PI / 180);
					}
				}
				else
				{
					x -= 2 * speed * Math.Cos(rotation * Math.PI / 180);
					y -= 2 * speed * Math.Sin(rotation * Math.PI / 180);
				}
			}

			if (Game.STATE == 0 && parent != null)
			{
				removeEventListener(Event.ENTER_FRAME, enterFrameHandler);
				Game.instance.collisionCheckList.Remove(this);
				parent.removeChild(this);
			}

			dieAlready = false;
			touchingPlayer = false;
		}
	}
}
