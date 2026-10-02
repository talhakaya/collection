using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public static float dt;
	public static Game instance;
	public static bool input;
	private static bool inputOld;
	public static bool inputDown;
	public static bool inputUp;
	public static float time = 0f;
	public static int score = 0;
	public static int totalScore = 0;
	public static Color color0 = new Color(183 / 255f, 162 / 255f, 130 / 255f);
	public static Color color1 = new Color(193 / 255f, 106 / 255f, 68 / 255f);
	public static Color color2 = new Color(170 / 255f, 68 / 255f, 68 / 255f);
	public static Color color3 = new Color(189 / 255f, 68 / 255f, 193 / 255f);
	public static Color color4 = new Color(110 / 255f, 64 / 255f, 183 / 255f);
	public static Color[] colors = new Color[5] {color0, color1, color2, color3, color4};
	public GameObject cupcakePrefab;
	public GameObject bananaPrefab;
	public GameObject potatoPrefab;
	public GameObject hamburgerPrefab;
	public GameObject colaPrefab;
	private float foodPeriod = 0f;
	public AudioSource aMusic;
	public AudioSource aHit;
	public AudioSource aEat;
	public GameObject main;
	public GameObject menu;
	public TextTalha scoreText;
	public TextTalha timeText;
	public TextTalha ruleText;
	public TextTalha ruleNoText;
	public static int ruleNo = 0;
	public string[] rules;
	public AudioClip[] eatClips;
	
	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		end ();
	}
	
	void Update ()
	{

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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

		if (main.activeSelf)
		{
			scoreText.text = "Score: " + score;
			timeText.text = "Time: " + Mathf.RoundToInt(time);
			float foodCreation = time % 1f;
			if (foodPeriod < 0.5f && foodCreation >= 0.5f)
			{
				food (Random.Range(-6f, 6f), 10);
			}
			foodPeriod = foodCreation;

			if (time > 20f)
			{
				end();
			}
		}
		else
		{
			if (ruleNo < rules.Length - 1)
			{
				ruleNoText.text = "Rule " + ruleNo + ":";
				ruleText.text = rules[ruleNo];
			}
			else
			{
				ruleNoText.text = "Congratulations! You'll die alone.";
				ruleText.text = "Total Score: " + totalScore;
			}

			if (Input.GetKeyDown(KeyCode.Return) && ruleNo < rules.Length - 1)
			{
				ruleNo++;
				init ();
			}
		}
	}

	void init()
	{
		main.SetActive (true);
		menu.SetActive (false);
		time = 0f;
		score = 0;
		for (int i = 0; i < 10; i++)
		{
			food (Random.Range(-6f, -3f), Random.Range(-2f, 2f));
		}
	}

	void end()
	{
		totalScore += score;
		main.SetActive (false);
		menu.SetActive (true);

	}

	GameObject food(float x, float y)
	{
		GameObject go;
		float rand = Random.Range (0f, 5f);
		if (rand < 1f)
		{
			go = Instantiate(cupcakePrefab, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
			go.name = "food";
		}
		else if (rand < 2f)
		{
			go = Instantiate(bananaPrefab, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
			go.name = "food";
		}
		else if (rand < 3f)
		{
			go = Instantiate(potatoPrefab, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
			go.name = "food";
		}
		else if (rand < 4f)
		{
			go = Instantiate(hamburgerPrefab, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
			go.name = "food";
		}
		else
		{
			go = Instantiate(colaPrefab, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
			go.name = "food";
		}
		return go;
	}
}
