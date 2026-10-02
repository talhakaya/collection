using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public enum Lang { EN, TR };
public class Game : MonoBehaviour
{
    public static Lang lang;
    public static Game instance;
    public static float time;
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
    public static bool cameraMove;
    public static Vector3 shadowVector;

    public static int noOfStrokes;
    public static int noOfHoles;
    public AudioSource audioSource;
    public Text textStroke;
    public Text textHole;
    //public Text[] textFadeOut;

    public bool isMenu;

    public ImageButton buttonContinue;
    public ImageButton buttonNewGame;
    public ImageButton buttonOptions;
    public ImageButton buttonReverseShooting;
    public ImageButton buttonHolesOnWalls;
    public ImageButton buttonSound;
    public ImageButton buttonMusic;
    public ImageButton buttonOutlines;
    public ImageButton buttonTerrainEffect;
    public ImageButton buttonCircleHoleEffect;
    public ImageButton buttonBallTrailEffect;
    public ImageButton buttonBack;
    public GameObject[] mainMenu;
    public GameObject[] optionsMenu;
    public int newGamePressedCount;

    public int overrideNoOfHoles;
    private Vector2 cameraMoveDeltaPos;
    public static bool reverseShooting;
    public static bool holesOnWalls;
    public static bool soundOn;
    public static bool musicOn;
    public static bool terrainEffectOn;
    public static bool circleHoleEffectOn;
    public static bool ballTrailEffectOn;
    public AudioClip[] soundsBallHitWall;
    public static bool developerMode = false;
    public GameObject colorPicker;

    public GameObject[] devModeDisableObjects;

	void Awake ()
	{
        instance = this;
        shadowVector = Vector3.down + Vector3.left;
        noOfHoles = PlayerPrefs.GetInt("noOfHoles", 1);
        noOfStrokes = PlayerPrefs.GetInt("noOfStrokes", 0);
        if (overrideNoOfHoles > 0)
        {
            noOfHoles = overrideNoOfHoles;
            PlayerPrefs.SetInt("noOfHoles", noOfHoles);
        }

        if (isMenu)
        {
        }
        //adTime = 0f;
        
        audioSource = GetComponent<AudioSource>();
        //Screen.orientation = ScreenOrientation.Landscape;
        time = 0f;
        cameraMoveDeltaPos = Vector2.zero;
        int outlineDefault = 2;
#if !UNITY_EDITOR && UNITY_ANDROID
        outlineDefault = 0;
#endif
        OutlineSprite.isOn = (OutlineSprite.Outline) PlayerPrefs.GetInt("OutlineSprite.isOn", outlineDefault);
        Game.reverseShooting = (PlayerPrefs.GetInt("Game.reverseShooting", 1) == 1);
        Game.holesOnWalls = (PlayerPrefs.GetInt("Game.holesOnWalls", 1) == 1);
        Game.soundOn = (PlayerPrefs.GetInt("Game.soundOn", 1) == 1);
        Game.musicOn = (PlayerPrefs.GetInt("Game.musicOn", 1) == 1);
        Game.terrainEffectOn = (PlayerPrefs.GetInt("Game.terrainEffectOn", 1) == 1);
        Game.circleHoleEffectOn = (PlayerPrefs.GetInt("Game.circleHoleEffectOn", 1) == 1);
        Game.ballTrailEffectOn = (PlayerPrefs.GetInt("Game.ballTrailEffectOn", 1) == 1);
        if (Application.systemLanguage == SystemLanguage.Turkish)
        {
            lang = Lang.TR;
        }
        else
        {
            lang = Lang.EN;
        }
        if (isMenu)
        {
            buttonContinue.interactable = (noOfHoles != 1);
            if (Game.reverseShooting)
            {
                buttonReverseShooting.textEN = "reverse shooting: on";
                buttonReverseShooting.textTR = "ters çekerek vur";
            }
            else
            {
                buttonReverseShooting.textEN = "reverse shooting: off";
                buttonReverseShooting.textTR = "düz çekerek vur";
            }
            if (Game.holesOnWalls)
            {
                buttonHolesOnWalls.textEN = "side holes: on";
                buttonHolesOnWalls.textTR = "yan delikler: açık";
            }
            else
            {
                buttonHolesOnWalls.textEN = "side holes: off";
                buttonHolesOnWalls.textTR = "yan delikler: kapalı";
            }
            if (Game.soundOn)
            {
                buttonSound.textEN = "sound: on";
                buttonSound.textTR = "ses: açık";
            }
            else
            {
                buttonSound.textEN = "sound: off";
                buttonSound.textTR = "ses: kapalı";
            }
            if (Game.musicOn)
            {
                buttonMusic.textEN = "music: on";
                buttonMusic.textTR = "müzik: açık";
            }
            else
            {
                buttonMusic.textEN = "music: off";
                buttonMusic.textTR = "müzik: kapalı";
            }
            if (OutlineSprite.isOn == OutlineSprite.Outline.x4)
            {
                buttonOutlines.textEN = "outlines: 4X";
                buttonOutlines.textTR = "dış çizgiler: 4X";
            }
            else if (OutlineSprite.isOn == OutlineSprite.Outline.x8)
            {
                buttonOutlines.textEN = "outlines: 8X";
                buttonOutlines.textTR = "dış çizgiler: 8X";
            }
            else
            {
                buttonOutlines.textEN = "outlines: off";
                buttonOutlines.textTR = "dış çizgiler: kapalı";
            }
            if (Game.terrainEffectOn)
            {
                buttonTerrainEffect.textEN = "terrain effect: on";
                buttonTerrainEffect.textTR = "yer efekti: açık";
            }
            else
            {
                buttonTerrainEffect.textEN = "terrain effect: off";
                buttonTerrainEffect.textTR = "yer efekti: kapalı";
            }
            if (Game.circleHoleEffectOn)
            {
                buttonCircleHoleEffect.textEN = "circle hole effect: on";
                buttonCircleHoleEffect.textTR = "çember efekti: açık";
            }
            else
            {
                buttonCircleHoleEffect.textEN = "circle hole effect: off";
                buttonCircleHoleEffect.textTR = "çember efekti: kapalı";
            }
            if (Game.ballTrailEffectOn)
            {
                buttonBallTrailEffect.textEN = "ball trail effect: on";
                buttonBallTrailEffect.textTR = "top kuyruğu: açık";
            }
            else
            {
                buttonBallTrailEffect.textEN = "ball trail effect: off";
                buttonBallTrailEffect.textTR = "top kuyruğu: kapalı";
            }
        }
	}
	
