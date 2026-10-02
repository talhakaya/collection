using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game : MonoBehaviour {
    public static Game instance;
    public static Level currentLevel;
    public Level[] levels;
    public int levelIndex;
    public static float time;
	public static float timeSpeed;
    public static float dt;
	public static float dtPhysics;
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
    public Transform mouse;
    public GameObject[] allTools;
    public Image imageBlack;
    private int _moneyTotal;
    public int moneyTotal {
        get {
            return _moneyTotal;
        }
        set {
            if (value != _moneyTotal) {
                UiManager.instance.uiMoneyCounter.AddMoney(value - _moneyTotal);
            }
            _moneyTotal = value;
        }
    }
    private float _milk;
    public float milk {
        get {
            return _milk;
        }
        set {
            UiManager.instance.uiMilkCounter.SetMilk(value, _milk);
            UiManager.instance.uiMilkEnd.SetMilk(value, _milk);
            _milk = value;
        }
    }

    void Awake() {
        shadowVector = Vector3.down + Vector3.left;
        timeSpeed = 1f;
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        currentLevel = Instantiate(levels[levelIndex].gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Level>();
        ResetLevel();
        _milk = 1f;
        _moneyTotal = 700;
    }

    public void PrevLevel() {
        moneyTotal += currentLevel.moneyCollected;
        Destroy(currentLevel.gameObject);
        levelIndex--;
        if (levelIndex < 0) {
            levelIndex = 0;
        }
        currentLevel = Instantiate(levels[levelIndex].gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Level>();
        ResetLevel();
    }

    public void NextLevel() {
        moneyTotal += currentLevel.moneyCollected;
        Destroy(currentLevel.gameObject);
        levelIndex++;
        if (levelIndex >= levels.Length) {
            levelIndex = levels.Length - 1;
        }
        currentLevel = Instantiate(levels[levelIndex].gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Level>();
        ResetLevel();
    }

    public void ResetLevel() {
        imageBlack.color = new Color(0f, 0f, 0f, 1f);
        currentLevel.ResetLevel();
    }

    void Update () {
        if (Input.GetButtonDown("Cancel")) {
            Application.Quit();
        }
        if (Input.GetButtonDown("Restart")) {
            ResetLevel();
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            PrevLevel();
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            NextLevel();
        }

        dt = Time.deltaTime * timeSpeed;
        dtPhysics = Time.fixedDeltaTime * timeSpeed;
		time += dt;

        MousePosition.get = Input.mousePosition;
        MousePosition.get.z = 10;
        MousePosition.get = Camera.main.ScreenToWorldPoint(MousePosition.get);
        MousePosition.get.z = 0f;
        MousePosition.x = MousePosition.get.x;
        MousePosition.y = MousePosition.get.y;
        mouse.position = MousePosition.get;

        if (currentLevel.finish.ed && !UiManager.instance.uiScore.isActive) {
            if (currentLevel.hasChallenge) {
                UiManager.instance.uiScore.StartScoring(currentLevel.moneyCollected, moneyTotal, currentLevel.moneyCollected >= currentLevel.moneyToFinish, currentLevel.isStealthy, currentLevel.time <= currentLevel.timeLimit);
            }
            else if (currentLevel.milkChangeAtTheEnd != 0) {
                if (!UiManager.instance.uiMilkEnd.isActive) {
                    UiManager.instance.uiMilkEnd.isActive = true;
                    milk += currentLevel.milkChangeAtTheEnd;
                }
            }
            else {
                NextLevel();
            }
        }

        imageBlack.color = new Color(0f, 0f, 0f, Mathf.Clamp(imageBlack.color.a - dt * 2f, 0f, 1f));

        UiManager.instance.imageRestart.enabled = Player.person.state == Person.State.Dead;
    }

    public static float Rhythm(float period = 1f) {
        float animRatio = (Game.time % period) / period;
        if (animRatio > 0.5f) {
            animRatio = 1f - animRatio;
        }
        return animRatio * 2f;
    }

    public static bool CanWalkInto(GameObject go) {
        return go.CompareTag("Player") || go.CompareTag("Enemy") || go.CompareTag("Drone") || go.CompareTag("Bug");
    }
}
