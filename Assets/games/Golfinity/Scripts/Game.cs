using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Collection.Controls;

namespace Games.Golfinity
{
	public enum Lang { EN, TR, ES, ZH, JA, KO, RU, IT, FR, PT, DE };
	public enum GameState { Play, Map };
	public class Game : MonoBehaviour
	{
	    public static Lang lang;
	    public static GameState state;
	    public static Camera cam;
	    public static Game instance;
	    public static float time;
	    public static int cheatCount;
	    public static float cheatTime;
	    public static float dt;
		public static bool input;
		private static bool inputOld;
		public static bool inputDown;
	    public static bool inputUp;
	    public static bool cameraMove;
	    public static Vector3 shadowVector;
	    public static List<string> stars;
	    public const int STAR_LENGTH = 1024;

	    private static int _gold;
	    public static int gold
	    {
	        get { return _gold; }
	        set
	        {
	            _gold = value;
	            if (instance != null) instance.RefreshUpgradeButtons();
	        }
	    }
	    public static int goldAnimating;
	    public static int noOfStrokes;
	    public AudioSource audioSource;
	    public TextMeshProUGUI textGold;
	    public GameObject textStrokeParent;
	    public TextMeshProUGUI textStroke;
	    public GameObject textHoleParent;
	    public TextMeshProUGUI textHole;
	    public ButtonExtended buttonUpgrade;
	    //public Text[] textFadeOut;

	    public static int noOfStrokesSinceBeginningOfLevel;
	    public static Vector3 lastHitPos;

	    private Vector2 cameraMoveDeltaPos;
	    public static bool reverseShooting;
	    public static bool holesOnWalls;
	    public static bool soundOn;
	    public static bool musicOn;
	    public static bool terrainEffectOn;
	    public static bool circleHoleEffectOn;
	    public static bool removedAds;
	    public AudioClip[] soundsBallHitWall;
	    public AudioClip soundPickup;
	    public AudioClip soundGold;
	    public Map map;
	    public GolfBall ball;
	    public LevelGenerator level;
	    public GameObject backButton;
	    private bool logoShown;

	    /// In the collection: writes the stars to the save file.
	    public static void SaveStars()
	    {
	        Collection.Saving.SaveManager.Slot.golfinity.stars = new List<string>(stars);
	        Collection.Saving.SaveManager.MarkDirty();
	    }

	    void Awake ()
		{
	        Application.targetFrameRate = 60;
	        instance = this;
	        shadowVector = Vector3.down + Vector3.left;
	        noOfStrokes = Collection.Saving.SaveManager.Slot.golfinity.noOfStrokes;
	        gold = Collection.Saving.SaveManager.Slot.golfinity.gold;
	        // In the collection: everything the game keeps is its part of the collection's save
	        // file (SaveManager.Slot.golfinity) rather than PlayerPrefs keys.
	        stars = new List<string>();
	        foreach (string saved in Collection.Saving.SaveManager.Slot.golfinity.stars)
	        {
	            if (saved == "") break;
	            stars.Add(saved);
	        }
	        if (stars.Count == 0) stars.Add("");

	        audioSource = GetComponent<AudioSource>();
	        time = 0f;
	        cameraMoveDeltaPos = Vector2.zero;
	        OutlineSprite.isOn = Collection.Saving.SaveManager.Slot.golfinity.outlineOn;
	        Game.reverseShooting = Collection.Saving.SaveManager.Slot.golfinity.reverseShooting;
	        Game.holesOnWalls = Collection.Saving.SaveManager.Slot.golfinity.holesOnWalls;
	        Game.soundOn = Collection.Saving.SaveManager.Slot.golfinity.soundOn;
	        Game.musicOn = Collection.Saving.SaveManager.Slot.golfinity.musicOn;
	        Game.terrainEffectOn = Collection.Saving.SaveManager.Slot.golfinity.terrainEffectOn;
	        Game.circleHoleEffectOn = Collection.Saving.SaveManager.Slot.golfinity.circleHoleEffectOn;
	        Game.removedAds = true;
	        switch (Application.systemLanguage)
	        {
	            case SystemLanguage.Turkish:
	                lang = Lang.TR;
	                break;
	            case SystemLanguage.Chinese:
	            case SystemLanguage.ChineseSimplified:
	            case SystemLanguage.ChineseTraditional:
	                lang = Lang.ZH;
	                break;
	            case SystemLanguage.Japanese:
	                lang = Lang.JA;
	                break;
	            case SystemLanguage.Korean:
	                lang = Lang.KO;
	                break;
	            case SystemLanguage.Russian:
	                lang = Lang.RU;
	                break;
	            case SystemLanguage.Spanish:
	            case SystemLanguage.Catalan:
	                lang = Lang.ES;
	                break;
	            case SystemLanguage.French:
	                lang = Lang.FR;
	                break;
	            case SystemLanguage.Italian:
	                lang = Lang.IT;
	                break;
	            case SystemLanguage.Portuguese:
	                lang = Lang.PT;
	                break;
	            case SystemLanguage.German:
	                lang = Lang.DE;
	                break;
	            default:
	                lang = Lang.EN;
	                break;
	        }
	        if (Collection.Saving.SaveManager.Slot.golfinity.lang >= 0) lang = (Lang)Collection.Saving.SaveManager.Slot.golfinity.lang;
	        Local.SetLanguage(lang);
	        cam = Camera.main;
		}

