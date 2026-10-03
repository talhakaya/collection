using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Collection.Controls;

namespace Games.KayabrosPrototype0
{
	public class Game : MonoBehaviour {
	    public const float levelWidth = 32;
	    public const float levelHeight = 32;
	    public const int numColumns = 4;
	    public const int numRows = 4;
	    public static float roomWidth = levelWidth / numColumns;
	    public static float roomHeight = levelHeight / numRows;
	    public static Game instance;
	    public static float time;
	    public static float timeSpeed;
	    public static float dt;
	    public static float rhythm;
	    public static bool failed;
	    public static float failTimer;
	    public static Color color0 = new Color(183 / 255f, 162 / 255f, 130 / 255f);
	    public static Color color1 = new Color(193 / 255f, 106 / 255f, 68 / 255f);
	    public static Color color2 = new Color(170 / 255f, 68 / 255f, 68 / 255f);
	    public static Color color3 = new Color(189 / 255f, 68 / 255f, 193 / 255f);
	    public static Color color4 = new Color(110 / 255f, 64 / 255f, 183 / 255f);
	    public static Color[] colors = new Color[] { color0, color1, color2, color3, color4 };
	    public static Vector3 shadowVector;
	    public static float DefaultShadowDistance = 0.3f;
	    public Rigidbody2D body;
	    public Player player;
	    public Exit exit;
	    public GameObject moneyCase;
	    public GameObject enemyPrefab;
	    public GameObject wallPrefab;

	    void Start() {
	        instance = this;
	        shadowVector = Vector3.down + Vector3.left;
	        failed = false;
	        failTimer = 0f;
	        timeSpeed = 1f;
	        GenerateLevel();
	    }

	    void Update() {
	        Vector2 vel = player.transform.position - transform.position;
	        float velLength = Geometry.lengthOfVector2(vel);
	        if (velLength < 5f) {
	            body.linearVelocity = vel * 2f;
	        }
	        else {
	            body.linearVelocity = Geometry.normalizeVector2(vel, 10f);
	        }
	        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), 5f * Time.deltaTime);

			// In the collection: the Escape-quit is gone (the collection has its own exit).

	        timeSpeed = failed ? 0f : 1f;
	        dt = Time.deltaTime * timeSpeed;
	        time += dt;
	        if (time % 4f < 2f) {
	            rhythm = Easing.SineEaseOut(time % 4f, 0f, 1f, 2f);
	        }
	        else {
	            rhythm = Easing.SineEaseOut(4f - time % 4f, 0f, 1f, 2f);
	        }

	        if (TaloketoInputManager.GetButton("Fire3")) {
	            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	        }

	        if (failed) {
	            failTimer += Time.deltaTime;
	            if (failTimer <= 1f) {
	                transform.position += new Vector3(0f, 0f, -10f * Time.deltaTime);
	            }
	        }
	        MousePosition.get = TaloketoInputManager.mousePosition;
	        MousePosition.get.z = 10;
	        MousePosition.get = Camera.main.ScreenToWorldPoint(MousePosition.get);
	        MousePosition.get.z = 0f;
	        // In the collection: with a gamepad the right stick aims where the mouse did.
	        MousePosition.get = Collection.Controls.PortHelpers.AimPoint(MousePosition.get, player.transform.position, 3f);
	        MousePosition.x = MousePosition.get.x;
	        MousePosition.y = MousePosition.get.y;
	    }

	    void GenerateLevel() {
	        int playerX = Random.Range(0, numColumns);
	        int playerY = Random.Range(0, numRows);
	        int caseX = Random.Range(0, numColumns);
	        int caseY = Random.Range(0, numRows);
	        int exitX = Random.Range(0, numColumns);
	        int exitY = Random.Range(0, numRows);
	        GameObject w = Instantiate(wallPrefab);
	        w.transform.localScale = new Vector3(levelWidth, 1f, wallPrefab.transform.localScale.z);
	        w.transform.localPosition = new Vector3(0f, levelHeight * 0.5f, wallPrefab.transform.localPosition.z);
	        w = Instantiate(wallPrefab);
	        w.transform.localScale = new Vector3(levelWidth, 1f, wallPrefab.transform.localScale.z);
	        w.transform.localPosition = new Vector3(0f, -levelHeight * 0.5f, wallPrefab.transform.localPosition.z);
	        w = Instantiate(wallPrefab);
	        w.transform.localScale = new Vector3(1f, levelHeight, wallPrefab.transform.localScale.z);
	        w.transform.localPosition = new Vector3(-levelWidth * 0.5f, 0f, wallPrefab.transform.localPosition.z);
	        w = Instantiate(wallPrefab);
	        w.transform.localScale = new Vector3(1f, levelHeight, wallPrefab.transform.localScale.z);
	        w.transform.localPosition = new Vector3(levelWidth * 0.5f, 0f, wallPrefab.transform.localPosition.z);

	        for (int i = 0; i < numColumns; i++) {
	            for (int j = 0; j < numRows; j++) {
	                Vector3 pos = new Vector3(-levelWidth * 0.5f + (i + 0.5f) * roomWidth, -levelHeight * 0.5f + (j + 0.5f) * roomHeight, wallPrefab.transform.localPosition.z);
	                const float wallExistProbability = 0.6f;
	                if (Random.value < wallExistProbability) {
	                    w = Instantiate(wallPrefab, pos + new Vector3(-roomWidth * 0.5f, 0f, 0f), Quaternion.identity);
	                    w.transform.localScale = new Vector3(1f, roomHeight, wallPrefab.transform.localScale.z);
	                }
	                if (Random.value < wallExistProbability) {
	                    w = Instantiate(wallPrefab, pos + new Vector3(roomWidth * 0.5f, 0f, 0f), Quaternion.identity);
	                    w.transform.localScale = new Vector3(1f, roomHeight, wallPrefab.transform.localScale.z);
	                }
	                if (Random.value < wallExistProbability) {
	                    w = Instantiate(wallPrefab, pos + new Vector3(0f, -roomHeight * 0.5f, 0f), Quaternion.identity);
	                    w.transform.localScale = new Vector3(roomWidth, 1f, wallPrefab.transform.localScale.z);
	                }
	                if (Random.value < wallExistProbability) {
	                    w = Instantiate(wallPrefab, pos + new Vector3(0f, roomHeight * 0.5f, 0f), Quaternion.identity);
	                    w.transform.localScale = new Vector3(roomWidth, 1f, wallPrefab.transform.localScale.z);
	                }

	                if (playerX == i && playerY == j) {
	                    player.transform.position = new Vector3(pos.x, pos.y, player.transform.position.z);
	                }
	                else {
	                    for (int k = 0, len = Random.Range(0, 2); k < len; k++) {
	                        Instantiate(enemyPrefab, pos + new Vector3(0.3f * Random.Range(-roomWidth, roomWidth), 0.3f * Random.Range(-roomHeight, roomHeight), 0f), Quaternion.identity);
	                    }
	                }

	                if (exitX == i && exitY == j) {
	                    exit.transform.position = new Vector3(pos.x, pos.y, exit.transform.position.z)
	                        + new Vector3((Random.value < 0.5f ? 1f : -1f) * roomWidth * 0.25f, (Random.value < 0.5f ? 1f : -1f) * roomHeight * 0.35f, 0f);
	                }

	                if (caseX == i && caseY == j) {
	                    moneyCase.transform.position = new Vector3(pos.x, pos.y, moneyCase.transform.position.z);
	                }
	            }
	        }
	    }
	}
}
