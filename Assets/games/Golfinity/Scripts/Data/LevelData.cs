using System;
using System.Collections;
using System.Collections.Generic;

namespace Games.Golfinity
{
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using UnityEngine;

	[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
	public class LevelData : ScriptableObject
	{
	    public List<Level> levels;
	    public List<Level> otherLevels;
	    public List<int> lockGold;

	    public int GetSeed(int holeNo)
	    {
	        if (holeNo >= 0 && holeNo < levels.Count) return levels[holeNo].seed;
	        for (int i = 0, len = otherLevels.Count; i < len; i++)
	        {
	            if (otherLevels[i].holeNo == holeNo)
	            {
	                return otherLevels[i].seed;
	            }
	        }
	        return holeNo;
	    }

	    public int GetNumHits(int holeNo)
	    {
	        if (holeNo > 0 && holeNo < levels.Count) return levels[holeNo].numHits;
	        for (int i = 0, len = otherLevels.Count; i < len; i++)
	        {
	            if (otherLevels[i].holeNo == holeNo)
	            {
	                return otherLevels[i].numHits;
	            }
	        }
	        LevelGenerator.GetLevelInfo(holeNo, GetSeed(holeNo), out int noOfColumns, out int noOfRows);
	        return GetDefaultNumHits(noOfColumns, noOfRows);
	    }

	    public static int GetDefaultNumHits(int noOfColumns, int noOfRows)
	    {
	        if (noOfRows > 10 && noOfColumns < 12)
	        {
	            return noOfRows * (noOfColumns / 3);
	        }
	        return (noOfRows / 2) * (noOfColumns / 3);
	    }

	    public int NumGoldTotalToUnlock(int lockIndex)
	    {
	        int i = lockIndex - 1;
	        if (i >= lockGold.Count) return lockGold[lockGold.Count - 1];
	        return lockGold[i];
	    }
	}

	[System.Serializable]
	public class Level
	{
	    public int holeNo;
	    public int seed;
	    public int numHits;
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(LevelData))]
	public class LevelDataEditor : Editor
	{
	    private int currentlyEditingLevelNo0 = -1;
	    private int currentlyEditingSeed;
	    private int currentlyEditingLevelNo1 = -1;
	    private int currentlyEditingNumHits;
	    private int currentlyEditingLevelNo2 = -1;

	    public override void OnInspectorGUI()
	    {
	        LevelData levelData = (LevelData)target;
	        if (levelData.levels.Count == 0)
	        {
	            if (GUILayout.Button("Fill default values"))
	            {
	                levelData.levels = new List<Level>();
	                for (int i = 0; i < 250; i++)
	                {
	                    LevelGenerator.GetLevelInfo(i, i, out int noOfColumns, out int noOfRows);
	                    levelData.levels.Add(new Level()
	                    {
	                        holeNo = i,
	                        seed = i,
	                        numHits = LevelData.GetDefaultNumHits(noOfColumns, noOfRows)
	                    });
	                }
	            }
	        }
	        else
	        {
	            currentlyEditingLevelNo0 = EditorGUILayout.IntField("Level", currentlyEditingLevelNo0);
	            currentlyEditingSeed = EditorGUILayout.IntField("Seed", currentlyEditingSeed);
	            if (GUILayout.Button($"Change seed of level {currentlyEditingLevelNo0} to {currentlyEditingSeed}"))
	            {
	                levelData.levels[currentlyEditingLevelNo0].seed = currentlyEditingSeed;
	            }
	            currentlyEditingLevelNo1 = EditorGUILayout.IntField("Level", currentlyEditingLevelNo1);
	            currentlyEditingNumHits = EditorGUILayout.IntField("Num hits", currentlyEditingNumHits);
	            if (GUILayout.Button($"Change num hits of level {currentlyEditingLevelNo1} to {currentlyEditingNumHits}"))
	            {
	                levelData.levels[currentlyEditingLevelNo1].numHits = currentlyEditingNumHits;
	            }
	            currentlyEditingLevelNo2 = EditorGUILayout.IntField("Level", currentlyEditingLevelNo2);
	            if (GUILayout.Button($"Change seed of level {currentlyEditingLevelNo2} to random"))
	            {
	                levelData.levels[currentlyEditingLevelNo2].seed = UnityEngine.Random.Range(0, int.MaxValue);
	            }
	        }

	        base.OnInspectorGUI();

	        serializedObject.ApplyModifiedProperties();
	    }
	}
	#endif

}