	    private void Start()
	    {
	        int lastUnbeatenHole = GetUnbeatenLastHole();
	        if (lastUnbeatenHole > 0)
	        {
	            SetState(GameState.Map);
	        }
	        else
	        {
	            SetState(GameState.Play);
	            level.OpenLevel(lastUnbeatenHole);
	        }
	    }

	    void Update ()
		{
			if (TaloketoInputManager.GetButtonDown("Back"))
	        {
	            if (UIReferences.optionsPopup.gameObject.activeSelf)
	            {
	                UIReferences.optionsPopup.Hide();
	            }
	            else if (UIReferences.levelScorePopup.gameObject.activeSelf)
	            {
	                // Goes to the map rather than just closing - dismissing the score screen
	                // used to leave the player sitting in the level they'd already finished.
	                UIReferences.levelScorePopup.OnClickRetry();
	            }
	            else if (UIReferences.cheatPopup.gameObject.activeSelf)
	            {
	                UIReferences.cheatPopup.Hide();
	            }
	            else if (UIReferences.upgradePopup.gameObject.activeSelf)
	            {
	                UIReferences.upgradePopup.Hide();
	            }
	            else if (state == GameState.Play)
	            {
	                SetState(GameState.Map);
	            }
	            // Nothing to do at the map screen. This used to Application.Quit(), which made
	            // sense when Golfinity was a standalone build, but here it would tear down the
	            // whole collection. Leaving a game is the collection's own shortcut
	            // (see GlobalInputManager: gamepad Select+Start, or Shift+Escape).
	        }
			
			dt = Time.deltaTime;
			time += dt;

	        cameraMove = false;
	#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
	        int isThereTouch = 0;
	        foreach (Touch touch in Input.touches)
	        {
	            isThereTouch++;
	            cameraMoveDeltaPos -= touch.deltaPosition * 0.1f;
	            if (isThereTouch == 1)
	            {
	                MousePosition.get = cam.ScreenToWorldPoint(touch.position) + Vector3.forward;
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
	        MousePosition.get = cam.ScreenToWorldPoint(TaloketoInputManager.mousePosition) + Vector3.forward;
	        MousePosition.x = MousePosition.get.x;
	        MousePosition.y = MousePosition.get.y;

	        input = TaloketoInputManager.GetMouseButton(0);
	        inputDown = input && !inputOld;
	        inputUp = !input && inputOld;

	        inputOld = input;
	#endif
	        if (!cameraMove)
	        {
	            cameraMoveDeltaPos = Vector2.zero;
	        }

	        if (state == GameState.Play)
	        {
	            if (!GolfBall.instance.draggingMouse && TaloketoInputManager.GetAxisRaw("Horizontal") == 0 && TaloketoInputManager.GetAxisRaw("Vertical") == 0)
	            {
	                Vector3 ballPos = new Vector3(GolfBall.instance.transform.position.x, GolfBall.instance.transform.position.y, transform.position.z);
	                ballPos += new Vector3(cameraMoveDeltaPos.x, cameraMoveDeltaPos.y, 0f);
	                transform.position += 2f * dt * (ballPos - transform.position);
	            }
	            transform.position += new Vector3(TaloketoInputManager.GetAxisRaw("Horizontal"), TaloketoInputManager.GetAxisRaw("Vertical"), 0f) * 20f * Time.deltaTime;
	            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -LevelGenerator.tileWidth, level.noOfColumns * LevelGenerator.tileWidth), Mathf.Clamp(transform.position.y, -LevelGenerator.tileWidth, level.noOfRows * LevelGenerator.tileWidth), transform.position.z);
	        }

	        textGold.text = (gold - goldAnimating).ToString();
	        textHole.text = (LevelGenerator.CurrentHoleNo + 1).ToString();
	        textStroke.text = $"{noOfStrokesSinceBeginningOfLevel}/{LevelGenerator.NumHits}";
	        textHoleParent.SetActive(state == GameState.Play);
	        textStrokeParent.SetActive(state == GameState.Play);

	        if (cheatTime < 0.2f)
	        {
	            cheatTime += Time.deltaTime;
	            if (cheatTime >= 0.2f) cheatCount = 0;
	        }
	    }

