namespace Games.WhereLostOnesGo
{
	/// <summary>
	/// A bird: one translucent black pixel flying across the sky. Ported from Bird.as.
	///
	/// The first flock leaves from the bridge's tower; each bird that arrives is replaced
	/// by one setting off from the left edge (the start point Game sets after the first
	/// flock).
	/// </summary>
	public class Bird : Sprite
	{
		public static double birdX;
		public static double birdY;
		public static double destinationX;
		public static double destinationY;

		public Bird(double _x, double _y, double _destinationX, double _destinationY)
		{
			graphics.beginFill(4278190080, 0.5);
			graphics.drawRect(0, 0, 1, 1);
			graphics.endFill();
			x = _x - Game.BirdX + Game.BirdX * 2 * Flash.random();
			y = _y - Game.BirdY + Game.BirdY * 2 * Flash.random();
			double destinationX = _destinationX - Game.BirdX + Game.BirdX * 2 * Flash.random();
			double destinationY = _destinationY - Game.BirdY + Game.BirdY * 2 * Flash.random();
			TweenLite.to(this, (45 + 5 * Flash.random()) * Game.debugTimerConst, new TweenVars
			{
				x = destinationX,
				y = destinationY,
				onComplete = tweenDoneHandler,
			});
		}

		public void tweenDoneHandler()
		{
			parent.addChild(new Bird(birdX, birdY, destinationX, destinationY));
			parent.removeChild(this);
		}
	}
}