	void Update ()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                if (optionsMenu[0].activeSelf)
                {
                    for (int i = 0; i < optionsMenu.Length; i++)
                    {
                        optionsMenu[i].SetActive(false);
                    }
                    for (int i = 0; i < mainMenu.Length; i++)
                    {
                        mainMenu[i].SetActive(true);
                    }
                }
                else
                {
                    Application.Quit();
                }
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        if (developerMode)
        {
            if (Input.GetKey(KeyCode.Alpha0))
            {
                noOfHoles = 1;
                noOfStrokes = 0;
                GolfBall.instance.ResetBall();
                LevelGenerator.instance.ResetLevel();
            }
            if (Input.GetKey(KeyCode.Alpha1))
            {
                noOfHoles = Random.Range(0, 50);
                noOfStrokes = noOfHoles * 4 + Random.Range(0, 50);
                GolfBall.instance.ResetBall();
                LevelGenerator.instance.ResetLevel();
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                noOfHoles = Random.Range(51, 100);
                noOfStrokes = noOfHoles * 4 + Random.Range(0, 50);
                GolfBall.instance.ResetBall();
                LevelGenerator.instance.ResetLevel();
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                noOfHoles = Random.Range(101, 150);
                noOfStrokes = noOfHoles * 4 + Random.Range(0, 50);
                GolfBall.instance.ResetBall();
                LevelGenerator.instance.ResetLevel();
            }
            if (Input.GetKey(KeyCode.Alpha4))
            {
                noOfHoles = Random.Range(151, 200);
                noOfStrokes = noOfHoles * 4 + Random.Range(0, 50);
                GolfBall.instance.ResetBall();
                LevelGenerator.instance.ResetLevel();
            }
            if (Input.GetKey(KeyCode.Alpha5))
            {
                noOfHoles = Random.Range(201, 250);
                noOfStrokes = noOfHoles * 4 + Random.Range(0, 50);
                GolfBall.instance.ResetBall();
                LevelGenerator.instance.ResetLevel();
            }
            if (Input.GetKey(KeyCode.Alpha6))
            {
                noOfHoles = Random.Range(251, 300);
                noOfStrokes = noOfHoles * 4 + Random.Range(0, 50);
                GolfBall.instance.ResetBall();
                LevelGenerator.instance.ResetLevel();
            }
            if (Input.GetKey(KeyCode.Alpha7))
            {
                noOfHoles = Random.Range(301, 350);
                noOfStrokes = noOfHoles * 4 + Random.Range(0, 50);
                GolfBall.instance.ResetBall();
                LevelGenerator.instance.ResetLevel();
            }
            if (Input.GetKey(KeyCode.Alpha8))
            {
                noOfHoles = Random.Range(351, 400);
                noOfStrokes = noOfHoles * 4 + Random.Range(0, 50);
                GolfBall.instance.ResetBall();
                LevelGenerator.instance.ResetLevel();
            }
            if (Input.GetKey(KeyCode.Alpha9))
            {
                noOfHoles = Random.Range(401, 450);
                noOfStrokes = noOfHoles * 4 + Random.Range(0, 50);
                GolfBall.instance.ResetBall();
                LevelGenerator.instance.ResetLevel();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                colorPicker.SetActive(!colorPicker.activeSelf);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                for (int i = 0; i < devModeDisableObjects.Length; i++)
                {
                    devModeDisableObjects[i].SetActive(!devModeDisableObjects[i].activeSelf);
                }
            }
        }
		
