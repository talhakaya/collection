using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Collection.Controls;

namespace Games.ToEveryoneIllNeverMeet
{
	public class Game : MonoBehaviour {

	    public static Game instance;
		public static float time;
		public static float dt;
	    public static Color color0 = new Color(255 / 255f, 208 / 255f, 184 / 255f);
		public static Color color1 = new Color(232 / 255f, 139 / 255f, 227 / 255f);
		public static Color color2 = new Color(165 / 255f, 176 / 255f, 255 / 255f);
		public static Color color3 = new Color(139 / 255f, 232 / 255f, 209 / 255f);
		public static Color color4 = new Color(215 / 255f, 255 / 255f, 166 / 255f);
		public static Color[] colors = new Color[]{color0, color1, color2, color3, color4};
		public static bool input;
		private static bool inputOld;
		public static bool inputDown;
		public static bool inputUp;
	    public static Vector3 shadowVector;
	    public Transform teleporter;
	    public Transform face;
	    public static bool fadeOut;
	    public const int LastLevel = 11;

	    // In the collection: the game reloads its one scene for every level and counts them
	    // in LevelPass.no. A load that is not such a reload is a fresh start.
	    public static bool travelling;
	    private static bool watchingScenes;
	    private static bool autoSyncBefore;
	    private static string scenePath;

		void Awake ()
		{
	        // In the collection: the process outlives the game, so the statics start over
	        // here, and gravity is put back (the last level switches it off).
	        if (!travelling)
	        {
	            LevelPass.no = 0;
	            time = 0f;
	            FacingPlayer.sortedSentences = null;
	        }
	        travelling = false;
	        Physics.gravity = new Vector3(0f, -9.81f, 0f);
	        scenePath = gameObject.scene.path;
	        if (!watchingScenes)
	        {
	            watchingScenes = true;
	            // In the collection: the game places the player through its transform, here and
	            // in the last level. Unity 5 passed that on to the character controller by
	            // itself; in Unity 6 that is this switch, which the collection has off.
	            autoSyncBefore = Physics.autoSyncTransforms;
	            Physics.autoSyncTransforms = true;
	            UnityEngine.SceneManagement.SceneManager.sceneLoaded += onSceneLoaded;
	        }
	        shadowVector = Vector3.down + Vector3.left;
	        instance = this;
	        fadeOut = false;
	        List<Vector3> positions = getDifferentPositions(3);
	        transform.position = new Vector3(positions[0].x, transform.position.y, positions[0].z);
	        teleporter.transform.position = new Vector3(positions[1].x, teleporter.transform.position.y, positions[1].z);
	        face.transform.position = new Vector3(positions[2].x, face.transform.position.y, positions[2].z);
	        if (LevelPass.no == 0 || LevelPass.no == Game.LastLevel)
	        {
	            face.gameObject.SetActive(false);
	            if (LevelPass.no == Game.LastLevel)
	            {
	                teleporter.gameObject.SetActive(false);
	                transform.position += Vector3.up;
	            }
	        }
	        GlobalInputManager.HideGameCursor(); // In the collection: was Cursor.visible = false
		}

	    // In the collection: the music object survives scene loads (DontDestroyOnLoad), so it
	    // has to be removed when the scene that loads is not this game's.
	    private static void onSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
	    {
	        if (scene.path == scenePath)
	        {
	            return;
	        }
	        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= onSceneLoaded;
	        watchingScenes = false;
	        Physics.autoSyncTransforms = autoSyncBefore;
	        Physics.gravity = new Vector3(0f, -9.81f, 0f);
	        if (Music.instance != null)
	        {
	            Destroy(Music.instance.gameObject);
	            Music.instance = null;
	        }
	    }

		void Update ()
		{
			// In the collection: the Escape-quit is gone (the collection has its own exit).

			dt = Time.deltaTime;
			time += dt;

			MousePosition.get = Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
			MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = TaloketoInputManager.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;

	        if (LevelPass.no == Game.LastLevel)
	        {
	            Physics.gravity = Vector3.zero;
	            transform.position += Vector3.up * Game.dt;
	            GetComponent<AudioSource>().enabled = false;
	        }
		}

	    public static List<Vector3> getDifferentPositions(int noOfPos)
	    {
	        List<Vector3> toReturn = new List<Vector3>();
	        for (int i = 0; i < noOfPos; i++)
	        {
	            toReturn.Add(Vector3.zero);
	        }
	        for (int i = 0; i < noOfPos; i++)
	        {
	            bool found = false;
	            while (!found)
	            {
	                toReturn[i] = new Vector3(Random.Range(-30f, 30f), 0f, Random.Range(-30f, 30f));
	                found = true;
	                for (int j = 0; j < i; j++)
	                {
	                    if (Geometry.lengthOfVector3(toReturn[j] - toReturn[i]) < 10f)
	                    {
	                        found = false;
	                        break;
	                    }
	                }
	            }
	        }

	        return toReturn;
	    }
	}
}
