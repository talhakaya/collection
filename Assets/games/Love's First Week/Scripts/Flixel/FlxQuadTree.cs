using System;

namespace Games.LovesFirstWeek
{
	/// A node of the singly-linked lists the quadtree keeps its objects in.
	public class FlxList
	{
		public FlxObject obj;
		public FlxList next;
	}

	/// <summary>
	/// org.flixel.system.FlxQuadTree, which is what FlxG.overlap and FlxG.collide run on.
	///
	/// Ported rather than replaced by a plain pair loop, because its quirks are part of how
	/// the game behaves:
	///   - which pairs are tested, and in what order, follows the tree;
	///   - an object that straddles several leaves is tested against the same partner once
	///     per leaf, so a callback can fire more than once in a step;
	///   - an object outside FlxG.worldBounds is never added, and so collides with nothing;
	///   - the overlap test uses each object's swept hull (where it was, to where it is),
	///     not just its current box.
	/// </summary>
	public class FlxQuadTree : FlxRect
	{
		public const uint A_LIST = 0;
		public const uint B_LIST = 1;

		public static uint divisions;

		protected static double _min;
		protected static FlxObject _object;
		protected static double _objectLeftEdge;
		protected static double _objectTopEdge;
		protected static double _objectRightEdge;
		protected static double _objectBottomEdge;
		protected static uint _list;
		protected static bool _useBothLists;
		protected static Func<FlxObject, FlxObject, bool> _processingCallback;
		protected static Action<FlxObject, FlxObject> _notifyCallback;
		protected static FlxList _iterator;

		protected bool _canSubdivide;
		protected FlxList _headA;
		protected FlxList _tailA;
		protected FlxList _headB;
		protected FlxList _tailB;
		protected FlxQuadTree _northWestTree;
		protected FlxQuadTree _northEastTree;
		protected FlxQuadTree _southEastTree;
		protected FlxQuadTree _southWestTree;
		protected double _leftEdge;
		protected double _rightEdge;
		protected double _topEdge;
		protected double _bottomEdge;
		protected double _halfWidth;
		protected double _halfHeight;
		protected double _midpointX;
		protected double _midpointY;

		public FlxQuadTree(double X, double Y, double Width, double Height, FlxQuadTree Parent = null)
			: base(X, Y, Width, Height)
		{
			_headA = _tailA = new FlxList();
			_headB = _tailB = new FlxList();

			// Copy the parent's children (if there are any)
			if (Parent != null)
			{
				FlxList iterator;
				FlxList ot;
				if (Parent._headA.obj != null)
				{
					iterator = Parent._headA;
					while (iterator != null)
					{
						if (_tailA.obj != null)
						{
							ot = _tailA;
							_tailA = new FlxList();
							ot.next = _tailA;
						}

						_tailA.obj = iterator.obj;
						iterator = iterator.next;
					}
				}

				if (Parent._headB.obj != null)
				{
					iterator = Parent._headB;
					while (iterator != null)
					{
						if (_tailB.obj != null)
						{
							ot = _tailB;
							_tailB = new FlxList();
							ot.next = _tailB;
						}

						_tailB.obj = iterator.obj;
						iterator = iterator.next;
					}
				}
			}
			else
			{
				// _min is a uint in the source, so the division truncates.
				_min = Math.Floor((width + height) / (2 * divisions));
			}

			_canSubdivide = width > _min || height > _min;

			// Set up comparison/sort helpers
			_northWestTree = null;
			_northEastTree = null;
			_southEastTree = null;
			_southWestTree = null;
			_leftEdge = x;
			_rightEdge = x + width;
			_halfWidth = width / 2;
			_midpointX = _leftEdge + _halfWidth;
			_topEdge = y;
			_bottomEdge = y + height;
			_halfHeight = height / 2;
			_midpointY = _topEdge + _halfHeight;
		}

		public void destroy()
		{
			_object = null;
			_processingCallback = null;
			_notifyCallback = null;
		}

		public void load(FlxBasic ObjectOrGroup1, FlxBasic ObjectOrGroup2 = null, Action<FlxObject, FlxObject> NotifyCallback = null, Func<FlxObject, FlxObject, bool> ProcessCallback = null)
		{
			add(ObjectOrGroup1, A_LIST);
			if (ObjectOrGroup2 != null)
			{
				add(ObjectOrGroup2, B_LIST);
				_useBothLists = true;
			}
			else
			{
				_useBothLists = false;
			}

			_notifyCallback = NotifyCallback;
			_processingCallback = ProcessCallback;
		}