	    public void SoundBallHitWall()
	    {
	        if (!Game.soundOn) return;
	        AudioSource.PlayClipAtPoint(soundsBallHitWall[Random.Range(0, soundsBallHitWall.Length)], GolfBall.instance.transform.position);
	    }

	    public void SoundGold()
	    {
	        if (!Game.soundOn) return;
	        AudioSource.PlayClipAtPoint(soundGold, GolfBall.instance.transform.position);
	    }

	    public void SoundPickup()
	    {
	        if (!Game.soundOn) return;
	        AudioSource.PlayClipAtPoint(soundPickup, GolfBall.instance.transform.position);
	    }

	    public void SoundHole()
	    {
	        if (!Game.soundOn) return;
	        audioSource.pitch = 0.9f + Random.value * 0.2f;
	        audioSource.Play();
	    }

	    public void OnClickMap() {
	        SetState(GameState.Map);
	        if (UIReferences.tutorial.gameObject.activeSelf) UIReferences.tutorial.Hide();
	    }

	    public void OnClickOptions() {
	        UIReferences.optionsPopup.Show();
	        if (UIReferences.tutorial.gameObject.activeSelf) UIReferences.tutorial.Hide();
	    }

	    public void OnClickUpgrade()
	    {
	        UIReferences.upgradePopup.Show();
	        if (UIReferences.tutorial.gameObject.activeSelf) UIReferences.tutorial.Hide();
	    }

	    public void OnClickCheat()
	    {
	        cheatCount++;
	        cheatTime = 0f;
	        if (cheatCount == 4)
	        {
	            UIReferences.cheatPopup.Show();
	        }
	    }

	    public void RefreshUpgradeButtons()
	    {
	        bool isAvailable = false;
	        if (UIReferences.instance != null)
	        {
	            //for (int i = 0, len = UIReferences.upgradePopup.upgradeCosts.Length; i < len; i++)
	            //{
	            //    switch (i)
	            //    {
	            //        case 0:
	            //            if (!Game.unlock0Bought && gold > UIReferences.upgradePopup.upgradeCosts[i])
	            //            {
	            //                isAvailable = true;
	            //            }
	            //            break;
	            //        case 1:
	            //            if (!Game.unlock1Bought && gold > UIReferences.upgradePopup.upgradeCosts[i])
	            //            {
	            //                isAvailable = true;
	            //            }
	            //            break;
	            //        default:
	            //            throw new System.NotImplementedException();
	            //    }
	            //}
	            buttonUpgrade.colorIconFlashing = isAvailable;
	            UIReferences.levelScorePopup.buttonUpgrade.colorIconFlashing = isAvailable;
	        }
	        else
	        {
	            StartCoroutine(DoRefreshUpgradeButtons());
	        }
	    }

	    private IEnumerator DoRefreshUpgradeButtons()
	    {
	        yield return new WaitUntil(() => (UIReferences.instance != null));
	        RefreshUpgradeButtons();
	    }

