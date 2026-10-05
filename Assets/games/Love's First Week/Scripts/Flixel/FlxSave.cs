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

		/// Every property is null until it has been set, as an unset SharedObject property is.
		///
		/// In the collection: these were PlayerPrefs keys. They are the game's part of the
		/// collection's save file now, where -1 and the empty string stand for "not set".
		public class Data
		{
			private static Collection.Saving.LovesFirstWeekSave Saved
			{
				get { return Collection.Saving.SaveManager.Slot.lovesFirstWeek; }
			}

			public int? level
			{
				get { return Saved.level >= 0 ? Saved.level : (int?)null; }
				set { Saved.level = value ?? -1; Collection.Saving.SaveManager.MarkDirty(); }
			}

			public string lang
			{
				get { return Saved.lang != "" ? Saved.lang : null; }
				set { Saved.lang = value ?? ""; Collection.Saving.SaveManager.MarkDirty(); }
			}

			public bool? dialog
			{
				get { return Saved.dialog >= 0 ? Saved.dialog != 0 : (bool?)null; }
				set { Saved.dialog = value == null ? -1 : (value.Value ? 1 : 0); Collection.Saving.SaveManager.MarkDirty(); }
			}

			public bool? is1p
			{
				get { return Saved.is1p >= 0 ? Saved.is1p != 0 : (bool?)null; }
				set { Saved.is1p = value == null ? -1 : (value.Value ? 1 : 0); Collection.Saving.SaveManager.MarkDirty(); }
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
	}
}
