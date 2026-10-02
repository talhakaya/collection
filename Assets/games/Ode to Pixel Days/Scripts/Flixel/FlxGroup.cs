using System.Collections.Generic;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// org.flixel.FlxGroup. Members update and draw in the order they were added, which is
	/// the game's whole notion of layering: a level adds its background first and its
	/// monsters last.
	///
	/// A removed member leaves a null slot that the next add reuses, as in Flixel, so the
	/// order of the others never shifts.
	/// </summary>
	public class FlxGroup : FlxBasic
	{
		public List<FlxBasic> members;

		public FlxGroup()
		{
			members = new List<FlxBasic>();
		}

		public int length
		{
			get { return members.Count; }
		}

		public override void destroy()
		{
			if (members != null)
			{
				for (int i = 0; i < members.Count; i++)
				{
					FlxBasic basic = members[i];
					if (basic != null)
					{
						basic.destroy();
					}
				}

				members.Clear();
				members = null;
			}
		}

		public override void preUpdate()
		{
		}

		/// Indexed, not foreach: members add to the group they are in while it updates
		/// (a block spawning particles), and those must update the same step.
		public override void update()
		{
			int i = 0;
			while (i < members.Count)
			{
				FlxBasic basic = members[i++];
				if (basic != null && basic.exists && basic.active)
				{
					basic.preUpdate();
					basic.update();
					basic.postUpdate();
				}
			}
		}

		public override void draw()
		{
			int i = 0;
			while (i < members.Count)
			{
				FlxBasic basic = members[i++];
				if (basic != null && basic.exists && basic.visible)
				{
					basic.draw();
				}
			}
		}

		public T add<T>(T Object) where T : FlxBasic
		{
			// Don't bother adding an object twice.
			if (members.IndexOf(Object) >= 0)
			{
				return Object;
			}

			// First, look for a null entry where we can add the object.
			for (int i = 0; i < members.Count; i++)
			{
				if (members[i] == null)
				{
					members[i] = Object;
					return Object;
				}
			}

			members.Add(Object);
			return Object;
		}

		public FlxBasic remove(FlxBasic Object, bool Splice = false)
		{
			int index = members.IndexOf(Object);
			if (index < 0 || index >= members.Count)
			{
				return null;
			}

			if (Splice)
			{
				members.RemoveAt(index);
			}
			else
			{
				members[index] = null;
			}

			return Object;
		}

		public FlxBasic replace(FlxBasic OldObject, FlxBasic NewObject)
		{
			int index = members.IndexOf(OldObject);
			if (index < 0 || index >= members.Count)
			{
				return null;
			}

			members[index] = NewObject;
			return NewObject;
		}

		public FlxBasic getFirstAlive()
		{
			for (int i = 0; i < members.Count; i++)
			{
				FlxBasic basic = members[i];
				if (basic != null && basic.exists && basic.alive)
				{
					return basic;
				}
			}

			return null;
		}

		public int countLiving()
		{
			int count = -1;
			for (int i = 0; i < members.Count; i++)
			{
				FlxBasic basic = members[i];
				if (basic != null)
				{
					if (count < 0)
					{
						count = 0;
					}

					if (basic.exists && basic.alive)
					{
						count++;
					}
				}
			}

			return count;
		}

		public override void kill()
		{
			int i = 0;
			while (i < members.Count)
			{
				FlxBasic basic = members[i++];
				if (basic != null && basic.exists)
				{
					basic.kill();
				}
			}

			base.kill();
		}
	}

	/// org.flixel.FlxState: a group with a create() the game calls once it is current.
	public class FlxState : FlxGroup
	{
		public virtual void create()
		{
		}
	}
}
