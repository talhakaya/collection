using UnityEngine;

namespace Games.LovesFirstWeek
{
	/// <summary>
	/// org.flixel.FlxSave, as far as the game uses it. The original is a Flash
	/// SharedObject with a free-form data object; the game keeps four things in it - the
	/// level reached, the language, whether the level's dialogue should play, and whether
	/// the game in progress is for one player - so data has exactly those.
	///
	/// A SharedObject wrote itself out when the player closed. Here each change is written
	/// to PlayerPrefs as it is made, under a prefixed key - PlayerPrefs is shared by every
	/// game in the collection.
	/// </summary>
	public class FlxSave
	{
		private const string Prefix = "LovesFirstWeek.";

		/// Every property is null until it has been set, as an unset SharedObject property is.
		public class Data
		{
			private readonly string prefix;

			internal Data(string prefix)
			{
				this.prefix = prefix;
			}

			public int? level
			{
				get { return PlayerPrefs.HasKey(prefix + "level") ? PlayerPrefs.GetInt(prefix + "level") : (int?)null; }
				set { setInt("level", value); }
			}

			public string lang
			{
				get { return PlayerPrefs.HasKey(prefix + "lang") ? PlayerPrefs.GetString(prefix + "lang") : null; }
				set
				{
					if (value == null)
					{
						PlayerPrefs.DeleteKey(prefix + "lang");
					}
					else
					{
						PlayerPrefs.SetString(prefix + "lang", value);
					}

					PlayerPrefs.Save();
				}
			}

			public bool? dialog
			{
				get { return getBool("dialog"); }
				set { setInt("dialog", value == null ? (int?)null : (value.Value ? 1 : 0)); }
			}

			public bool? is1p
			{
				get { return getBool("is1p"); }
				set { setInt("is1p", value == null ? (int?)null : (value.Value ? 1 : 0)); }
			}

			private bool? getBool(string key)
			{
				return PlayerPrefs.HasKey(prefix + key) ? PlayerPrefs.GetInt(prefix + key) != 0 : (bool?)null;
			}

			private void setInt(string key, int? value)
			{
				if (value == null)
				{
					PlayerPrefs.DeleteKey(prefix + key);
				}
				else
				{
					PlayerPrefs.SetInt(prefix + key, value.Value);
				}

				PlayerPrefs.Save();
			}
		}

		public string name;
		public Data data;

		public bool bind(string Name)
		{
			name = Name;
			data = new Data(Prefix + Name + ".");
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
	}
}
