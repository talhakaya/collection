using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public static class SaveSystem {
	    // In the collection: these were PlayerPrefs keys. The game's save is its part of the
	    // collection's save file now.
	    private static Collection.Saving.CrimeFactorySave Data {
	        get { return Collection.Saving.SaveManager.Slot.crimeFactory; }
	    }

	    public static void SaveLevel(string levelName) {
	        Data.lastLevel = levelName;
	        Collection.Saving.SaveManager.MarkDirty();
	    }

	    public static void LoadLevel(out string lastLevelName) {
	        lastLevelName = Data.lastLevel != "" ? Data.lastLevel : "mom0";
	    }

	    public static void SaveStats(int money, int numFireRateUpgrades, int numShields, int numBombs) {
	        Data.money = money;
	        Data.fireRateUpgrades = numFireRateUpgrades;
	        Data.bombs = numBombs;
	        SaveNumShields(numShields);
	    }

	    public static void SaveNumShields(int numShields) {
	        Data.shields = numShields;
	        Collection.Saving.SaveManager.MarkDirty();
	    }

	    public static void LoadStats(out int money, out int numFireRateUpgrades, out int numShields, out int numBombs) {
	        money = Data.money;
	        numFireRateUpgrades = Data.fireRateUpgrades;
	        numShields = Data.shields;
	        numBombs = Data.bombs;
	    }

	    public static bool HasSave() {
	        return Data.lastLevel != "";
	    }

	    public static void Save(string lastLevelName, int money, int numFireRateUpgrades, int numShields, int numBombs) {
	        SaveLevel(lastLevelName);
	        SaveStats(money, numFireRateUpgrades, numShields, numBombs);
	    }

	    public static void Load(out string lastLevelName, out int money, out int numFireRateUpgrades, out int numShields, out int numBombs) {
	        LoadLevel(out lastLevelName);
	        LoadStats(out money, out numFireRateUpgrades, out numShields, out numBombs);
	    }

	    public static void WipeData() {
	        // In the collection: was PlayerPrefs.DeleteAll(). Only this game's part of the slot
	        // is emptied.
	        Collection.Saving.SaveManager.Slot.crimeFactory = new Collection.Saving.CrimeFactorySave();
	        Collection.Saving.SaveManager.MarkDirty();
	    }
	}
}
