using UnityEngine;
using System.Collections.Generic;
using System;
using Collection.Controls;

namespace Games.Golfinity
{
	public class LevelGenerator : MonoBehaviour
	{
	    public LevelData levelData;
	    public GameObject[] holesUp;
	    public GameObject[] holesLeft;
	    public GameObject[] terrainAll;
	    [HideInInspector] public List<GameObject> terrainAllHeight;
	    [HideInInspector] public List<GameObject> terrainNormal;
	    [HideInInspector] public List<GameObject> terrainShort;
	    public GameObject coin;
	    public const float tileWidth = 8f;
	    public const int numLevelsPerColor = 20;

	    public Color[] skyColors;
	    public Color[] terrainColors;
	    public Color[] mudColors;
	    public Color skyColor;
	    public Color terrainColor;
	    public Color mudColor;

	    public static int CurrentHoleNo = -1;
	    public static int NumHits = -1;
	    private static int Seed = -1;
	    public static int NumCoinsCollected;
	    public static int NumCoins;
	    private static System.Random Rand;

	    private bool haveGrouped;

	    private List<Terrain.Pack> lastPacks;
	    public void GroupTerrain(List<Terrain.Pack> packs)
	    {
	        if (lastPacks != null && lastPacks.Count == packs.Count)
	        {
	            bool isDifferent = false;
	            for (int i = 0, len = lastPacks.Count; i < len; i++)
	            {
	                if (lastPacks[i] != packs[i])
	                {
	                    isDifferent = true;
	                    break;
	                }
	            }
	            if (!isDifferent) return;
	            lastPacks = packs;
	        }
	        terrainAllHeight = new List<GameObject>();
	        terrainNormal = new List<GameObject>();
	        terrainShort = new List<GameObject>();
	        foreach (GameObject go in terrainAll)
	        {
	            Terrain terrain = go.GetComponent<Terrain>();
	            if (!terrain.IsUnlocked(packs)) continue;

	            terrainAllHeight.Add(go);
	            if (terrain.type == Terrain.Type.Normal || terrain.type == Terrain.Type.Short)
	            {
	                terrainNormal.Add(go);
	            }
	            if (terrain.type == Terrain.Type.Short)
	            {
	                terrainShort.Add(go);
	            }
	        }
	    }

	    void Update()
	    {
	        if (TaloketoInputManager.GetButtonDown("Debug Reset"))
	        {
	            ResetLevel();
	        }
	    }

	    public void OpenLevel(int holeNo)
	    {
	        CurrentHoleNo = holeNo;
	        NumHits = GetNumHits(holeNo);
	        ResetLevel();
	    }

	    public void ResetLevel()
	    {
	        Game.noOfStrokesSinceBeginningOfLevel = 0;
	        NumCoinsCollected = 0;
	        NumCoins = 0;
	        SetColors(CurrentHoleNo);
	        DeleteLevel();
	        GenerateLevel(CurrentHoleNo);
	    }

	    void DeleteLevel()
	    {
	        foreach (Transform child in transform)
	        {
	            Destroy(child.gameObject);
	        }
	    }

	    public int noOfColumns = 0;
	    public int noOfRows = 0;
	    public int noOfCoins = 0;
	    private int holeTile = 0;
	    private HashSet<Vector2Int> posToSkip = new HashSet<Vector2Int>();

