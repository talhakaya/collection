namespace Games.OdeToPixelDays
{
	/// <summary>
	/// org.flixel.FlxBasic - the root of everything a state can hold.
	///
	/// These are plain C# objects, not MonoBehaviours, so the game's constructors
	/// (new Hans(16, 132, scale)) transcribe as they are. Anything that needs a GameObject
	/// to be seen or heard gets one lazily, from FlxGame.
	/// </summary>
	public class FlxBasic
	{
		public int ID;
		public bool exists;
		public bool active;
		public bool visible;
		public bool alive;

		public FlxBasic()
		{
			ID = -1;
			exists = true;
			active = true;
			visible = true;
			alive = true;
		}

		public virtual void destroy()
		{
		}

		public virtual void preUpdate()
		{
		}

		public virtual void update()
		{
		}

		public virtual void postUpdate()
		{
		}

		public virtual void draw()
		{
		}

		public virtual void kill()
		{
			alive = false;
			exists = false;
		}

		public virtual void revive()
		{
			alive = true;
			exists = true;
		}
	}
}