		public void add(FlxBasic ObjectOrGroup, uint List)
		{
			_list = List;
			if (ObjectOrGroup is FlxGroup)
			{
				FlxGroup group = (FlxGroup)ObjectOrGroup;
				int i = 0;
				int l = group.members.Count;
				while (i < l)
				{
					FlxBasic basic = group.members[i++];
					if (basic != null && basic.exists)
					{
						if (basic is FlxGroup)
						{
							add(basic, List);
						}
						else if (basic is FlxObject)
						{
							_object = (FlxObject)basic;
							if (_object.exists && _object.allowCollisions != 0)
							{
								_objectLeftEdge = _object.x;
								_objectTopEdge = _object.y;
								_objectRightEdge = _object.x + _object.width;
								_objectBottomEdge = _object.y + _object.height;
								addObject();
							}
						}
					}
				}
			}
			else
			{
				_object = ObjectOrGroup as FlxObject;
				if (_object != null && _object.exists && _object.allowCollisions != 0)
				{
					_objectLeftEdge = _object.x;
					_objectTopEdge = _object.y;
					_objectRightEdge = _object.x + _object.width;
					_objectBottomEdge = _object.y + _object.height;
					addObject();
				}
			}
		}

		protected void addObject()
		{
			// If this quad (not its children) lies entirely inside this object, add it here
			if (!_canSubdivide || (_leftEdge >= _objectLeftEdge && _rightEdge <= _objectRightEdge && _topEdge >= _objectTopEdge && _bottomEdge <= _objectBottomEdge))
			{
				addToList();
				return;
			}

			// See if the selected object fits completely inside any of the quadrants
			if (_objectLeftEdge > _leftEdge && _objectRightEdge < _midpointX)
			{
				if (_objectTopEdge > _topEdge && _objectBottomEdge < _midpointY)
				{
					if (_northWestTree == null)
					{
						_northWestTree = new FlxQuadTree(_leftEdge, _topEdge, _halfWidth, _halfHeight, this);
					}

					_northWestTree.addObject();
					return;
				}

				if (_objectTopEdge > _midpointY && _objectBottomEdge < _bottomEdge)
				{
					if (_southWestTree == null)
					{
						_southWestTree = new FlxQuadTree(_leftEdge, _midpointY, _halfWidth, _halfHeight, this);
					}

					_southWestTree.addObject();
					return;
				}
			}

			if (_objectLeftEdge > _midpointX && _objectRightEdge < _rightEdge)
			{
				if (_objectTopEdge > _topEdge && _objectBottomEdge < _midpointY)
				{
					if (_northEastTree == null)
					{
						_northEastTree = new FlxQuadTree(_midpointX, _topEdge, _halfWidth, _halfHeight, this);
					}

					_northEastTree.addObject();
					return;
				}

				if (_objectTopEdge > _midpointY && _objectBottomEdge < _bottomEdge)
				{
					if (_southEastTree == null)
					{
						_southEastTree = new FlxQuadTree(_midpointX, _midpointY, _halfWidth, _halfHeight, this);
					}

					_southEastTree.addObject();
					return;
				}
			}

			// If it wasn't completely contained we have to check out the partial overlaps
			if (_objectRightEdge > _leftEdge && _objectLeftEdge < _midpointX && _objectBottomEdge > _topEdge && _objectTopEdge < _midpointY)
			{
				if (_northWestTree == null)
				{
					_northWestTree = new FlxQuadTree(_leftEdge, _topEdge, _halfWidth, _halfHeight, this);
				}

				_northWestTree.addObject();
			}

			if (_objectRightEdge > _midpointX && _objectLeftEdge < _rightEdge && _objectBottomEdge > _topEdge && _objectTopEdge < _midpointY)
			{
				if (_northEastTree == null)
				{
					_northEastTree = new FlxQuadTree(_midpointX, _topEdge, _halfWidth, _halfHeight, this);
				}

				_northEastTree.addObject();
			}

			if (_objectRightEdge > _midpointX && _objectLeftEdge < _rightEdge && _objectBottomEdge > _midpointY && _objectTopEdge < _bottomEdge)
			{
				if (_southEastTree == null)
				{
					_southEastTree = new FlxQuadTree(_midpointX, _midpointY, _halfWidth, _halfHeight, this);
				}

				_southEastTree.addObject();
			}

			if (_objectRightEdge > _leftEdge && _objectLeftEdge < _midpointX && _objectBottomEdge > _midpointY && _objectTopEdge < _bottomEdge)
			{
				if (_southWestTree == null)
				{
					_southWestTree = new FlxQuadTree(_leftEdge, _midpointY, _halfWidth, _halfHeight, this);
				}

				_southWestTree.addObject();
			}
		}