	    public static void GetLevelInfo(int holeNo, int seed, out int noOfColumns, out int noOfRows)
	    {
	        SetSeed(seed);

	        noOfColumns = Mathf.Min(4 + 2 * GetRandomIntSeed(0, 2 + holeNo / 200), 20);
	        noOfRows = Mathf.Min(4 + 2 * GetRandomIntSeed(0, 2 + holeNo / 100), 40);
	    }
	    public void GenerateLevel(int holeNo)
	    {
	        int packIndex = (holeNo / numLevelsPerColor) % 5;
	        List<Terrain.Pack> packs = new List<Terrain.Pack>();
	        switch (packIndex)
	        {
	            case 0:
	                break;
	            case 1:
	                packs.Add(Terrain.Pack.Muddy);
	                break;
	            case 2:
	                packs.Add(Terrain.Pack.Moving);
	                break;
	            case 3:
	                packs.Add(Terrain.Pack.Varied);
	                break;
	            case 4:
	                packs.Add(Terrain.Pack.Varied);
	                packs.Add(Terrain.Pack.Moving);
	                packs.Add(Terrain.Pack.Muddy);
	                break;
	            default:
	                throw new System.NotImplementedException();
	        }
	        GroupTerrain(packs);

	        int seed = levelData.GetSeed(holeNo);

	        GetLevelInfo(holeNo, seed, out noOfColumns, out noOfRows);

	        SetSeed(seed);
	        noOfCoins = 3;
	        posToSkip.Clear();

	        //decide height of the level depending on how many levels the player beat
	        if (holeNo < 11)
	        {
	            noOfRows = 2;
	            noOfCoins = 1;
	        }
	        else if (holeNo < 31)
	        {
	            noOfRows = 2 + 2 * GetRandomIntSeed(0, 2);
	            noOfCoins = GetRandomIntSeed(2, 4);
	        }
	        else if (holeNo < 51)
	        {
	            noOfRows = 2 + 2 * GetRandomIntSeed(0, 3);
	            noOfCoins = GetRandomIntSeed(2, 4);
	        }

	        //decide where the hole tile will be on the x-axis. (on y-axis it's always on the top row)
	        holeTile = GetRandomIntSeed(0, noOfColumns);

	        Vector2Int[] coinPos = new Vector2Int[noOfCoins];
	        Vector2Int holePos = new Vector2Int(holeTile, noOfRows - 1);
	        Vector2Int ballPos = new Vector2Int(0, 1);
	        for (int i = 0; i < noOfCoins; i++)
	        {
	            Vector2Int newPos = ballPos;
	            int count = 0;
	            while (count < 10)
	            {
	                if (newPos != holePos && newPos != ballPos)
	                {
	                    bool foundSamePos = false;
	                    for (int j = 0; j < i; j++)
	                    {
	                        if (newPos == coinPos[j])
	                        {
	                            foundSamePos = true;
	                            break;
	                        }
	                    }
	                    if (!foundSamePos) break;
	                }
	                newPos = new Vector2Int(GetRandomIntSeed(0, noOfColumns), 1 + 2 * GetRandomIntSeed(0, noOfRows / 2));
	                count++;
	            }
	            coinPos[i] = newPos;
	        }

	        //decide if hole can be mounted on a wall
	        if (holeNo > 100 && Game.holesOnWalls)
	        {
	            holeTile = GetRandomIntSeed(-1, noOfColumns + 1);
	        }
	        //make sure ihole is not on the tile where the ball will spawn
	        while (noOfRows == 2 && holeTile == 0)
	        {
	            holeTile = GetRandomIntSeed(0, noOfColumns);
	        }

	        Dictionary<int, Vector2Int> emptyTiles = new Dictionary<int, Vector2Int>();
	        for (int j = -1; j < noOfRows + 1; j++)
	        {
	            //decide which tiles will be empty on this row for the ball to be able to go up. two values can be the same, which would result in only one empty tile
	            int emptyTile = GetRandomIntSeed(0, noOfColumns);
	            int emptyTile2 = GetRandomIntSeed(0, noOfColumns);

	            if (holeTile == -1 || holeTile == noOfColumns)
	            {
	                while (j == noOfRows - 2 && (emptyTile == holeTile - 1 || emptyTile == holeTile + 1))
	                {
	                    emptyTile = GetRandomIntSeed(0, noOfColumns);
	                }
	                while (j == noOfRows - 2 && (emptyTile2 == holeTile - 1 || emptyTile2 == holeTile + 1))
	                {
	                    emptyTile2 = GetRandomIntSeed(0, noOfColumns);
	                }
	            }
	            else
	            {
	                while (j == noOfRows - 2 && emptyTile == holeTile)
	                {
	                    emptyTile = GetRandomIntSeed(0, noOfColumns);
	                }
	                while (j == noOfRows - 2 && emptyTile2 == holeTile)
	                {
	                    emptyTile2 = GetRandomIntSeed(0, noOfColumns);
	                }
	            }
	            emptyTiles.Add(j, new Vector2Int(emptyTile, emptyTile2));
	            if (j != -1 && j != noOfRows)
	            {
	                posToSkip.Add(new Vector2Int(emptyTile, j));
	                if (emptyTile != emptyTile2) posToSkip.Add(new Vector2Int(emptyTile2, j));
	            }
	        }

	        bool createdHole = false;

	        for (int j = -1; j < noOfRows + 1; j++)
	        {
	            int emptyTile = emptyTiles[j].x;
	            int emptyTile2 = emptyTiles[j].y;

	            for (int i = -1; i < noOfColumns + 1; i++)
	            {
	                //create walls around map
	                if (i == -1 || i == noOfColumns || j == -1 || j == noOfRows)
	                {
	                    //if hole is mounted on a wall, create it
	                    if (j == noOfRows - 1 && holeTile == i)
	                    {
	                        GameObject go = Create(holesLeft[GetRandomIntSeed(0, holesLeft.Length)], i, j, 0f);
	                        if (i == -1)
	                        {
	                            go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
	                        }
	                        createdHole = true;
	                    }
	                    //if a wall on a level where ball will be around, create a terrain and rotate it 90 degrees for creating interesting side walls
	                    else if (j % 2 == 1)
	                    {
	                        if (i == -1)
	                        {
	                            TryCreate(terrainShort[GetRandomIntSeed(0, terrainShort.Count)], i, j, -90);
	                        }
	                        else
	                        {
	                            TryCreate(terrainShort[GetRandomIntSeed(0, terrainShort.Count)], i, j, 90);
	                        }
	                    }
	                    //create normal wall
	                    else
	                    {
	                        TryCreate(terrainAllHeight[0], i, j);
	                    }
	                }
	                //put the ball
	                else if (i == 0 && j == 1)
	                {
	                    Vector3 p = pos(i, j);
	                    p.z = -2f;
	                    GolfBall.instance.transform.position = p;
	                    GolfBall.instance.startPos = GolfBall.instance.transform.position;
	                }
	                //create the actual terrain in which the ball will stand on
	                else if (j % 2 == 0)
	                {
	                    if (i == 0 && j == 0)
	                    {
	                        GameObject terrain = null;
	                        while (terrain == null || terrain.GetComponent<Terrain>().pack == Terrain.Pack.Muddy)
	                        {
	                            terrain = terrainShort[GetRandomIntSeed(0, terrainShort.Count)];
	                        }
	                        TryCreate(terrain, i, j);
	                    }
	                    //empty tile so that the ball can go up a level
	                    if (i == emptyTile || i == emptyTile2)
	                    {

	                    }
	                    //create hole
	                    else if (j == noOfRows - 2 && i == holeTile)
	                    {
	                        Create(holesUp[GetRandomIntSeed(0, holesUp.Length)], i, j, 0f);
	                        createdHole = true;
	                    }
	                    //create only short walls near walls, so it will work nicely with the "interesting" side walls
	                    else if (i == 0 || i == noOfColumns - 1)
	                    {
	                        TryCreate(terrainShort[GetRandomIntSeed(0, terrainShort.Count)], i, j);
	                    }
	                    //for all the other terrain, create them one by one, with specific weights for wall types with different heights
	                    else
	                    {
	                        if (j > 0 && GetPercentChanceSeed(30))
	                        {
	                            TryCreate(terrainShort[GetRandomIntSeed(0, terrainShort.Count)], i, j, 180);
	                        }
	                        else if (i == emptyTile - 1 || i == emptyTile + 1 || i == emptyTile2 - 1 || i == emptyTile2 + 1)
	                        {
	                            if ((i == emptyTile + 1 && i != emptyTile2 - 1) || (i == emptyTile2 + 1 && i != emptyTile - 1))
	                            {
	                                if (GetPercentChanceSeed(30))
	                                {
	                                    TryCreate(terrainShort[GetRandomIntSeed(0, terrainShort.Count)], i, j, 90);
	                                }
	                                else
	                                {
	                                    TryCreate(terrainShort[GetRandomIntSeed(0, terrainShort.Count)], i, j);
	                                }
	                            }
	                            else if ((i == emptyTile - 1 && i != emptyTile2 + 1) || (i == emptyTile2 - 1 && i != emptyTile + 1))
	                            {
	                                if (GetPercentChanceSeed(30))
	                                {
	                                    TryCreate(terrainShort[GetRandomIntSeed(0, terrainShort.Count)], i, j, -90);
	                                }
	                                else
	                                {
	                                    TryCreate(terrainShort[GetRandomIntSeed(0, terrainShort.Count)], i, j);
	                                }
	                            }
	                            else
	                            {
	                                TryCreate(terrainShort[GetRandomIntSeed(0, terrainShort.Count)], i, j);
	                            }
	                        }
	                        else
	                        {
	                            TryCreate(terrainAllHeight[GetRandomIntSeed(0, terrainAllHeight.Count)], i, j);
	                        }
	                    }
	                }
	            }
	        }

	        for (int i = 0; i < noOfCoins; i++)
	        {
	            if (coinPos[i].x == 0 && coinPos[i].y == 1) continue;
	            bool skip = false;
	            for (int j = i + 1; j < noOfCoins; j++)
	            {
	                if (coinPos[i].x == coinPos[j].x && coinPos[i].y == coinPos[j].y)
	                {
	                    skip = true;
	                    break;
	                }
	            }
	            if (!skip)
	            {
	                NumCoins++;
	                Create(coin, coinPos[i].x, coinPos[i].y, 0);
	            }
	        }

	        //create huge side walls so that it will look like the boundary walls look infinite
	        Create(terrainAll[0], -6, noOfRows / 2, 0).transform.localScale = new Vector3(9, noOfRows + 10, 1);
	        Create(terrainAll[0], noOfColumns + 5, noOfRows / 2, 0).transform.localScale = new Vector3(9, noOfRows + 10, 1);
	        Create(terrainAll[0], noOfColumns / 2, -6, 0).transform.localScale = new Vector3(noOfColumns + 10, 9, 1);
	        Create(terrainAll[0], noOfColumns / 2, noOfRows + 5, 0).transform.localScale = new Vector3(noOfColumns + 10, 9, 1);

	        if (!createdHole)
	        {
	            Debug.Log("didn't create hole: " + holeTile);
	        }
	    }