		dt = Time.deltaTime;
		time += dt;

        cameraMove = false;
#if !UNITY_EDITOR && UNITY_ANDROID
        int isThereTouch = 0;
        foreach (Touch touch in Input.touches)
        {
            isThereTouch++;
            cameraMoveDeltaPos -= touch.deltaPosition * 0.2f;
            if (isThereTouch == 1)
            {
                MousePosition.get = Camera.main.ScreenToWorldPoint(touch.position) + Vector3.forward;
                MousePosition.x = MousePosition.get.x;
                MousePosition.y = MousePosition.get.y;
            }
        }

        if (isThereTouch > 1)
        {
            input = false;
            inputDown = false;
            inputUp = false;
            cameraMove = true;
        }

        if (input && isThereTouch == 1)
        {
            input = true;
            inputDown = false;
            inputUp = false;
        }
        else if (input && isThereTouch == 0)
        {
            input = false;
            inputDown = false;
            inputUp = true;
        }
        else if (!input && isThereTouch == 1)
        {
            input = true;
            inputDown = true;
            inputUp = false;
        }
        else if (!input && isThereTouch == 0)
        {
            input = false;
            inputDown = false;
            inputUp = false;
        }
#else
        MousePosition.get = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward;
        MousePosition.x = MousePosition.get.x;
        MousePosition.y = MousePosition.get.y;

        input = Input.GetMouseButton(0);
        inputDown = input && !inputOld;
        inputUp = !input && inputOld;

        inputOld = input;
#endif
        if (!cameraMove)
        {
            cameraMoveDeltaPos = Vector2.zero;
        }

        if (!GolfBall.instance.draggingMouse)
        {
            Vector3 ballPos = new Vector3(GolfBall.instance.transform.position.x, GolfBall.instance.transform.position.y, transform.position.z);
            ballPos += new Vector3(cameraMoveDeltaPos.x, cameraMoveDeltaPos.y, 0f);
            transform.position += 2f * dt * (ballPos - transform.position);
        }

