using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Collection.Controls;

namespace Games.WhereIsHe
{
	public class Game : MonoBehaviour {

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
	    public static Vector3 shadowVector;

	    private float lineRhythm;
	    public float lineHeightOffset;
	    public bool started;
	    private bool firstOffsetDrop;
	    public GameObject[] enableWhenStarted;
	    public GameObject[] disableWhenStarted;

	    public GameObject enableWhenTalking;
	    public Text talkText;
	    public Image talkImage;
	    public static float screenShake = 0f;
	    private float screenShakeOld = 0f;

		// In the collection: the old project had a layer named "Ground" at index 8, which the
		// collection does not name. The objects keep the index, so the mask is built from it.
		public const int GroundMask = 1 << 8;

		// In the collection: statics outlive the game here, where they died with the application.
		void Awake ()
		{
		    time = 0f;
		    screenShake = 0f;
		    LineManager.SrgbSplit = 0f;
		    WorldWander.currentRoom = null;
		}

		// In the collection: the two prompts in the text objects name what works here. Enter
		// still starts the game, and so does A on a gamepad; ESC no longer quits, the
		// collection's own exit does.
		private Text[] prompts;
		private string[] promptTexts;
		private int promptDevice = -1;

		void updatePrompts ()
		{
		    UnityEngine.InputSystem.Gamepad pad = UnityEngine.InputSystem.Gamepad.current;
		    UnityEngine.InputSystem.Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;
		    int device = (pad != null && (keyboard == null || pad.lastUpdateTime > keyboard.lastUpdateTime)) ? 1 : 0;
		    if (device == promptDevice) return;
		    promptDevice = device;
		    if (prompts == null)
		    {
		        prompts = Resources.FindObjectsOfTypeAll<Text>();
		        promptTexts = new string[prompts.Length];
		        for (int i = 0; i < prompts.Length; i++) promptTexts[i] = prompts[i].text;
		    }
		    for (int i = 0; i < prompts.Length; i++)
		    {
		        if (prompts[i] == null || !prompts[i].gameObject.scene.IsValid() || !promptTexts[i].Contains("Press E")) continue;
		        prompts[i].text = promptTexts[i]
		            .Replace("Press Enter", device == 1 ? "Press A" : "Press Enter")
		            .Replace("Press ESC to quit", device == 1 ? "Press Start+Select to quit" : "Press Shift+ESC to quit");
		    }
		}

		void Start ()
		{
	        instance = this;
	        shadowVector = Vector3.down + Vector3.left;
	        if (!started)
	        {
	            lineHeightOffset = 70f;
	            for (int i = 0; i < disableWhenStarted.Length; i++)
	            {
	                disableWhenStarted[i].SetActive(true);
	            }
	            for (int i = 0; i < enableWhenStarted.Length; i++)
	            {
	                enableWhenStarted[i].SetActive(false);
	            }
	        }
		}

		void Update ()
		{
		    updatePrompts();
	        if (!started)
	        {
	            if (TaloketoInputManager.GetButtonDown("Submit"))
	            {
	                started = true;
	                firstOffsetDrop = true;
	                for (int i = 0; i < disableWhenStarted.Length; i++)
	                {
	                    disableWhenStarted[i].SetActive(false);
	                }
	            }
	        }

	        // In the collection: the Escape quit is gone, the collection has its own exit.

	        if (LineManager.SrgbSplit > 0f)
	        {
	            LineManager.SrgbSplit -= dt * (1f + LineManager.SrgbSplit);
	            if (LineManager.SrgbSplit < 0f)
	            {
	                LineManager.SrgbSplit = 0f;
	            }
	        }

			dt = Time.deltaTime;
			time += dt;

	        lineRhythm = Game.time % 2f;
	        if (lineRhythm > 1f)
	        {
	            lineRhythm = 2f - lineRhythm;
	        }
	        LineManager.Sheight = lineRhythm * 0.04f + lineHeightOffset;
	        if (firstOffsetDrop)
	        {
	            lineHeightOffset -= dt * lineHeightOffset;
	            if (lineHeightOffset <= 0f)
	            {
	                firstOffsetDrop = false;
	                for (int i = 0; i < enableWhenStarted.Length; i++)
	                {
	                    enableWhenStarted[i].SetActive(true);
	                }
	                Game.screenShakeMedium();
	                lineHeightOffset = 0f;
	            }
	        }
	        if (lineHeightOffset > 0f)
	        {
	            lineHeightOffset -= dt;
	            if (lineHeightOffset < 0f)
	            {
	                lineHeightOffset = 0f;
	            }
	        }

			MousePosition.get = Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
			MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = TaloketoInputManager.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;

	        if (screenShake != screenShakeOld)
	        {
	            if (screenShake > 0f)
	            {
	                transform.position = WorldWander.camPos + Geometry.createVector3(Random.value * 360f, screenShake * 0.4f);
	                screenShakeOld = screenShake;
	                screenShake -= dt;
	                if (screenShake < 0f)
	                {
	                    screenShake = 0f;
	                    transform.position = WorldWander.camPos;
	                }
	            }
	        }
		}

	    public void startTalking(string s, SpriteRenderer sprite)
	    {
	        enableWhenTalking.SetActive(true);
	        talkText.gameObject.SetActive(true);
	        talkText.text = s;
	        if (sprite != null)
	        {
	            talkImage.gameObject.SetActive(true);
	            talkImage.sprite = sprite.sprite;
	            talkImage.color = sprite.color;
	            talkImage.SetNativeSize();
	        }
	    }

	    public void stopTalking()
	    {
	        enableWhenTalking.SetActive(false);
	        talkText.gameObject.SetActive(false);
	        talkImage.gameObject.SetActive(false);
	    }

	    public static void screenShakeSmall()
	    {
	        if (screenShake < 0.2f)
	        {
	            screenShake += 0.2f;
	        }
	    }

	    public static void screenShakeMedium()
	    {
	        if (screenShake < 0.4f)
	        {
	            screenShake += 0.4f;
	        }
	    }

	    public static void screenShakeLarge()
	    {
	        if (screenShake < 1f)
	        {
	            screenShake += 1f;
	        }
	    }
	}
}