	    GameObject TryCreate(GameObject prefab, int i, int j, float angle = 0)
	    {
	        Terrain terrainPrefab = prefab.GetComponent<Terrain>();
	        if (terrainPrefab != null && terrainPrefab.size == Terrain.Size.ThreeXThree)
	        {
	            bool shouldSkip =
	                // check if it is on the last floor
	                j == noOfRows - 1 || j == noOfRows - 2
	                // check if it overlaps with the hole tile
	                || ((j == noOfRows - 3 || j == noOfRows - 4) && (holeTile >= i && holeTile <= i + 2))
	                // check if there is enough place to the right
	                || i >= noOfColumns - 2
	                // check if any of the positions should be skipped for whatever reason
	                || posToSkip.Contains(new Vector2Int(i + 0, j + 0))
	                || posToSkip.Contains(new Vector2Int(i + 0, j + 1))
	                || posToSkip.Contains(new Vector2Int(i + 0, j + 2))
	                || posToSkip.Contains(new Vector2Int(i + 1, j + 0))
	                || posToSkip.Contains(new Vector2Int(i + 1, j + 1))
	                || posToSkip.Contains(new Vector2Int(i + 1, j + 2))
	                || posToSkip.Contains(new Vector2Int(i + 2, j + 0))
	                || posToSkip.Contains(new Vector2Int(i + 2, j + 1))
	                || posToSkip.Contains(new Vector2Int(i + 2, j + 2));

	            if (shouldSkip)
	            {
	                return TryCreate(terrainShort[GetRandomIntSeed(0, terrainShort.Count)], i, j, angle);
	            }
	            else
	            {
	                posToSkip.Add(new Vector2Int(i + 0, j + 0));
	                posToSkip.Add(new Vector2Int(i + 0, j + 1));
	                posToSkip.Add(new Vector2Int(i + 0, j + 2));
	                posToSkip.Add(new Vector2Int(i + 1, j + 0));
	                posToSkip.Add(new Vector2Int(i + 1, j + 1));
	                posToSkip.Add(new Vector2Int(i + 1, j + 2));
	                posToSkip.Add(new Vector2Int(i + 2, j + 0));
	                posToSkip.Add(new Vector2Int(i + 2, j + 1));
	                posToSkip.Add(new Vector2Int(i + 2, j + 2));
	                return Create(prefab, i + 1, j + 1, angle);
	            }
	        }

	        if (!posToSkip.Contains(new Vector2Int(i, j)))
	            return Create(prefab, i, j, angle);
	        else return null;
	    }

