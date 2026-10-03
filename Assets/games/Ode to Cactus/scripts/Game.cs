using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.OdeToCactus
{
	public class Game : MonoBehaviour {

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
	    public static float roadSpeed = 10f;
	    public static int sceneNo;
	    public static Game instance;
	    public GameObject explosion;

		void Awake ()
		{
	        instance = this;

	        // In the collection: statics outlive the game here. A scene reached any other way
	        // than through nextLevel is the game being started, so the run begins again.
	        if (!travelling)
	        {
	            sceneNo = 0;
	            time = 0f;
	            roadSpeed = 10f;
	            Score.score = 0;
	        }
	        travelling = false;

	        // In the collection: lights the 3D scenes as Unity 4 did, see LegacyLighting.cs.
	        gameObject.AddComponent<LegacyLighting>();
		}

		void Update ()
		{
	        // In the collection: the Escape quit is gone, the collection has its own exit.
	        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.numpadEnterKey.isPressed)
	        {
	            dt = 10 * Time.deltaTime;
	        }
	        else
	        {
	            dt = Time.deltaTime;
	        }

			time += dt;

			MousePosition.get = Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
			MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = TaloketoInputManager.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;
		}

	    private static bool travelling;

	    // In the collection: scenes are loaded by path, their names are not unique here.
	    private static void load(string scene)
	    {
	        travelling = true;
	        UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/games/Ode to Cactus/scenes/" + scene + ".unity");
	    }

	    public static void nextLevel()
	    {
	        if (sceneNo == -1)
	        {
	            load("road");
	        }
	        else if (sceneNo == 0)
	        {
	            load("noise");
	        }
	        else if (sceneNo == 1)
	        {
	            load("mondo");
	        }
	        else if (sceneNo == 2)
	        {
	            load("noise");
	        }
	        else if (sceneNo == 3)
	        {
	            load("norr");
	        }
	        else if (sceneNo == 4)
	        {
	            load("noise");
	        }
	        else if (sceneNo == 5)
	        {
	            load("asia");
	        }
	        else if (sceneNo == 6)
	        {
	            load("noise");
	        }
	        else if (sceneNo == 7)
	        {
	            load("norr2");
	        }
	        else if (sceneNo == 8)
	        {
	            load("noise");
	        }
	        else if (sceneNo == 9)
	        {
	            load("end");
	        }
	        else if (sceneNo == 10)
	        {
	            load("noise");
	            sceneNo = -2;
	        }

	        sceneNo++;
	    }

	    public static void restartLevel()
	    {
	        sceneNo--;
	        nextLevel();
	    }
	}
}
