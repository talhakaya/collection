using UnityEngine;
using System.Collections;
using Collection.Controls;
using UnityEngine.InputSystem;

namespace Games.LetsNeverDoThatAgain
{
	public class Game : MonoBehaviour {

	    public static Game instance;
		public static float time;
	    public static float dt;
	    //public static Color color0 = new Color(183 / 255f, 162 / 255f, 130 / 255f);
	    //public static Color color1 = new Color(193 / 255f, 106 / 255f, 68 / 255f);
	    //public static Color color2 = new Color(170 / 255f, 68 / 255f, 68 / 255f);
	    //public static Color color3 = new Color(189 / 255f, 68 / 255f, 193 / 255f);
	    //public static Color color4 = new Color(110 / 255f, 64 / 255f, 183 / 255f);
	    public static Color color0 = new Color(225 / 255f, 255 / 255f, 100 / 255f);
	    public static Color color1 = new Color(77 / 255f, 255 / 255f, 150 / 255f);
	    public static Color color2 = new Color(94 / 255f, 164 / 255f, 219 / 255f);
	    public static Color color3 = new Color(146 / 255f, 82 / 255f, 225 / 255f);
	    public static Color color4 = new Color(231 / 255f, 114 / 255f, 130 / 255f);
		public static Color[] colors = new Color[]{color0, color1, color2, color3, color4};
		public static bool input;
		private static bool inputOld;
		public static bool inputDown;
		public static bool inputUp;

	    public AudioSource aStep;
	    public AudioSource aText;
	    public AudioSource aHit;

	    void Awake()
	    {
	        instance = this;

	        // In the collection: statics outlive the game here. A scene reached any other way
	        // than through Scene0.nextLevel is the game being started, so it begins again.
	        if (!Scene0.travelling)
	        {
	            Scene0.sceneCount = 0;
	            time = 0f;
	            input = inputOld = inputDown = inputUp = false;
	        }
	        Scene0.travelling = false;
	    }

		void Start ()
		{

		}

		void Update ()
		{
	        // In the collection: the Escape-quit that stood here is gone; the collection has its own exit.

			dt = Time.deltaTime * ((Keyboard.current != null && (Keyboard.current.numpadEnterKey.isPressed || Keyboard.current.enterKey.isPressed)) ? 4f : 1f);
			time += dt;

			MousePosition.get = Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
			MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = TaloketoInputManager.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;
		}
	}
}
