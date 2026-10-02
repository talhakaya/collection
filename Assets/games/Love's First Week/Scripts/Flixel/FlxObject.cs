using System;

namespace Games.LovesFirstWeek
{
	/// <summary>
	/// org.flixel.FlxObject: a rectangle with motion, and the pairwise separation that is
	/// all of Flixel's collision response.
	///
	/// This is the engine's own arithmetic, transcribed - not Unity physics. The game's
	/// feel is these exact formulas run at Flixel's fixed step: the half-step velocity
	/// integration in updateMotion, drag only applying while nothing accelerates, and
	/// separateX/separateY resolving one axis at a time with a 4-pixel overlap bias.
	///
	/// Rotation and paths are left out: the game uses neither, and every collider is an
	/// axis-aligned box.
	/// </summary>
	public class FlxObject : FlxBasic
	{
		public const uint LEFT = 0x0001;
		public const uint RIGHT = 0x0010;
		public const uint UP = 0x0100;
		public const uint DOWN = 0x1000;
		public const uint NONE = 0;
		public const uint CEILING = UP;
		public const uint FLOOR = DOWN;
		public const uint WALL = LEFT | RIGHT;
		public const uint ANY = LEFT | RIGHT | UP | DOWN;

		public const double OVERLAP_BIAS = 4;

		public double x;
		public double y;
		public double width;
		public double height;
		public bool immovable;
		public FlxPoint velocity;
		public double mass;
		public double elasticity;
		public FlxPoint acceleration;
		public FlxPoint drag;
		public FlxPoint maxVelocity;
		public FlxPoint scrollFactor;
		public double health;
		public bool moves;
		public uint touching;
		public uint wasTouching;
		public uint allowCollisions;
		public FlxPoint last;

		protected bool _flicker;
		protected double _flickerTimer;
		protected FlxPoint _point;
		protected FlxRect _rect;

		public FlxObject(double X = 0, double Y = 0, double Width = 0, double Height = 0)
		{
			x = X;
			y = Y;
			last = new FlxPoint(x, y);
			width = Width;
			height = Height;
			mass = 1.0;
			elasticity = 0.0;

			immovable = false;
			moves = true;

			touching = NONE;
			wasTouching = NONE;
			allowCollisions = ANY;

			velocity = new FlxPoint();
			acceleration = new FlxPoint();
			drag = new FlxPoint();
			maxVelocity = new FlxPoint(10000, 10000);

			scrollFactor = new FlxPoint(1.0, 1.0);
			_flicker = false;
			_flickerTimer = 0;

			_point = new FlxPoint();
			_rect = new FlxRect();
		}

		public override void preUpdate()
		{
			if (_flickerTimer != 0)
			{
				if (_flickerTimer > 0)
				{
					_flickerTimer -= FlxG.elapsed;
					if (_flickerTimer <= 0)
					{
						_flickerTimer = 0;
						_flicker = false;
					}
				}
			}

			last.x = x;
			last.y = y;
		}

		public override void postUpdate()
		{
			if (moves)
			{
				updateMotion();
			}

			wasTouching = touching;
			touching = NONE;
		}

		/// Half the velocity change is applied before the position moves and half after,
		/// which is what keeps a jump the same height whatever the step size.
		protected virtual void updateMotion()
		{
			double delta;
			double velocityDelta;

			velocityDelta = (FlxU.computeVelocity(velocity.x, acceleration.x, drag.x, maxVelocity.x) - velocity.x) / 2;
			velocity.x += velocityDelta;
			delta = velocity.x * FlxG.elapsed;
			velocity.x += velocityDelta;
			x += delta;

			velocityDelta = (FlxU.computeVelocity(velocity.y, acceleration.y, drag.y, maxVelocity.y) - velocity.y) / 2;
			velocity.y += velocityDelta;
			delta = velocity.y * FlxG.elapsed;
			velocity.y += velocityDelta;
			y += delta;
		}

