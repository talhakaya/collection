namespace Games.LovesFirstWeek
{
	/// <summary>
	/// org.flixel.FlxCamera, reduced to what the game asks of it: one camera that follows
	/// a target and is kept inside the level's bounds. The effects (flash, fade, shake) are
	/// never called and are left out.
	///
	/// It is only the scroll offset. Everything drawn subtracts it from its own position;
	/// the Unity camera that renders the game never moves.
	/// </summary>
	public class FlxCamera : FlxBasic
	{
		public const uint STYLE_LOCKON = 0;
		public const uint STYLE_PLATFORMER = 1;
		public const uint STYLE_TOPDOWN = 2;
		public const uint STYLE_TOPDOWN_TIGHT = 3;

		/// How large Flash drew each game pixel. The game sets it; nothing here reads it -
		/// the picture is fitted to the window instead.
		public static double defaultZoom = 1;

		public double width;
		public double height;
		public FlxObject target;
		public FlxRect deadzone;
		public FlxRect bounds;
		public FlxPoint scroll;
		public uint bgColor;

		protected FlxPoint _point;

		public FlxCamera(int X, int Y, int Width, int Height, double Zoom = 0)
		{
			width = Width;
			height = Height;
			target = null;
			deadzone = null;
			scroll = new FlxPoint();
			_point = new FlxPoint();
			bounds = null;
			bgColor = FlxG.bgColor;
		}

		public override void destroy()
		{
			target = null;
			scroll = null;
			deadzone = null;
			bounds = null;
			_point = null;
		}

		public override void update()
		{
			// Either follow the object closely,
			// or doublecheck our deadzone and update accordingly.
			if (target != null)
			{
				if (deadzone == null)
				{
					focusOn(target.getMidpoint(_point));
				}
				else
				{
					double edge;
					double targetX = target.x + (target.x > 0 ? 0.0000001 : -0.0000001);
					double targetY = target.y + (target.y > 0 ? 0.0000001 : -0.0000001);

					edge = targetX - deadzone.x;
					if (scroll.x > edge)
					{
						scroll.x = edge;
					}

					edge = targetX + target.width - deadzone.x - deadzone.width;
					if (scroll.x < edge)
					{
						scroll.x = edge;
					}

					edge = targetY - deadzone.y;
					if (scroll.y > edge)
					{
						scroll.y = edge;
					}

					edge = targetY + target.height - deadzone.y - deadzone.height;
					if (scroll.y < edge)
					{
						scroll.y = edge;
					}
				}
			}

			// Make sure we didn't go outside the camera's bounds
			if (bounds != null)
			{
				if (scroll.x < bounds.left)
				{
					scroll.x = bounds.left;
				}

				if (scroll.x > bounds.right - width)
				{
					scroll.x = bounds.right - width;
				}

				if (scroll.y < bounds.top)
				{
					scroll.y = bounds.top;
				}

				if (scroll.y > bounds.bottom - height)
				{
					scroll.y = bounds.bottom - height;
				}
			}
		}

		public void follow(FlxObject Target, uint Style = STYLE_LOCKON)
		{
			target = Target;
			double helper;
			switch (Style)
			{
				case STYLE_PLATFORMER:
					double w = width / 8;
					double h = height / 3;
					deadzone = new FlxRect((width - w) / 2, (height - h) / 2 - h * 0.25, w, h);
					break;
				case STYLE_TOPDOWN:
					helper = FlxU.max(width, height) / 4;
					deadzone = new FlxRect((width - helper) / 2, (height - helper) / 2, helper, helper);
					break;
				case STYLE_TOPDOWN_TIGHT:
					helper = FlxU.max(width, height) / 8;
					deadzone = new FlxRect((width - helper) / 2, (height - helper) / 2, helper, helper);
					break;
				case STYLE_LOCKON:
				default:
					deadzone = null;
					break;
			}
		}

		public void focusOn(FlxPoint Point)
		{
			Point.x += Point.x > 0 ? 0.0000001 : -0.0000001;
			Point.y += Point.y > 0 ? 0.0000001 : -0.0000001;
			scroll.make(Point.x - width * 0.5, Point.y - height * 0.5);
		}

		/// UpdateWorld also sets FlxG.worldBounds, which is the area collisions happen in:
		/// an object that leaves it stops colliding with anything.
		public void setBounds(double X = 0, double Y = 0, double Width = 0, double Height = 0, bool UpdateWorld = false)
		{
			if (bounds == null)
			{
				bounds = new FlxRect();
			}

			bounds.make(X, Y, Width, Height);
			if (UpdateWorld)
			{
				FlxG.worldBounds.copyFrom(bounds);
			}

			update();
		}
	}
}