		protected void addToList()
		{
			FlxList ot;
			if (_list == A_LIST)
			{
				if (_tailA.obj != null)
				{
					ot = _tailA;
					_tailA = new FlxList();
					ot.next = _tailA;
				}

				_tailA.obj = _object;
			}
			else
			{
				if (_tailB.obj != null)
				{
					ot = _tailB;
					_tailB = new FlxList();
					ot.next = _tailB;
				}

				_tailB.obj = _object;
			}

			if (!_canSubdivide)
			{
				return;
			}

			if (_northWestTree != null) _northWestTree.addToList();
			if (_northEastTree != null) _northEastTree.addToList();
			if (_southEastTree != null) _southEastTree.addToList();
			if (_southWestTree != null) _southWestTree.addToList();
		}

		public bool execute()
		{
			bool overlapProcessed = false;

			if (_headA.obj != null)
			{
				FlxList iterator = _headA;
				while (iterator != null)
				{
					_object = iterator.obj;
					if (_useBothLists)
					{
						_iterator = _headB;
					}
					else
					{
						_iterator = iterator.next;
					}

					if (_object.exists && _object.allowCollisions > 0 &&
					    _iterator != null && _iterator.obj != null &&
					    _iterator.obj.exists && overlapNode())
					{
						overlapProcessed = true;
					}

					iterator = iterator.next;
				}
			}

			// Advance through the tree by calling overlap on each child
			if (_northWestTree != null && _northWestTree.execute()) overlapProcessed = true;
			if (_northEastTree != null && _northEastTree.execute()) overlapProcessed = true;
			if (_southEastTree != null && _southEastTree.execute()) overlapProcessed = true;
			if (_southWestTree != null && _southWestTree.execute()) overlapProcessed = true;

			return overlapProcessed;
		}

		/// Note overlapProcessed is not reset between partners - once one pair has been
		/// processed, the notify callback fires for every later hull overlap in this node.
		/// That is how the source reads, so that is how this runs.
		protected bool overlapNode()
		{
			bool overlapProcessed = false;
			while (_iterator != null)
			{
				if (!_object.exists || _object.allowCollisions <= 0)
				{
					break;
				}

				FlxObject checkObject = _iterator.obj;
				if (_object == checkObject || !checkObject.exists || checkObject.allowCollisions <= 0)
				{
					_iterator = _iterator.next;
					continue;
				}

				// Calculate bulk hull for _object
				double objectHullX = _object.x < _object.last.x ? _object.x : _object.last.x;
				double objectHullY = _object.y < _object.last.y ? _object.y : _object.last.y;
				double objectHullWidth = _object.x - _object.last.x;
				objectHullWidth = _object.width + (objectHullWidth > 0 ? objectHullWidth : -objectHullWidth);
				double objectHullHeight = _object.y - _object.last.y;
				objectHullHeight = _object.height + (objectHullHeight > 0 ? objectHullHeight : -objectHullHeight);

				// Calculate bulk hull for checkObject
				double checkObjectHullX = checkObject.x < checkObject.last.x ? checkObject.x : checkObject.last.x;
				double checkObjectHullY = checkObject.y < checkObject.last.y ? checkObject.y : checkObject.last.y;
				double checkObjectHullWidth = checkObject.x - checkObject.last.x;
				checkObjectHullWidth = checkObject.width + (checkObjectHullWidth > 0 ? checkObjectHullWidth : -checkObjectHullWidth);
				double checkObjectHullHeight = checkObject.y - checkObject.last.y;
				checkObjectHullHeight = checkObject.height + (checkObjectHullHeight > 0 ? checkObjectHullHeight : -checkObjectHullHeight);

				// Check for intersection of the two hulls
				if (objectHullX + objectHullWidth > checkObjectHullX &&
				    objectHullX < checkObjectHullX + checkObjectHullWidth &&
				    objectHullY + objectHullHeight > checkObjectHullY &&
				    objectHullY < checkObjectHullY + checkObjectHullHeight)
				{
					// Execute callback functions if they exist. The callbacks may run further
					// overlap checks of their own, so the statics are restored after each.
					FlxObject current = _object;
					FlxList iterator = _iterator;
					Func<FlxObject, FlxObject, bool> process = _processingCallback;
					Action<FlxObject, FlxObject> notify = _notifyCallback;
					bool useBothLists = _useBothLists;

					if (process == null || process(current, checkObject))
					{
						overlapProcessed = true;
					}

					if (overlapProcessed && notify != null)
					{
						notify(current, checkObject);
					}

					_object = current;
					_iterator = iterator;
					_processingCallback = process;
					_notifyCallback = notify;
					_useBothLists = useBothLists;
				}

				_iterator = _iterator.next;
			}

			return overlapProcessed;
		}
	}
}