		public virtual bool overlaps(FlxBasic ObjectOrGroup, bool InScreenSpace = false, FlxCamera Camera = null)
		{
			if (ObjectOrGroup is FlxGroup)
			{
				bool results = false;
				FlxGroup group = (FlxGroup)ObjectOrGroup;
				for (int i = 0; i < group.members.Count; i++)
				{
					FlxBasic basic = group.members[i];
					if (basic != null && overlaps(basic, InScreenSpace, Camera))
					{
						results = true;
					}
				}

				return results;
			}

			if (ObjectOrGroup is FlxTilemap)
			{
				// The tilemap's own test is the one that knows about individual tiles.
				return ((FlxTilemap)ObjectOrGroup).overlaps(this, InScreenSpace, Camera);
			}

			FlxObject obj = ObjectOrGroup as FlxObject;
			if (!InScreenSpace)
			{
				return obj.x + obj.width > x && obj.x < x + width && obj.y + obj.height > y && obj.y < y + height;
			}

			if (Camera == null)
			{
				Camera = FlxG.camera;
			}

			FlxPoint objectScreenPos = obj.getScreenXY(null, Camera);
			getScreenXY(_point, Camera);
			return objectScreenPos.x + obj.width > _point.x && objectScreenPos.x < _point.x + width &&
			       objectScreenPos.y + obj.height > _point.y && objectScreenPos.y < _point.y + height;
		}

		public virtual bool overlapsPoint(FlxPoint Point, bool InScreenSpace = false, FlxCamera Camera = null)
		{
			if (!InScreenSpace)
			{
				return Point.x > x && Point.x < x + width && Point.y > y && Point.y < y + height;
			}

			if (Camera == null)
			{
				Camera = FlxG.camera;
			}

			double X = Point.x - Camera.scroll.x;
			double Y = Point.y - Camera.scroll.y;
			getScreenXY(_point, Camera);
			return X > _point.x && X < _point.x + width && Y > _point.y && Y < _point.y + height;
		}

		public virtual bool onScreen(FlxCamera Camera = null)
		{
			if (Camera == null)
			{
				Camera = FlxG.camera;
			}

			getScreenXY(_point, Camera);
			return _point.x + width > 0 && _point.x < Camera.width && _point.y + height > 0 && _point.y < Camera.height;
		}

		/// The truncation of the scroll is Flixel's, and deliberate: it is what keeps
		/// scrolled sprites on whole pixels.
		public FlxPoint getScreenXY(FlxPoint Point = null, FlxCamera Camera = null)
		{
			if (Point == null)
			{
				Point = new FlxPoint();
			}

			if (Camera == null)
			{
				Camera = FlxG.camera;
			}

			Point.x = x - (int)(Camera.scroll.x * scrollFactor.x);
			Point.y = y - (int)(Camera.scroll.y * scrollFactor.y);
			Point.x += Point.x > 0 ? 0.0000001 : -0.0000001;
			Point.y += Point.y > 0 ? 0.0000001 : -0.0000001;
			return Point;
		}

		public void flicker(double Duration = 1)
		{
			_flickerTimer = Duration;
			if (_flickerTimer == 0)
			{
				_flicker = false;
			}
		}

		public bool flickering
		{
			get { return _flickerTimer != 0; }
		}

		public bool solid
		{
			get { return (allowCollisions & ANY) > NONE; }
			set { allowCollisions = value ? ANY : NONE; }
		}

		public FlxPoint getMidpoint(FlxPoint Point = null)
		{
			if (Point == null)
			{
				Point = new FlxPoint();
			}

			Point.x = x + width * 0.5;
			Point.y = y + height * 0.5;
			return Point;
		}

		public virtual void reset(double X, double Y)
		{
			revive();
			touching = NONE;
			wasTouching = NONE;
			x = X;
			y = Y;
			last.x = x;
			last.y = y;
			velocity.x = 0;
			velocity.y = 0;
		}

		public bool isTouching(uint Direction)
		{
			return (touching & Direction) > NONE;
		}

		public bool justTouched(uint Direction)
		{
			return (touching & Direction) > NONE && (wasTouching & Direction) <= NONE;
		}

		public virtual void hurt(double Damage)
		{
			health = health - Damage;
			if (health <= 0)
			{
				kill();
			}
		}

		public static bool separate(FlxObject Object1, FlxObject Object2)
		{
			bool separatedX = separateX(Object1, Object2);
			bool separatedY = separateY(Object1, Object2);
			return separatedX || separatedY;
		}