	    GameObject Create(GameObject prefab, int i, int j, float angle)
	    {
	        GameObject go = Instantiate(prefab, pos(i, j), Quaternion.identity) as GameObject;
	        go.transform.SetParent(transform);
	        go.transform.eulerAngles = Vector3.forward * angle;
	        return go;
	    }

	    Vector3 pos(int i, int j)
	    {
	        return new Vector3(i * tileWidth, j * tileWidth, 0f);
	    }

	    public void SetColors(int holeNo)
	    {
	        skyColor = skyColors[(holeNo / numLevelsPerColor) % skyColors.Length];
	        terrainColor = terrainColors[(holeNo / numLevelsPerColor) % terrainColors.Length];
	        mudColor = mudColors[(holeNo / numLevelsPerColor) % mudColors.Length];
	        Game.cam.backgroundColor = skyColor;
	    }

	    public static void SetSeed(int seed)
	    {
	        Seed = seed;
	        Rand = new System.Random(Seed);
	    }

	    public static int GetRandomIntSeed(int min, int max)
	    {
	        return Rand.Next(min, max);
	    }

	    public static bool GetPercentChanceSeed(int percent)
	    {
	        return Rand.Next(0, 10000) <= 100 * percent;
	    }

	    public int GetNumHits(int holeNo)
	    {
	        return levelData.GetNumHits(holeNo);
	    }

