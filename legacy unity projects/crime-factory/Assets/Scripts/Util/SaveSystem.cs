using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem {
    private const string LAST_LEVEL = "LAST_LEVEL";
    private const string MONEY = "MONEY";
    private const string FIRE_RATE = "FIRE_RATE";
    private const string SHIELDS = "SHIELDS";
    private const string BOMBS = "BOMBS";

    public static void SaveLevel(string levelName) {
        PlayerPrefs.SetString(LAST_LEVEL, levelName);
        PlayerPrefs.Save();
    }

    public static void LoadLevel(out string lastLevelName) {
        lastLevelName = PlayerPrefs.GetString(LAST_LEVEL, "mom0");
    }

    public static void SaveStats(int money, int numFireRateUpgrades, int numShields, int numBombs) {
        PlayerPrefs.SetInt(MONEY, money);
        PlayerPrefs.SetInt(FIRE_RATE, numFireRateUpgrades);
        PlayerPrefs.SetInt(BOMBS, numBombs);
        SaveNumShields(numShields);
    }

    public static void SaveNumShields(int numShields) {
        PlayerPrefs.SetInt(SHIELDS, numShields);
        PlayerPrefs.Save();
    }

    public static void LoadStats(out int money, out int numFireRateUpgrades, out int numShields, out int numBombs) {
        money = PlayerPrefs.GetInt(MONEY, 0);
        numFireRateUpgrades = PlayerPrefs.GetInt(FIRE_RATE, 0);
        numShields = PlayerPrefs.GetInt(SHIELDS, 0);
        numBombs = PlayerPrefs.GetInt(BOMBS, 0);
    }

    public static bool HasSave() {
        return PlayerPrefs.GetString(LAST_LEVEL, "") != "";
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
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