		public static bool separateX(FlxObject Object1, FlxObject Object2)
		{
			// can't separate two immovable objects
			bool obj1immovable = Object1.immovable;
			bool obj2immovable = Object2.immovable;
			if (obj1immovable && obj2immovable)
			{
				return false;
			}

			// If one of the objects is a tilemap, just pass it off.
			if (Object1 is FlxTilemap)
			{
				return ((FlxTilemap)Object1).overlapsWithCallback(Object2, separateX);
			}

			if (Object2 is FlxTilemap)
			{
				return ((FlxTilemap)Object2).overlapsWithCallback(Object1, separateX, true);
			}

			// First, get the two object deltas
			double overlap = 0;
			double obj1delta = Object1.x - Object1.last.x;
			double obj2delta = Object2.x - Object2.last.x;
			if (obj1delta != obj2delta)
			{
				// Check if the X hulls actually overlap
				double obj1deltaAbs = obj1delta > 0 ? obj1delta : -obj1delta;
				double obj2deltaAbs = obj2delta > 0 ? obj2delta : -obj2delta;
				FlxRect obj1rect = new FlxRect(Object1.x - (obj1delta > 0 ? obj1delta : 0), Object1.last.y, Object1.width + (obj1delta > 0 ? obj1delta : -obj1delta), Object1.height);
				FlxRect obj2rect = new FlxRect(Object2.x - (obj2delta > 0 ? obj2delta : 0), Object2.last.y, Object2.width + (obj2delta > 0 ? obj2delta : -obj2delta), Object2.height);
				if (obj1rect.x + obj1rect.width > obj2rect.x && obj1rect.x < obj2rect.x + obj2rect.width && obj1rect.y + obj1rect.height > obj2rect.y && obj1rect.y < obj2rect.y + obj2rect.height)
				{
					double maxOverlap = obj1deltaAbs + obj2deltaAbs + OVERLAP_BIAS;

					// If they did overlap (and can), figure out by how much and flip the corresponding flags
					if (obj1delta > obj2delta)
					{
						overlap = Object1.x + Object1.width - Object2.x;
						if (overlap > maxOverlap || (Object1.allowCollisions & RIGHT) == 0 || (Object2.allowCollisions & LEFT) == 0)
						{
							overlap = 0;
						}
						else
						{
							Object1.touching |= RIGHT;
							Object2.touching |= LEFT;
						}
					}
					else if (obj1delta < obj2delta)
					{
						overlap = Object1.x - Object2.width - Object2.x;
						if (-overlap > maxOverlap || (Object1.allowCollisions & LEFT) == 0 || (Object2.allowCollisions & RIGHT) == 0)
						{
							overlap = 0;
						}
						else
						{
							Object1.touching |= LEFT;
							Object2.touching |= RIGHT;
						}
					}
				}
			}

			// Then adjust their positions and velocities accordingly (if there was any overlap)
			if (overlap != 0)
			{
				double obj1v = Object1.velocity.x;
				double obj2v = Object2.velocity.x;

				if (!obj1immovable && !obj2immovable)
				{
					overlap *= 0.5;
					Object1.x = Object1.x - overlap;
					Object2.x += overlap;

					double obj1velocity = Math.Sqrt(obj2v * obj2v * Object2.mass / Object1.mass) * (obj2v > 0 ? 1 : -1);
					double obj2velocity = Math.Sqrt(obj1v * obj1v * Object1.mass / Object2.mass) * (obj1v > 0 ? 1 : -1);
					double average = (obj1velocity + obj2velocity) * 0.5;
					obj1velocity -= average;
					obj2velocity -= average;
					Object1.velocity.x = average + obj1velocity * Object1.elasticity;
					Object2.velocity.x = average + obj2velocity * Object2.elasticity;
				}
				else if (!obj1immovable)
				{
					Object1.x = Object1.x - overlap;
					Object1.velocity.x = obj2v - obj1v * Object1.elasticity;
				}
				else if (!obj2immovable)
				{
					Object2.x += overlap;
					Object2.velocity.x = obj1v - obj2v * Object2.elasticity;
				}

				return true;
			}

			return false;
		}