        if (!isMenu)
        {
            if (Game.lang == Lang.EN)
            {
                textHole.text = "hole: " + noOfHoles;
                textStroke.text = "strokes: " + noOfStrokes;
            }
            else
            {
                textHole.text = "delik: " + noOfHoles;
                textStroke.text = "vuruş: " + noOfStrokes;
            }

            if (buttonBack.pressed)
            {
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            if (buttonContinue.pressed)
            {
                Analytics.CustomEvent("gameStart", new Dictionary<string, object>
                {
                    { "outlineOn", OutlineSprite.isOn != OutlineSprite.Outline.Off },
                    { "reverseShooting", reverseShooting },
                    { "holesOnWalls", holesOnWalls },
                    { "soundOn", soundOn },
                    { "musicOn", musicOn },
                    { "terrainEffectOn", terrainEffectOn },
                    { "circleHoleEffectOn", circleHoleEffectOn }
                });
                SceneManager.LoadScene(1);
            }
            if (buttonNewGame.pressed)
            {
                if (noOfHoles == 1)
                {
                    Analytics.CustomEvent("gameStart", new Dictionary<string, object>
                    {
                        { "outlineOn", OutlineSprite.isOn != OutlineSprite.Outline.Off },
                        { "reverseShooting", reverseShooting },
                        { "holesOnWalls", holesOnWalls },
                        { "soundOn", soundOn },
                        { "musicOn", musicOn },
                        { "terrainEffectOn", terrainEffectOn },
                        { "circleHoleEffectOn", circleHoleEffectOn }
                    });
                    SceneManager.LoadScene(1);
                }
                else
                {
                    if (newGamePressedCount == 3)
                    {
                        noOfHoles = 1;
                        noOfStrokes = 0;
                        PlayerPrefs.SetInt("noOfHoles", 1);
                        PlayerPrefs.SetInt("noOfStrokes", 0);
                        SceneManager.LoadScene(1);
                    }
                    else if (newGamePressedCount == 2)
                    {
                        buttonNewGame.textEN = noOfHoles + " holes?";
                        buttonNewGame.textTR = noOfHoles + " delik?";
                    }
                    else if (newGamePressedCount == 1)
                    {
                        buttonNewGame.textEN = "delete save?";
                        buttonNewGame.textTR = "kaydı sil?";
                    }
                    else if (newGamePressedCount == 0)
                    {
                        buttonNewGame.textEN = "sure?";
                        buttonNewGame.textTR = "emin misin?";
                    }
                    newGamePressedCount++;
                }
            }
            if (buttonOptions.pressed)
            {
                for (int i = 0; i < optionsMenu.Length; i++)
                {
                    optionsMenu[i].SetActive(true);
                }
                for (int i = 0; i < mainMenu.Length; i++)
                {
                    mainMenu[i].SetActive(false);
                }
                newGamePressedCount = 0;
                buttonNewGame.textEN = "new game";
                buttonNewGame.textTR = "yeni oyun";
            }
            if (buttonBack.pressed)
            {
                for (int i = 0; i < optionsMenu.Length; i++)
                {
                    optionsMenu[i].SetActive(false);
                }
                for (int i = 0; i < mainMenu.Length; i++)
                {
                    mainMenu[i].SetActive(true);
                }
            }
            if (buttonReverseShooting.pressed)
            {
                Game.reverseShooting = !Game.reverseShooting;
                if (Game.reverseShooting)
                {
                    buttonReverseShooting.textEN = "reverse shooting: on";
                    buttonReverseShooting.textTR = "ters çekerek vur";
                }
                else
                {
                    buttonReverseShooting.textEN = "reverse shooting: off";
                    buttonReverseShooting.textTR = "düz çekerek vur";
                }
                PlayerPrefs.SetInt("Game.reverseShooting", Game.reverseShooting ? 1 : 0);
            }
            if (buttonHolesOnWalls.pressed)
            {
                Game.holesOnWalls = !Game.holesOnWalls;
                if (Game.holesOnWalls)
                {
                    buttonHolesOnWalls.textEN = "side holes: on";
                    buttonHolesOnWalls.textTR = "yan delikler: açık";
                }
                else
                {
                    buttonHolesOnWalls.textEN = "side holes: off";
                    buttonHolesOnWalls.textTR = "yan delikler: kapalı";
                }
                PlayerPrefs.SetInt("Game.holesOnWalls", Game.holesOnWalls ? 1 : 0);
            }
            if (buttonSound.pressed)
            {
                Game.soundOn = !Game.soundOn;
                if (Game.soundOn)
                {
                    buttonSound.textEN = "sound: on";
                    buttonSound.textTR = "ses: açık";
                    buttonSound.GetComponent<AudioSource>().Play();
                }
                else
                {
                    buttonSound.textEN = "sound: off";
                    buttonSound.textTR = "ses: kapalı";
                    buttonSound.GetComponent<AudioSource>().Stop();
                }
                PlayerPrefs.SetInt("Game.soundOn", Game.soundOn ? 1 : 0);
            }
            if (buttonMusic.pressed)
            {
                Game.musicOn = !Game.musicOn;
                if (Game.musicOn)
                {
                    buttonMusic.textEN = "music: on";
                    buttonMusic.textTR = "müzik: açık";
                }
                else
                {
                    buttonMusic.textEN = "music: off";
                    buttonMusic.textTR = "müzik: kapalı";
                }
                PlayerPrefs.SetInt("Game.musicOn", Game.musicOn ? 1 : 0);
            }
            if (buttonOutlines.pressed)
            {
                if (OutlineSprite.isOn == OutlineSprite.Outline.Off)
                {
                    OutlineSprite.isOn = OutlineSprite.Outline.x4;
                }
                else if (OutlineSprite.isOn == OutlineSprite.Outline.x4)
                {
                    OutlineSprite.isOn = OutlineSprite.Outline.x8;
                }
                else if (OutlineSprite.isOn == OutlineSprite.Outline.x8)
                {
                    OutlineSprite.isOn = OutlineSprite.Outline.Off;
                }
                if (OutlineSprite.isOn == OutlineSprite.Outline.x4)
                {
                    buttonOutlines.textEN = "outlines: 4X";
                    buttonOutlines.textTR = "dış çizgiler: 4X";
                }
                else if (OutlineSprite.isOn == OutlineSprite.Outline.x8)
                {
                    buttonOutlines.textEN = "outlines: 8X";
                    buttonOutlines.textTR = "dış çizgiler: 8X";
                }
                else
                {
                    buttonOutlines.textEN = "outlines: off";
                    buttonOutlines.textTR = "dış çizgiler: kapalı";
                }
                PlayerPrefs.SetInt("OutlineSprite.isOn", (int) OutlineSprite.isOn);
            }
            if (buttonTerrainEffect.pressed)
            {
                Game.terrainEffectOn = !Game.terrainEffectOn;
                if (Game.terrainEffectOn)
                {
                    buttonTerrainEffect.textEN = "terrain effect: on";
                    buttonTerrainEffect.textTR = "yer efekti: açık";
                }
                else
                {
                    buttonTerrainEffect.textEN = "terrain effect: off";
                    buttonTerrainEffect.textTR = "yer efekti: kapalı";
                }
                PlayerPrefs.SetInt("Game.terrainEffectOn", Game.terrainEffectOn ? 1 : 0);
            }
            if (buttonCircleHoleEffect.pressed)
            {
                Game.circleHoleEffectOn = !Game.circleHoleEffectOn;
                if (Game.circleHoleEffectOn)
                {
                    buttonCircleHoleEffect.textEN = "circle hole effect: on";
                    buttonCircleHoleEffect.textTR = "çember efekti: açık";
                }
                else
                {
                    buttonCircleHoleEffect.textEN = "circle hole effect: off";
                    buttonCircleHoleEffect.textTR = "çember efekti: kapalı";
                }
                PlayerPrefs.SetInt("Game.circleHoleEffectOn", Game.circleHoleEffectOn ? 1 : 0);
            }
            if (buttonBallTrailEffect.pressed)
            {
                Game.ballTrailEffectOn = !Game.ballTrailEffectOn;
                if (Game.ballTrailEffectOn)
                {
                    buttonBallTrailEffect.textEN = "ball trail effect: on";
                    buttonBallTrailEffect.textTR = "top kuyruğu: açık";
                }
                else
                {
                    buttonBallTrailEffect.textEN = "ball trail effect: off";
                    buttonBallTrailEffect.textTR = "top kuyruğu: kapalı";
                }
                GolfBall.instance.updateTrailEffect();
                PlayerPrefs.SetInt("Game.ballTrailEffectOn", Game.ballTrailEffectOn ? 1 : 0);
            }
        }
	}

    public void SoundBallHitWall()
    {
        AudioSource.PlayClipAtPoint(soundsBallHitWall[Random.Range(0, soundsBallHitWall.Length)], transform.position + Vector3.forward * 10f);
    }
}