	    public void SetState(GameState newState)
	    {
	        state = newState;
	        switch (state)
	        {
	            case GameState.Map:
	                map.gameObject.SetActive(true);
	                ball.gameObject.SetActive(false);
	                level.gameObject.SetActive(false);
	                backButton.SetActive(false);
	                if (!logoShown)
	                {
	                    logoShown = true;
	                    UIReferences.logo.Show();
	                }
	                map.Init(LevelGenerator.CurrentHoleNo == -1 ? GetUnbeatenLastHole() : LevelGenerator.CurrentHoleNo + 1);
	                break;
	            case GameState.Play:
	                map.gameObject.SetActive(false);
	                ball.gameObject.SetActive(true);
	                level.gameObject.SetActive(true);
	                backButton.SetActive(true);
	                if (LevelGenerator.CurrentHoleNo == 0) UIReferences.tutorial.Show();
	                if (!logoShown)
	                {
	                    logoShown = true;
	                    UIReferences.logo.Show();
	                }
	                break;
	            default:
	                throw new System.NotImplementedException();
	        }
	    }

	    public static void OpenLevel(int holeNo)
	    {
	        GolfBall.instance.ResetBall();
	        Game.instance.level.OpenLevel(holeNo);
	    }

	    public static void ResetLevel()
	    {
	        GolfBall.instance.ResetBall();
	        Game.instance.level.ResetLevel();
	    }

	    // In the collection: the game has no end. In the story mode it gives its artifact once
	    // this many holes are behind the player, and can be played on after that.
	    public const int StoryArtifactHoles = 10;

	    public static void NextLevel()
	    {
	        int holeNo = LevelGenerator.CurrentHoleNo + 1;
	        if (holeNo >= StoryArtifactHoles) Collection.Story.StoryGames.Finish();
	        if (holeNo > 0 && holeNo % LevelGenerator.numLevelsPerColor == 0 && Game.instance.level.GetNumGoldToUnlock(holeNo / LevelGenerator.numLevelsPerColor) > 0)
	        {
	            Game.instance.SetState(GameState.Map);
	        }
	        else
	        {
	            OpenLevel(holeNo);
	        }

	    }

	    public static void SendAnalytics(string eventName)
	    {
	        UnityEngine.Analytics.Analytics.CustomEvent(eventName, new Dictionary<string, object>()
	        {
	            {"platform", Application.platform.ToString()}
	        });
	    }

	    public static int GetUnbeatenLastHole()
	    {
	        if (stars == null || stars.Count == 0) return 0;
	        return (stars.Count - 1) * STAR_LENGTH + stars[stars.Count - 1].Length;
	    }

	    public static bool SetNumStars(int holeNo, int numStars)
	    {
	        int lastUnbeated = GetUnbeatenLastHole();
	        if (holeNo > lastUnbeated)
	        {
	            return false;
	        }
	        else if (holeNo == lastUnbeated)
	        {
	            int lastStarStringLength = stars[stars.Count - 1].Length;
	            if (lastStarStringLength == STAR_LENGTH)
	            {
	                stars.Add("");
	            }
	            stars[stars.Count - 1] += (char)('0' + numStars);
	        }
	        else
	        {
	            int starsIndex = holeNo / STAR_LENGTH;
	            int charIndex = holeNo % STAR_LENGTH;
	            int numStarsOld = (int)(stars[starsIndex][charIndex] - '0');
	            if (numStars > numStarsOld)
	            {
	                stars[starsIndex] = stars[starsIndex].Substring(0, charIndex) + (char)('0' + numStars) + stars[starsIndex].Substring(charIndex + 1);
	            }
	        }
	        SaveStars();
	        return true;
	    }

	    public static int GetNumStars(int holeNo)
	    {
	        int lastUnbeated = GetUnbeatenLastHole();
	        if (holeNo < 0 || holeNo >= lastUnbeated)
	        {
	            return 0;
	        }
	        int starsIndex = holeNo / STAR_LENGTH;
	        int charIndex = holeNo % STAR_LENGTH;
	        return (stars[starsIndex][charIndex] - '0');
	    }
	}

}
