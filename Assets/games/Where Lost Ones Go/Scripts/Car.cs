namespace Games.WhereLostOnesGo
{
	/// A car on the bridge: a few translucent black pixels driving from one end to the
	/// other, then replaced by a new one. Ported from Car.as.
	public class Car : Sprite
	{
		public int whichCarX;
		public int whichCarY;

		public Car()
		{
			addEventListener(Event.ENTER_FRAME, enterFrameHandler);
			x = Game.CarXMin + Flash.random() * (Game.CarXMax - Game.CarXMin);
			y = Game.CarY;
			double rand = Flash.random();
			if (rand < 0.25)
			{
				whichCarX = 1;
				whichCarY = 1;
			}
			else if (rand < 0.5)
			{
				whichCarX = 2;
				whichCarY = 1;
			}
			else if (rand < 0.75)
			{
				whichCarX = 4;
				whichCarY = 2;
			}
			else
			{
				whichCarX = 5;
				whichCarY = 2;
			}

			y -= whichCarY;
			rand = Flash.random();
			double destinationX = Game.CarXMin;
			if (rand < 0.5)
			{
				destinationX = Game.CarXMax;
			}

			TweenLite.to(this, (25 + 25 * Flash.random()) * Game.debugTimerConst, new TweenVars
			{
				x = destinationX,
				y = Game.CarY - whichCarY,
				onComplete = tweenDoneHandler,
			});
		}

		public void enterFrameHandler()
		{
			graphics.clear();
			graphics.beginFill(4278190080, 0.25);
			graphics.drawRect(0, 0, whichCarX, whichCarY);
			graphics.endFill();
		}

		public void tweenDoneHandler()
		{
			removeEventListener(Event.ENTER_FRAME, enterFrameHandler);
			parent.addChild(new Car());
			parent.removeChild(this);
		}
	}
}
