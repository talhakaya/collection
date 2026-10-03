using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.VirtualPet
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
	    public static Vector3 shadowVector;
	    public static bool focusStatus;

	    void Awake()
	    {
	        // In the collection: this was PlayerPrefs.DeleteAll(), which would wipe every game's
	        // saves. Only this game's own keys are cleared (it always started fresh).
	        PlayerPrefs.DeleteKey("VirtualPet.hasSave");
	        PlayerPrefs.DeleteKey("VirtualPet.playerName");
	        PlayerPrefs.DeleteKey("VirtualPet.petName");
	        focusStatus = true;
	        // In the collection: the game ran in its own small square window (250 pixels, resizable,
	        // kept square). Inside the collection the resolution is left alone.
	    }

		void Start ()
		{
	        shadowVector = Vector3.down + Vector3.left;
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

	        // In the collection: lets the gamepad's pointer press the game's buttons.
	        PortHelpers.ClickUiWithEmulatedPointer();

	    }


	    void OnApplicationFocus(bool _focusStatus)
	    {
	        focusStatus = _focusStatus;
	    }
	}
}