		public static bool separateY(FlxObject Object1, FlxObject Object2)
		{
			// can't separate two immovable objects
			bool obj1immovable = Object1.immovable;
			bool obj2immovable = Object2.immovable;
			if (obj1immovable && obj2immovable)
			{
				return false;
			}

			// If one of the objects is a tilemap, just pass it off.
			if (Object1 is FlxTilemap)
			{
				return ((FlxTilemap)Object1).overlapsWithCallback(Object2, separateY);
			}

			if (Object2 is FlxTilemap)
			{
				return ((FlxTilemap)Object2).overlapsWithCallback(Object1, separateY, true);
			}

			// First, get the two object deltas
			double overlap = 0;
			double obj1delta = Object1.y - Object1.last.y;
			double obj2delta = Object2.y - Object2.last.y;
			if (obj1delta != obj2delta)
			{
				// Check if the Y hulls actually overlap
				double obj1deltaAbs = obj1delta > 0 ? obj1delta : -obj1delta;
				double obj2deltaAbs = obj2delta > 0 ? obj2delta : -obj2delta;
				FlxRect obj1rect = new FlxRect(Object1.x, Object1.y - (obj1delta > 0 ? obj1delta : 0), Object1.width, Object1.height + obj1deltaAbs);
				FlxRect obj2rect = new FlxRect(Object2.x, Object2.y - (obj2delta > 0 ? obj2delta : 0), Object2.width, Object2.height + obj2deltaAbs);
				if (obj1rect.x + obj1rect.width > obj2rect.x && obj1rect.x < obj2rect.x + obj2rect.width && obj1rect.y + obj1rect.height > obj2rect.y && obj1rect.y < obj2rect.y + obj2rect.height)
				{
					double maxOverlap = obj1deltaAbs + obj2deltaAbs + OVERLAP_BIAS;

					// If they did overlap (and can), figure out by how much and flip the corresponding flags
					if (obj1delta > obj2delta)
					{
						overlap = Object1.y + Object1.height - Object2.y;
						if (overlap > maxOverlap || (Object1.allowCollisions & DOWN) == 0 || (Object2.allowCollisions & UP) == 0)
						{
							overlap = 0;
						}
						else
						{
							Object1.touching |= DOWN;
							Object2.touching |= UP;
						}
					}
					else if (obj1delta < obj2delta)
					{
						overlap = Object1.y - Object2.height - Object2.y;
						if (-overlap > maxOverlap || (Object1.allowCollisions & UP) == 0 || (Object2.allowCollisions & DOWN) == 0)
						{
							overlap = 0;
						}
						else
						{
							Object1.touching |= UP;
							Object2.touching |= DOWN;
						}
					}
				}
			}

			// Then adjust their positions and velocities accordingly (if there was any overlap)
			if (overlap != 0)
			{
				double obj1v = Object1.velocity.y;
				double obj2v = Object2.velocity.y;

				if (!obj1immovable && !obj2immovable)
				{
					overlap *= 0.5;
					Object1.y = Object1.y - overlap;
					Object2.y += overlap;

					double obj1velocity = Math.Sqrt(obj2v * obj2v * Object2.mass / Object1.mass) * (obj2v > 0 ? 1 : -1);
					double obj2velocity = Math.Sqrt(obj1v * obj1v * Object1.mass / Object2.mass) * (obj1v > 0 ? 1 : -1);
					double average = (obj1velocity + obj2velocity) * 0.5;
					obj1velocity -= average;
					obj2velocity -= average;
					Object1.velocity.y = average + obj1velocity * Object1.elasticity;
					Object2.velocity.y = average + obj2velocity * Object2.elasticity;
				}
				else if (!obj1immovable)
				{
					Object1.y = Object1.y - overlap;
					Object1.velocity.y = obj2v - obj1v * Object1.elasticity;
					// This is special case code that handles cases like horizontal moving platforms you can ride
					if (Object2.active && Object2.moves && obj1delta > obj2delta)
					{
						Object1.x += Object2.x - Object2.last.x;
					}
				}
				else if (!obj2immovable)
				{
					Object2.y += overlap;
					Object2.velocity.y = obj1v - obj2v * Object2.elasticity;
					// This is special case code that handles cases like horizontal moving platforms you can ride
					if (Object1.active && Object1.moves && obj1delta < obj2delta)
					{
						Object2.x += Object1.x - Object1.last.x;
					}
				}

				return true;
			}

			return false;
		}
	}
}
