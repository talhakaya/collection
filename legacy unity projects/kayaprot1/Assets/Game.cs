using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
    public static Game instance;
	public static float time;
	public static float timeSpeed;
    public static float dt;
	public static Color color0 = new Color(183 / 255f, 162 / 255f, 130 / 255f);
	public static Color color1 = new Color(193 / 255f, 106 / 255f, 68 / 255f);
	public static Color color2 = new Color(170 / 255f, 68 / 255f, 68 / 255f);
	public static Color color3 = new Color(189 / 255f, 68 / 255f, 193 / 255f);
	public static Color color4 = new Color(110 / 255f, 64 / 255f, 183 / 255f);
	public static Color[] colors = new Color[]{color0, color1, color2, color3, color4};
	public static bool input;
	private static bool inputOld;
	public static bool inputDown;
	public static bool inputUp;
    public static Vector3 shadowVector;
    public static float DefaultShadowDistance = 0.3f;
    public GameObject[] tilePrefab;
    public GameObject[] wallPrefab;
    public GameObject[] housePrefab;
    public GameObject entrancePrefab;
    public GameObject exitPrefab;
    public static GameObject[,] tiles;
    public static TileType[,] tileTypes;
    public static int numRows;
    public static int numCols;
    public Player player;
    private Rigidbody2D body;
    public Transform wallParent;
    public Transform tileParent;
    public Transform houseParent;

    void Start () {
        instance = this;
        shadowVector = Vector3.down + Vector3.left;
        timeSpeed = 1f;
        body = GetComponent<Rigidbody2D>();
        GenerateWorld();
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.G)) {
            GenerateWorld();
        }

        dt = Time.deltaTime;
		time += dt;

		MousePosition.get = Camera.main.ScreenToWorldPoint (Input.mousePosition) + Vector3.forward;
		MousePosition.x = MousePosition.get.x;
		MousePosition.y = MousePosition.get.y;

		input = Input.GetMouseButton (0);
		inputDown = input && !inputOld;
		inputUp = !input && inputOld;

		inputOld = input;

        Vector2 vel = player.transform.position - transform.position;
        float velLength = Geometry.lengthOfVector2(vel);
        if (velLength < 10f) {
            body.velocity = vel * 2f;
        }
        else {
            body.velocity = Geometry.normalizeVector2(vel, 20f);
        }
    }

    public enum TileType { None, Normal, House, Wall, Entrance, Exit }
    void GenerateWorld() {
        if (tiles != null) {
            for (int i = 1; i < numRows - 1; i++) {
                for (int j = 1; j < numCols - 1; j++) {
                    if (tiles[i, j] != null) {
                        Destroy(tiles[i, j]);
                    }
                }
            }
        }

        numRows = Random.Range(30, 60);
        numCols = Random.Range(30, 60);
        int numBranchTries = Random.Range(10, 20);
        tileTypes = new TileType[numRows, numCols];
        int iCur = numRows / 2;
        int jCur = numCols / 2;
        //mark normal
        for (int k = 0; k < numBranchTries; k++) {
            iCur = numRows / 2;
            jCur = numCols / 2;
            int numShittyTries = 0;
            while (true) {
                if (tileTypes[iCur, jCur] == TileType.Normal) {
                    numShittyTries++;
                }
                else {
                    tileTypes[iCur, jCur] = TileType.Normal;
                }
                if (numShittyTries > 100 || !(iCur > 1 && jCur > 1 && iCur < numRows - 2 && jCur < numCols - 2)) {
                    break;
                }
                else {
                    float r = Random.value;
                    if (r < 0.25f) {
                        iCur++;
                    }
                    else if (r < 0.5f) {
                        iCur--;
                    }
                    else if (r < 0.75f) {
                        jCur++;
                    }
                    else {
                        jCur--;
                    }
                }
            }
            //iCur = Random.Range(2, numRows - 2);
            //jCur = Random.Range(2, numCols - 2);
        }
        //mark house
        bool goodLocationForHouse = false;
        int iHouseStart = 0;
        int jHouseStart = 0;
        int iHouseEnd = 0;
        int jHouseEnd = 0;
        while (!goodLocationForHouse) {
            iHouseStart = Random.Range(10, numRows - 10);
            jHouseStart = Random.Range(10, numCols - 10);
            iHouseEnd = iHouseStart + Random.Range(3, 10);
            jHouseEnd = jHouseStart + Random.Range(3, 10);
            for (int i = iHouseStart; i < iHouseEnd; i++) {
                for (int j = jHouseStart; j < jHouseEnd; j++) {
                    if (tileTypes[i, j] == TileType.Normal) {
                        goodLocationForHouse = true;
                        break;
                    }
                }
            }
        }
        for (int i = iHouseStart; i < iHouseEnd; i++) {
            for (int j = jHouseStart; j < jHouseEnd; j++) {
                tileTypes[i, j] = TileType.House;
            }
        }
        //mark entrance & exit
        List<Pint> edgePoints = new List<Pint>();
        for (int i = 1; i < numRows - 1; i++) {
            for (int j = 1; j < numCols - 1; j++) {
                if (tileTypes[i, j] == TileType.None && (
                    tileTypes[i + 1, j] == TileType.Normal || tileTypes[i + 1, j] == TileType.House ||
                    tileTypes[i - 1, j] == TileType.Normal || tileTypes[i - 1, j] == TileType.House ||
                    tileTypes[i, j - 1] == TileType.Normal || tileTypes[i, j - 1] == TileType.House ||
                    tileTypes[i, j + 1] == TileType.Normal || tileTypes[i, j + 1] == TileType.House
                )) {
                    tileTypes[i, j] = TileType.Wall;
                    if (Geometry.lengthOfVector2(new Vector2(i - numRows / 2f, j - numCols - 2f)) > (numRows + numCols) / 4f) {
                        edgePoints.Add(new Pint(i, j));
                    }
                }
            }
        }
        int entrance = Random.Range(0, edgePoints.Count);
        int exit = Random.Range(0, edgePoints.Count);
        while (exit == entrance) {
            exit = Random.Range(0, edgePoints.Count);
        }
        tileTypes[edgePoints[entrance].x, edgePoints[entrance].y] = TileType.Entrance;
        tileTypes[edgePoints[exit].x, edgePoints[exit].y] = TileType.Exit;
        //mark wall
        for (int i = 1; i < numRows - 1; i++) {
            for (int j = 1; j < numCols - 1; j++) {
                if (tileTypes[i, j] == TileType.None && (
                    tileTypes[i + 1, j] == TileType.Normal || tileTypes[i + 1, j] == TileType.House || tileTypes[i + 1, j] == TileType.Entrance || tileTypes[i + 1, j] == TileType.Exit ||
                    tileTypes[i - 1, j] == TileType.Normal || tileTypes[i - 1, j] == TileType.House || tileTypes[i - 1, j] == TileType.Entrance || tileTypes[i - 1, j] == TileType.Exit ||
                    tileTypes[i, j - 1] == TileType.Normal || tileTypes[i, j - 1] == TileType.House || tileTypes[i, j - 1] == TileType.Entrance || tileTypes[i, j - 1] == TileType.Exit ||
                    tileTypes[i, j + 1] == TileType.Normal || tileTypes[i, j + 1] == TileType.House || tileTypes[i, j + 1] == TileType.Entrance || tileTypes[i, j + 1] == TileType.Exit
                    )) {
                    tileTypes[i, j] = TileType.Wall;
                }
            }
        }
        //create objects
        tiles = new GameObject[numRows, numCols];
        for (int i = 1; i < numRows - 1; i++) {
            for (int j = 1; j < numCols - 1; j++) {
                if (tileTypes[i, j] == TileType.Wall) {
                    tiles[i, j] = Instantiate(wallPrefab[Random.Range(0, wallPrefab.Length)], new Vector3(i, j, 0f), Quaternion.identity);
                    tiles[i, j].transform.SetParent(wallParent);
                }
                else if (tileTypes[i, j] == TileType.Normal) {
                    tiles[i, j] = Instantiate(tilePrefab[Random.Range(0, tilePrefab.Length)], new Vector3(i, j, 0f), Quaternion.identity);
                    tiles[i, j].transform.SetParent(tileParent);
                }
                else if (tileTypes[i, j] == TileType.House) {
                    tiles[i, j] = Instantiate(housePrefab[Random.Range(0, housePrefab.Length)], new Vector3(i, j, 0f), Quaternion.identity);
                    tiles[i, j].transform.SetParent(houseParent);
                }
                else if (tileTypes[i, j] == TileType.Entrance) {
                    tiles[i, j] = Instantiate(entrancePrefab, new Vector3(i, j, 0f), Quaternion.identity);
                    if (tileTypes[i + 1, j] == TileType.Wall && tileTypes[i - 1, j] != TileType.None && tileTypes[i - 1, j] != TileType.Wall) {
                        tiles[i, j].transform.eulerAngles = new Vector3(0f, 0f, 180f);
                    }
                    else if (tileTypes[i, j + 1] == TileType.Wall && tileTypes[i, j - 1] != TileType.None && tileTypes[i, j - 1] != TileType.Wall) {
                        tiles[i, j].transform.eulerAngles = new Vector3(0f, 0f, 270f);
                    }
                    else if (tileTypes[i, j - 1] == TileType.Wall && tileTypes[i, j + 1] != TileType.None && tileTypes[i, j + 1] != TileType.Wall) {
                        tiles[i, j].transform.eulerAngles = new Vector3(0f, 0f, 90f);
                    }
                    player.transform.position = new Vector3(i, j, player.transform.position.z);
                }
                else if (tileTypes[i, j] == TileType.Exit) {
                    tiles[i, j] = Instantiate(exitPrefab, new Vector3(i, j, 0f), Quaternion.identity);
                    if (tileTypes[i - 1, j] == TileType.Wall && tileTypes[i + 1, j] != TileType.None && tileTypes[i + 1, j] != TileType.Wall) {
                        tiles[i, j].transform.eulerAngles = new Vector3(0f, 0f, 180f);
                    }
                    else if (tileTypes[i, j - 1] == TileType.Wall && tileTypes[i, j + 1] != TileType.None && tileTypes[i, j + 1] != TileType.Wall) {
                        tiles[i, j].transform.eulerAngles = new Vector3(0f, 0f, 270f);
                    }
                    else if (tileTypes[i, j + 1] == TileType.Wall && tileTypes[i, j - 1] != TileType.None && tileTypes[i, j - 1] != TileType.Wall) {
                        tiles[i, j].transform.eulerAngles = new Vector3(0f, 0f, 90f);
                    }
                }
            }
        }
    }
}

public class Pint {
    public int x;
    public int y;
    public Pint(int x, int y) {
        this.x = x;
        this.y = y;
    }
}
