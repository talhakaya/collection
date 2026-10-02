using System;

namespace Games.LovesFirstWeek
{
	/// The handful of org.flixel.FlxU helpers the engine itself leans on.
	public static class FlxU
	{
		public static double abs(double Value)
		{
			return Value > 0 ? Value : -Value;
		}

		public static double floor(double Value)
		{
			return Math.Floor(Value);
		}

		public static double ceil(double Value)
		{
			return Math.Ceiling(Value);
		}

		public static double max(double Number1, double Number2)
		{
			return Number1 >= Number2 ? Number1 : Number2;
		}

		public static double min(double Number1, double Number2)
		{
			return Number1 <= Number2 ? Number1 : Number2;
		}

		/// <summary>
		/// One step of velocity under acceleration, or under drag when nothing is
		/// accelerating it, clamped to Max. 10000 is Flixel's "no limit" sentinel.
		/// </summary>
		public static double computeVelocity(double Velocity, double Acceleration = 0, double Drag = 0, double Max = 10000)
		{
			if (Acceleration != 0)
			{
				Velocity += Acceleration * FlxG.elapsed;
			}
			else if (Drag != 0)
			{
				double drag = Drag * FlxG.elapsed;
				if (Velocity - drag > 0)
				{
					Velocity -= drag;
				}
				else if (Velocity + drag < 0)
				{
					Velocity += drag;
				}
				else
				{
					Velocity = 0;
				}
			}

			if (Velocity != 0 && Max != 10000)
			{
				if (Velocity > Max)
				{
					Velocity = Max;
				}
				else if (Velocity < -Max)
				{
					Velocity = -Max;
				}
			}

			return Velocity;
		}
	}
}