	    public int GetNumStars(int holeNo, int noOfStrokesSinceBeginningOfLevel)
	    {
	        int numHits = (holeNo != CurrentHoleNo) ? GetNumHits(holeNo) : NumHits;
	        if (noOfStrokesSinceBeginningOfLevel <= numHits) return 3;
	        if (noOfStrokesSinceBeginningOfLevel <= numHits * 3 / 2) return 2;
	        return 1;
	    }

	    public int GetNumGoldTotalToUnlock(int lockIndex)
	    {
	        return levelData.NumGoldTotalToUnlock(lockIndex);
	    }

	    public int GetNumGoldToUnlock(int lockIndex)
	    {
	        // auto-unlock if you've already played a level after
	        if (Game.GetUnbeatenLastHole() > lockIndex * numLevelsPerColor + 1) return 0;
	        return PlayerPrefs.GetInt($"lock{lockIndex}", GetNumGoldTotalToUnlock(lockIndex));
	    }

	    public bool PayGoldToUnlock(int lockIndex)
	    {
	        int numGoldLeft = PlayerPrefs.GetInt($"lock{lockIndex}", GetNumGoldTotalToUnlock(lockIndex));
	        if (numGoldLeft == 0 || Game.gold == 0) return false;
	        numGoldLeft--;
	        Game.gold--;
	        PlayerPrefs.SetInt($"lock{lockIndex}", numGoldLeft);
	        PlayerPrefs.SetInt("gold", Game.gold);
	        return true;
	    }
	}

}
