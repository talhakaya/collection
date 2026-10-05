using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Collection.Controls;

namespace Games.CrimeFactory
{
	public class Platformer : MonoBehaviour {

	    public Simulator simulator;

	    public static bool isPaused;
	    public static float time;
	    public static float dt;
		public static Color color0 = new Color(183 / 255f, 162 / 255f, 130 / 255f);
		public static Color color1 = new Color(193 / 255f, 106 / 255f, 68 / 255f);
		public static Color color2 = new Color(170 / 255f, 68 / 255f, 68 / 255f);
		public static Color color3 = new Color(189 / 255f, 68 / 255f, 193 / 255f);
	    public static bool isMK;
	    public static Color color4 = new Color(110 / 255f, 64 / 255f, 183 / 255f);
		public static Color[] colors = new Color[]{color0, color1, color2, color3, color4};
		public static bool input;
		private static bool inputOld;
		public static bool inputDown;
		public static bool inputUp;
	    public static Vector3 shadowVector;
	    public static float DefaultShadowDistance = 0.3f;
	    public static float OutlineDist = 0.045f;
	    public static int LevelIndex {
	        get { return levelIndex; }
	    }
	    private static int levelIndex;
	    private static string lastLevelName;
	    private static string pathFormat;
	    public int startLevelIndex;
	    public string startLevelName;
	    public static int money = 5000;
	    public static int numFireRateUpgrades;
	    public static int numShields;
	    public static int numBombs;
	    private Camera cam;
	    public static bool isUsingController = false;
	    public GameObject tutoRestart;

	    void Start () {
	        // In the collection: the process outlives the game, so the clock starts again with it.
	        time = 0f;
	        isPaused = false;
	        input = inputOld = inputDown = inputUp = false;
	        Localization.Init();
	        // In the collection: the levels live in a folder of their own under StreamingAssets.
	        pathFormat = Application.streamingAssetsPath + "/CrimeFactory/Levels/{0}.txt";
	        levelIndex = startLevelIndex;
	        lastLevelName = startLevelName;
	        LevelEditor.OnLoad += OnLoad;

	        SaveSystem.Load(out lastLevelName, out money, out numFireRateUpgrades, out numShields, out numBombs);
	        int.TryParse(lastLevelName, out levelIndex);
	        string path = string.Format(pathFormat, lastLevelName);
	        if (File.Exists(path)) {
	            LevelEditor.Load(File.ReadAllText(path), lastLevelName);
	        }
	        else {
	            path = string.Format(pathFormat, "mom0");
	            LevelEditor.Load(File.ReadAllText(path), lastLevelName);
	        }

	        shadowVector = Vector3.down + Vector3.left;
	        Simulator.dt = Time.fixedDeltaTime;
	        simulator = new Simulator();
	        simulator.restartCounter = 1;
	        cam = GetComponent<Camera>();
	    }

