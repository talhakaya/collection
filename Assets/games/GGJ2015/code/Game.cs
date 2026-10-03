using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.GGJ2015
{
	public class Game : MonoBehaviour {

		public static Game instance;
		public static float time;
		public static float dt;
		public static Color color0 = new Color(183 / 255f, 162 / 255f, 130 / 255f);
		public static Color color1 = new Color(193 / 255f, 106 / 255f, 68 / 255f);
		public static Color color2 = new Color(170 / 255f, 68 / 255f, 68 / 255f);
		public static Color color3 = new Color(189 / 255f, 68 / 255f, 193 / 255f);
		public static Color color4 = new Color(110 / 255f, 64 / 255f, 183 / 255f);
		public static Color[] colors = new Color[5] {color0, color1, color2, color3, color4};
		public static bool input;
		private static bool inputOld;
		public static bool inputDown;
		public static bool inputUp;
		public static float tileWidth = 0.64f;
		public static float tileHeight = 0.64f;
		public static float timeMultiplier = 1f;
		public static bool transition = false;
		public static float transitionTime = 2f;

		public int currentLevel;
		public GameObject[] levels;
		public float[] cameraAngles;
	//	public GameObject lines;
		public GameObject line;
		public TintScript transBg;
		private float transitionCounter;
		private float transitionCounterOld;
		private bool isLevelTransition;
		private bool isRotateTransition;
		private Vector3 levelRotateScale = Vector3.one;
		private int lapCount = 1;

		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
	//		for (int i = -22; i < 22; i++)
	//		{
	//			GameObject go = Instantiate(line, Vector3.forward * 8 + Vector3.up * tileHeight * (i + 0.5f), Quaternion.identity) as GameObject;
	//			go.name = "line";
	//			go.transform.parent = lines.transform;
	//		}
	//		for (int i = -22; i < 22; i++)
	//		{
	//			GameObject go = Instantiate(line, Vector3.forward * 8 + Vector3.right * tileWidth * (i + 0.5f), Quaternion.identity) as GameObject;
	//			go.name = "line";
	//			go.transform.parent = lines.transform;
	//			go.transform.Rotate(Vector3.forward * 90f);
	//		}
			Physics2D.gravity = Vector2.zero;
			levelConfig ();
			transform.Rotate(Vector3.forward * cameraAngles[currentLevel]);

			transBg.gameObject.SetActive (true);
		}

		void Update ()
		{
			if (TaloketoInputManager.GetButton("SpeedUp"))
			{
				timeMultiplier = 5f;
			}
			else
			{
				timeMultiplier = 1f;
			}
			dt = timeMultiplier * Time.deltaTime;
			time += dt;

			MousePosition.get = Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
			MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = TaloketoInputManager.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;

	//		if  (Pivot.instance != null && Pivot.instance.gameObject.activeSelf)
	//		{
	//			levels [currentLevel].transform.position = new Vector3 (Pivot.instance.transform.position.x, Pivot.instance.transform.position.y, levels [currentLevel].transform.position.z);
	//		}

			if (transition)
			{
				transitionCounter += dt;
				if (isLevelTransition)
				{
					if (currentLevel >= 1)
					{
						transform.Rotate(Vector3.forward * (cameraAngles[currentLevel] - cameraAngles[currentLevel - 1]) * dt / transitionTime);
					}
					else
					{
						transform.Rotate(Vector3.forward * (cameraAngles[currentLevel] - cameraAngles[cameraAngles.Length - 1]) * dt / transitionTime);
					}
					if (transitionCounter < transitionTime / 2f)
					{
						Camera.main.orthographicSize = 5f - (transitionCounter / transitionTime * 2f) * 2f;
						transBg.selfColor = new Color(1f, 1f, 1f, (transitionCounter / transitionTime * 2f));
					}
					else
					{
						if (transitionCounterOld < transitionTime / 2f)
						{
							transBg.selfColor = Color.white;
							levelConfig();
						}
						else
						{
							transBg.selfColor = new Color(1f, 1f, 1f, ((transitionTime - transitionCounter) / transitionTime * 2f));
						}
						Camera.main.orthographicSize = 5f - ((transitionTime - transitionCounter) / transitionTime * 2f) * 2f;
					}
				}
				else if (isRotateTransition)
				{
					transform.Rotate(Vector3.forward * 360 * dt / transitionTime * lapCount);
					if (levels[currentLevel].transform.localScale.x - levelRotateScale.x < 0f)
					{
						levels[currentLevel].transform.localScale += Vector3.right * dt / transitionTime * 2f;
					}
					else if (levels[currentLevel].transform.localScale.x - levelRotateScale.x > 0f)
					{
						levels[currentLevel].transform.localScale -= Vector3.right * dt / transitionTime * 2f;
					}
					if (levels[currentLevel].transform.localScale.y - levelRotateScale.y < 0f)
					{
						levels[currentLevel].transform.localScale += Vector3.up * dt / transitionTime * 2f;
					}
					else if (levels[currentLevel].transform.localScale.y - levelRotateScale.y > 0f)
					{
						levels[currentLevel].transform.localScale -= Vector3.up * dt / transitionTime * 2f;
					}

					if (transitionCounter >= transitionTime)
					{
						levels[currentLevel].transform.localScale = levelRotateScale;
					}
				}

				if (transitionCounter >= transitionTime)
				{
					transition = false;
				}

				transitionCounterOld = transitionCounter;
			}
			else
			{
				Camera.main.orthographicSize = 5f;
				transBg.selfColor = new Color(1f, 1f, 1f, 0f);
				isLevelTransition = false;
				isRotateTransition = false;
				if (Player.count > 0 && Player.count == Goal.count && Player.onGoal / 2 == Player.count)
				{
					Player.onGoal = 0;
					Goal.count = 0;
					nextLevel();
				}
				else if (Collection.Controls.PortHelpers.KeyDown(UnityEngine.InputSystem.Key.O))
				{
					Debug.Log ("Player.count: " + Player.count + "  Player.onGoal: " + Player.onGoal + "  Goal.count: " + Goal.count);
				}
			}
		}

		public void nextLevel()
		{
			currentLevel++;
			if (currentLevel >= levels.Length)
			{
				currentLevel = 0;
			}
			isLevelTransition = true;
			transition = true;
			transitionCounter = 0f;
			transitionTime = 2f;
		}

		public void rotateWorld(bool isVertical)
		{
			isRotateTransition = true;
			transition = true;
			transitionCounter = 0f;
			transitionTime = 2f;
			if (isVertical)
			{
				levelRotateScale = new Vector3(levels[currentLevel].transform.localScale.x, -levels[currentLevel].transform.localScale.y, levels[currentLevel].transform.localScale.z);
			}
			else
			{
				levelRotateScale = new Vector3(-levels[currentLevel].transform.localScale.x, levels[currentLevel].transform.localScale.y, levels[currentLevel].transform.localScale.z);
			}
			lapCount *= -1;
		}

		public static void fitInTile(GameObject go)
		{
			go.transform.position = new Vector3 (Mathf.Round (go.transform.position.x / tileWidth) * tileWidth, Mathf.Round (go.transform.position.y / tileHeight) * tileHeight, go.transform.position.z);
		}

		public static void fitInTileHalf(GameObject go)
		{
			Vector3 halfTile = new Vector3 (tileWidth / 2f, tileHeight / 2f, 0f);
			go.transform.position -= halfTile;
			go.transform.position = new Vector3 (Mathf.Round (go.transform.position.x / tileWidth) * tileWidth, Mathf.Round (go.transform.position.y / tileHeight) * tileHeight, go.transform.position.z);
			go.transform.position += halfTile;
		}

		private void levelConfig()
		{
			Player.count = 0;
			for (int i = 0; i < levels.Length; i++)
			{
				levels[i].transform.parent.gameObject.SetActive(i == currentLevel);
				levels[i].SetActive(i == currentLevel);
			}
			Goal.count = 0;
			foreach (Transform child in levels[currentLevel].transform.parent)
			{
				if (child.name == "player")
				{
					Player.count++;
				}
			}
			foreach (Transform child in levels[currentLevel].transform)
			{
				foreach (Transform child2 in child)
				{
					if (child2.name == "goal")
					{
						child2.GetComponent<Goal>().reset();
						Goal.count++;
					}
				}
			}
		}
	}
}
