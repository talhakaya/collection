using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public enum GameSaveState
	{
		None,
		PlayedPrologue,
		Day1,
		PlayedDay1
	}

	public enum LevelType
	{
		DreamCollect,
		Explore
	}

	public enum Language
	{
		Eng,
		Tur
	}

	public class GameManagerScript : MonoBehaviour
	{
		public static GameManagerScript instance;
		public static LevelType levelType;
		public static Language lang;
		public static GameSaveState gameSaveState;

		public GameObject loadingScreen;
		public GameObject arcadeGame;
		public Texture2D cursorTexture;
		public CursorMode cursorMode = CursorMode.Auto;
		private Vector2 cursorSpot = Vector2.zero;


		void Awake ()
		{
			if (instance != null)
			{
				Destroy(instance.gameObject);
			}
			instance = this;
			gameSaveState = GameSaveState.Day1;
			Collection.Controls.PortHelpers.KeepWithinGame(gameObject); // In the collection: was DontDestroyOnLoad
		}

		void Start ()
		{
			lang = Language.Eng; // In the collection: was Language.Tur. English only for now; the Turkish lines are all still here.
			Cursor.SetCursor(cursorTexture, cursorSpot, cursorMode);
		}

		void Update ()
		{

		}

		public static void restartLevel()
		{
			changeLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
		}

		public static void changeLevel(string levelName)
		{
			GameObject loading = Instantiate(instance.loadingScreen, CameraScript.instance.transform.position + 2 * Vector3.forward, 
												Quaternion.identity) as GameObject;
			loading.transform.parent = CameraScript.instance.transform;
			loading.name = "LoadingScreen";
			loading.transform.Rotate(-90 * Vector3.right);

			LoadingScreen loadingScript = loading.GetComponent<LoadingScreen>();

			loadingScript.levelName = levelName;
		}
	}
}