		void Update () {
	        if (CancelPressed()) {
	            UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/games/Crime Factory/Scenes/menu.unity"); // In the collection: by path, scene names are not unique here.
	        }
	        if ((simulator == null || simulator.player == null) && !LevelEditor.On) return;
	        // In the collection: this looked at four joystick buttons and eight keys through KeyCode.
	        // The device that was used last decides, which is what those tests amounted to.
	        var pad = UnityEngine.InputSystem.Gamepad.current;
	        var keyboard = UnityEngine.InputSystem.Keyboard.current;
	        if (pad != null && pad.wasUpdatedThisFrame && (keyboard == null || pad.lastUpdateTime > keyboard.lastUpdateTime)) {
	            isUsingController = true;
	        }
	        if (keyboard != null && keyboard.anyKey.isPressed) {
	            isUsingController = false;
	        }

			MousePosition.get = Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
			MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = TaloketoInputManager.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;

	        isPaused = LevelEditor.On;

	        if (!isPaused) {
	            dt = Time.deltaTime;
	            time += dt;
	            cam.orthographicSize = Mathf.Min((Mathf.Min(LevelEditor.level.GetLength(0) * 1.334f, LevelEditor.level.GetLength(1)) + 2) * 0.5f, 10);
	            float camSize = cam.orthographicSize * 2f - 1; 
	            Vector3 goalDist = new Vector3(0f, 0f, 0f);
	            if (LevelEditor.level.GetLength(0) < camSize * Screen.width / Screen.height)
	            {
	                transform.position = new Vector3((LevelEditor.level.GetLength(0) - 1) * 0.5f, transform.position.y, transform.position.z);
	            }
	            else
	            {
	                goalDist.x = transform.position.x - simulator.player.rect.x;
	                if (Mathf.Abs(goalDist.x) > 1f)
	                {
	                    transform.position += new Vector3(goalDist.x > 0f ? 1f : (goalDist.x < 0f ? -1f : 0f), 0f, 0f) * -Mathf.Abs(goalDist.x) * 2f * dt;
	                }
	                float camHorizontalSize = cam.orthographicSize * Screen.width / Screen.height;
	                float minX = camHorizontalSize - 1.5f;
	                float maxX = LevelEditor.level.GetLength(0) - camHorizontalSize + 0.5f;
	                if (transform.position.x < minX) transform.position = new Vector3(minX, transform.position.y, transform.position.z);
	                if (transform.position.x > maxX) transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
	            }
	            if (LevelEditor.level.GetLength(1) < camSize)
	            {
	                transform.position = new Vector3(transform.position.x, (LevelEditor.level.GetLength(1) - 1) * 0.5f, transform.position.z);
	            }
	            else
	            {
	                goalDist.y = transform.position.y - simulator.player.rect.y;
	                if (Mathf.Abs(goalDist.y) > 2f)
	                {
	                    transform.position += new Vector3(0f, goalDist.y > 0f ? 1f : (goalDist.y < 0f ? -1f : 0f), 0f) * -Mathf.Abs(goalDist.y) * 2f * dt;
	                }
	                float minY = cam.orthographicSize - 1.5f;
	                float maxY = LevelEditor.level.GetLength(1) - cam.orthographicSize + 0.5f;
	                if (transform.position.y < minY) transform.position = new Vector3(transform.position.x, minY, transform.position.z);
	                if (transform.position.y > maxY) transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
	            }
	        }
	        else {
	            dt = 0f;
	            transform.position += new Vector3(TaloketoInputManager.GetAxisRaw("Horizontal"), TaloketoInputManager.GetAxisRaw("Vertical"), 0f) * 10f * Time.deltaTime;
	        }

	        if (simulator != null && simulator.player != null) {
	            tutoRestart.SetActive(!simulator.player.isActiveAndEnabled);
	        }
	    }

	    // In the collection: Cancel was Escape and Start, which are the collection's pause
	    // screen now. It is Backspace and Select.
	    public static bool CancelPressed() {
	        return TaloketoInputManager.GetButtonDown("Cancel");
	    }

	    private int frameCount;

	    void FixedUpdate() {
	        if (!LevelEditor.On) {
	#if UNITY_EDITOR
	            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.numpadEnterKey.isPressed) {
	                frameCount++;
	                if (frameCount > 20) {
	                    frameCount = 0;
	                    simulator.Update();
	                }
	            }
	            else {
	                simulator.Update();
	            }
	#else
	            simulator.Update();
	#endif
	        }
	    }

	    public static void Restart() {
	        if (!string.IsNullOrEmpty(lastLevelName)) {
	            LevelEditor.Load(File.ReadAllText(string.Format(pathFormat, lastLevelName)), lastLevelName);
	        }
	        else
	        {
	            LevelEditor.Load(File.ReadAllText(string.Format(pathFormat, "empty")), lastLevelName);
	        }
	    }

	    public static void NextLevel() {
	        levelIndex++;
	        lastLevelName = levelIndex.ToString();
	        Restart();
	    }

	    public static void OpenLevel(string levelName) {
	        lastLevelName = levelName;
	        int.TryParse(levelName, out levelIndex);
	        LevelEditor.Load(File.ReadAllText(string.Format(pathFormat, levelName)), levelName);
	    }

	    public void OnLoad(string levelName) {
	        lastLevelName = levelName;
	        int.TryParse(levelName, out levelIndex);
	        SaveSystem.SaveLevel(levelName);
	    }
	}
}
