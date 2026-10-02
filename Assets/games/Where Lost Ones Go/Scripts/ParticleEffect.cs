using System;

namespace Games.WhereLostOnesGo
{
	/// A 4x4 particle, flung in a direction and slowing to a stop over ten frames, falling
	/// a pixel a frame if it has gravity. Ported from ParticleEffect.as.
	public class ParticleEffect : Sprite
	{
		public double force;
		public uint color;
		public Point direction;
		public bool isThereGravity;
		public const int constToDeath = 10;
		public int countDownToDeath = 10;

		public ParticleEffect(double _x, double _y, double _force, uint _color, Point _direction, bool _isThereGravity)
		{
			x = _x;
			y = _y;
			force = _force;
			color = _color;
			direction = _direction;
			isThereGravity = _isThereGravity;
			addEventListener(Event.ENTER_FRAME, enterFrameHandler);
			directionNormaling();
		}

		public void enterFrameHandler()
		{
			graphics.clear();
			--countDownToDeath;
			if ((countDownToDeath <= 0 || Game.STATE == 0) && parent != null)
			{
				removeEventListener(Event.ENTER_FRAME, enterFrameHandler);
				parent.removeChild(this);
			}
			else
			{
				graphics.beginFill(color, 0.5 + Flash.random() * 0.5);
				graphics.drawRect(0, 0, 4, 4);
				graphics.endFill();
				x += direction.x * force * countDownToDeath / constToDeath;
				y += direction.y * force * countDownToDeath / constToDeath;
				if (isThereGravity)
				{
					++y;
				}
			}
		}

		/// Makes the direction a unit vector, in place - the Point passed in is changed.
		public void directionNormaling()
		{
			if (direction.x == 0 && direction.y != 0)
			{
				direction.y = direction.y > 0 ? 1 : -1;
			}
			else if (direction.y == 0 && direction.x != 0)
			{
				direction.x = direction.x > 0 ? 1 : -1;
			}
			else if (direction.y != 0 && direction.x != 0)
			{
				double radiusOfDirection = Math.Sqrt(Math.Pow(direction.x, 2) + Math.Pow(direction.y, 2));
				direction.x /= radiusOfDirection;
				direction.y /= radiusOfDirection;
			}
		}
	}
}
