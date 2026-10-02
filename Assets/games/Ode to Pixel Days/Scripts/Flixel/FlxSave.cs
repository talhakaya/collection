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
		private const string Prefix = "OdeToPixelDays.";

		public class Data
		{
			private readonly string key;

			internal Data(string key)
			{
				this.key = key;
			}

			/// Null until a game has been started, as an unset SharedObject property is.
			public int? level
			{
				get
				{
					if (!PlayerPrefs.HasKey(key))
					{
						return null;
					}

					return PlayerPrefs.GetInt(key);
				}
				set
				{
					if (value == null)
					{
						PlayerPrefs.DeleteKey(key);
					}
					else
					{
						PlayerPrefs.SetInt(key, value.Value);
					}

					PlayerPrefs.Save();
				}
			}
		}

		public string name;
		public Data data;

		public bool bind(string Name)
		{
			name = Name;
			data = new Data(Prefix + Name + ".level");
			return true;
		}

		public bool flush()
		{
			PlayerPrefs.Save();
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
