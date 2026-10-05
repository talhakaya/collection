using UnityEngine;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// org.flixel.FlxSave, as far as the game uses it. The original is a Flash
	/// SharedObject with a free-form data object; the game only ever stores one thing in
	/// it, the level reached, so data has exactly that.
	///
	/// A SharedObject wrote itself out when the player closed. Here each change is written
	/// to PlayerPrefs as it is made, under a prefixed key - PlayerPrefs is shared by every
	/// game in the collection.
	/// </summary>
	public class FlxSave
	{

		/// In the collection: the level was a PlayerPrefs key. It is the game's part of the
		/// collection's save file now, where -1 stands for "not set".
		public class Data
		{
			/// Null until a game has been started, as an unset SharedObject property is.
			public int? level
			{
				get
				{
					int saved = Collection.Saving.SaveManager.Slot.odeToPixelDays.level;
					return saved >= 0 ? saved : (int?)null;
				}
				set
				{
					Collection.Saving.SaveManager.Slot.odeToPixelDays.level = value ?? -1;
					Collection.Saving.SaveManager.MarkDirty();
				}
			}
		}

		public string name;
		public Data data;

		public bool bind(string Name)
		{
			name = Name;
			data = new Data();
			return true;
		}

		public bool flush()
		{
			return true;
		}

		public bool close()
		{
			return flush();
		}

		public bool erase()
		{
			if (data == null)
			{
				return false;
			}

			data.level = null;
			return true;
		}
	}
}
