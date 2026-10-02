namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// A glint on the water: a short white line of one to four pixels that fades in and
	/// out, then moves somewhere else on the sea. Ported from SeaEffect.as.
	///
	/// "Somewhere on the sea" excludes the quay in the bottom-right corner of the photo.
	/// </summary>
	public class SeaEffect : Sprite
	{
		public double _alpha = 0;
		public int whichX;
		public double alphaSpeed = 0.01 + 0.01 * Flash.random();
		public bool rightSpot;

		public SeaEffect()
		{
			addEventListener(Event.ENTER_FRAME, enterFrameHandler);
			doneHandler();
		}

		public void enterFrameHandler()
		{
			_alpha += alphaSpeed;
			double graphicsAlpha = 0;
			if (_alpha < 0.5)
			{
				graphicsAlpha = _alpha;
			}
			else
			{
				graphicsAlpha = 1 - _alpha;
			}

			graphics.clear();
			graphics.beginFill(2868903935, graphicsAlpha);
			graphics.drawRect(0, 0, whichX, 1);
			graphics.endFill();
			if (_alpha >= 1)
			{
				_alpha = 0;
				doneHandler();
			}
		}

		public void doneHandler()
		{
			do
			{
				x = 640 * Flash.random();
				y = 230 + Flash.random() * 90;
				double rand = Flash.random();
				if (rand < 0.25)
				{
					whichX = 1;
				}
				else if (rand < 0.5)
				{
					whichX = 2;
				}
				else if (rand < 0.75)
				{
					whichX = 3;
				}
				else
				{
					whichX = 4;
				}

				calculateRightSpot();
			}
			while (!rightSpot);
		}

		public void calculateRightSpot()
		{
			rightSpot = !(x > 330 && y > 270 && (x - 330) / 310 + (y - 270) / 90 > 1);
		}
	}
}
