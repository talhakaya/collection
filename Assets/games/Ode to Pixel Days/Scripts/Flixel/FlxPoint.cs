namespace Games.OdeToPixelDays
{
	/// <summary>
	/// org.flixel.FlxPoint. A class, not a struct, on purpose: the game shares one point
	/// between objects (every sprite in a level is handed the level's own scale point, and
	/// Hans shrinks it in place on the last level), which only works by reference.
	///
	/// Doubles throughout the Flixel layer, as ActionScript's Number is - so the game code
	/// transcribes with its literals as written, and the maths rounds as it did.
	/// </summary>
	public class FlxPoint
	{
		public double x;
		public double y;

		public FlxPoint(double X = 0, double Y = 0)
		{
			x = X;
			y = Y;
		}

		public FlxPoint make(double X = 0, double Y = 0)
		{
			x = X;
			y = Y;
			return this;
		}

		public FlxPoint copyFrom(FlxPoint Point)
		{
			x = Point.x;
			y = Point.y;
			return this;
		}

		public FlxPoint copyTo(FlxPoint Point)
		{
			Point.x = x;
			Point.y = y;
			return Point;
		}
	}

	/// org.flixel.FlxRect.
	public class FlxRect
	{
		public double x;
		public double y;
		public double width;
		public double height;

		public FlxRect(double X = 0, double Y = 0, double Width = 0, double Height = 0)
		{
			x = X;
			y = Y;
			width = Width;
			height = Height;
		}

		public double left { get { return x; } }
		public double right { get { return x + width; } }
		public double top { get { return y; } }
		public double bottom { get { return y + height; } }

		public FlxRect make(double X = 0, double Y = 0, double Width = 0, double Height = 0)
		{
			x = X;
			y = Y;
			width = Width;
			height = Height;
			return this;
		}

		public FlxRect copyFrom(FlxRect Rect)
		{
			x = Rect.x;
			y = Rect.y;
			width = Rect.width;
			height = Rect.height;
			return this;
		}

		public bool overlaps(FlxRect Rect)
		{
			return Rect.x + Rect.width > x && Rect.x < x + width && Rect.y + Rect.height > y && Rect.y < y + height;
		}
	}
}
