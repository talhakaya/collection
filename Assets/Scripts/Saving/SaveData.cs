using System;
using System.Collections.Generic;

namespace Collection.Saving
{
	/// <summary>
	/// Everything the collection keeps between sessions, written as one JSON file
	/// (see SaveManager). Three story slots, a separate area for games started from
	/// "Just the games", and the settings, which belong to the player rather than to a slot.
	/// </summary>
	[Serializable]
	public class SaveData
	{
		public const int SlotCount = 3;

		public int version = 1;
		public List<SaveSlotData> slots = new List<SaveSlotData>();

		/// What games started from "Just the games" save into. Never shown as a slot.
		public SaveSlotData freePlay = new SaveSlotData();

		public Settings settings = new Settings();
	}

	[Serializable]
	public class Settings
	{
		public float masterVolume = 1f;
		public float musicVolume = 1f;
		public bool fullscreen = true;

		/// Zero means the display's own resolution.
		public int resolutionWidth;
		public int resolutionHeight;
	}

	/// <summary>
	/// One playthrough: how long it has been played, and each game's own save. A game reads
	/// and writes its part through SaveManager.Slot and calls SaveManager.MarkDirty() after
	/// changing it.
	///
	/// A game that keeps nothing between sessions has no part here.
	/// </summary>
	[Serializable]
	public class SaveSlotData
	{
		/// False for a slot nobody has started a game in.
		public bool started;

		/// Seconds spent in games with time running: menus and the pause screen do not count.
		public float timePlayed;

		public BloodSpaceSave bloodSpace = new BloodSpaceSave();
		public CrimeFactorySave crimeFactory = new CrimeFactorySave();
		public GolfinitySave golfinity = new GolfinitySave();
		public LovesFirstWeekSave lovesFirstWeek = new LovesFirstWeekSave();
		public NykrigSave nykrig = new NykrigSave();
		public OdeToPixelDaysSave odeToPixelDays = new OdeToPixelDaysSave();
		public PenisClonerSave penisCloner = new PenisClonerSave();
		public SleepyTimeSave sleepyTime = new SleepyTimeSave();
		public VirtualPetSave virtualPet = new VirtualPetSave();

		/// The story mode's own part.
		public StorySave story = new StorySave();
	}

	/// <summary>
	/// What the story mode keeps. The main character: the shape of its head, and the
	/// look of its body as set in the character creator (a development tool) - its
	/// proportions, and each choice from the character catalog by the option's id, so that
	/// reordering or renaming the catalog does not change a saved character. And how far
	/// the story has got: the conversations had, the artifacts won, where the character is.
	/// </summary>
	[Serializable]
	public class StorySave
	{
		/// False until a look has been saved; the character keeps the one it has in the scene.
		public bool characterSaved;
		public float height = 1f;
		public float fatness = 1f;

		/// The catalog choices, a key ("top", "pantsColor") and the chosen option's id.
		public List<string> optionKeys = new List<string>();
		public List<string> optionIds = new List<string>();

		/// The shape of the main character's head: its six sides (right, left, up, down,
		/// front, back), each from 0, a flat cube side, to 1, the sphere. A new game starts
		/// with the sphere.
		public List<float> headSides = new List<float> { 1f, 1f, 1f, 1f, 1f, 1f };

		/// Conversations that happen only once and have happened, by their Yarn node's name.
		public List<string> conversations = new List<string>();

		/// Things remembered from conversations (a choice the player made), as a name and a
		/// value. See StoryMemory.
		public List<string> memoryKeys = new List<string>();
		public List<string> memoryValues = new List<string>();

		/// The level the story is at, counted from 0 (see GameContext.StoryLevelScenes).
		public int level;

		/// Coins, from chests and barrels.
		public int coins;

		/// Things that are used up for good, by their ids: chests opened, barrels broken.
		public List<string> spent = new List<string>();

		/// The artifacts won so far, by their ids, in the order they were won: the order
		/// they follow the character in.
		public List<string> artifacts = new List<string>();

		/// Where the character was standing when the story was last left (for a game, or for
		/// the menu), and which way it faced. Not set until then.
		public bool placeSaved;
		public float placeX;
		public float placeY;
		public float placeZ;
		public float placeFacing;

		/// Where the level's bicycle was left, once it has been ridden: where it stands on
		/// the ground, and which way it faces. Until then it is where its scene has it.
		public bool bikeSaved;
		public float bikeX;
		public float bikeY;
		public float bikeZ;
		public float bikeFacing;
	}

	[Serializable]
	public class BloodSpaceSave
	{
		public int won;
		public int gameMode;
		public int highscore;
		public bool musicOn = true;
	}

	[Serializable]
	public class CrimeFactorySave
	{
		/// Empty until a level has been reached: there is nothing to continue.
		public string lastLevel = "";
		public int money;
		public int fireRateUpgrades;
		public int shields;
		public int bombs;
	}

	[Serializable]
	public class GolfinitySave
	{
		public int noOfStrokes;
		public int gold;

		/// Stars per hole, a digit a hole, in strings of a fixed number of holes.
		public List<string> stars = new List<string>();

		/// Gold still owed on each lock, by lock index. -1: nothing paid yet.
		public List<int> locks = new List<int>();

		public bool accessedUpgradePopup;
		public bool unlock0Enabled;
		public bool unlock1Enabled;
		public bool unlock0Bought;
		public bool unlock1Bought;

		// The game's own options.
		public bool outlineOn = true;
		public bool reverseShooting = true;
		public bool holesOnWalls = true;
		public bool soundOn = true;
		public bool musicOn = true;
		public bool terrainEffectOn = true;
		public bool circleHoleEffectOn = true;

		/// The game's Lang value. -1: not chosen, follow the system language.
		public int lang = -1;
	}

	/// The game kept these in a Flash shared object, where a property that was never set is
	/// absent; -1 and the empty string stand for that here.
	[Serializable]
	public class LovesFirstWeekSave
	{
		public int level = -1;
		public string lang = "";
		public int dialog = -1;
		public int is1p = -1;
	}

	[Serializable]
	public class NykrigSave
	{
		public int score;
	}

	[Serializable]
	public class OdeToPixelDaysSave
	{
		/// -1 until a game has been started.
		public int level = -1;
	}

	[Serializable]
	public class PenisClonerSave
	{
		public int level;
	}

	[Serializable]
	public class SleepyTimeSave
	{
		public bool playedBefore;
		public List<int> scores = new List<int>();
	}

	[Serializable]
	public class VirtualPetSave
	{
		public bool hasSave;
		public string playerName = "";
		public string petName = "";
	}
}
