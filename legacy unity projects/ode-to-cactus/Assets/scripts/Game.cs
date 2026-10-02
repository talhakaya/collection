using UnityEngine;
using System.Collections;

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
	}
	
	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKey(KeyCode.KeypadEnter))
        {
            dt = 10 * Time.deltaTime;
        }
        else
        {
            dt = Time.deltaTime;
        }
		
		time += dt;

		MousePosition.get = Camera.main.ScreenToWorldPoint (Input.mousePosition) + Vector3.forward;
		MousePosition.x = MousePosition.get.x;
		MousePosition.y = MousePosition.get.y;

		input = Input.GetMouseButton (0);
		inputDown = input && !inputOld;
		inputUp = !input && inputOld;

		inputOld = input;
	}

    public static void nextLevel()
    {
        if (sceneNo == -1)
        {
            Application.LoadLevel("road");
        }
        else if (sceneNo == 0)
        {
            Application.LoadLevel("noise");
        }
        else if (sceneNo == 1)
        {
            Application.LoadLevel("mondo");
        }
        else if (sceneNo == 2)
        {
            Application.LoadLevel("noise");
        }
        else if (sceneNo == 3)
        {
            Application.LoadLevel("norr");
        }
        else if (sceneNo == 4)
        {
            Application.LoadLevel("noise");
        }
        else if (sceneNo == 5)
        {
            Application.LoadLevel("asia");
        }
        else if (sceneNo == 6)
        {
            Application.LoadLevel("noise");
        }
        else if (sceneNo == 7)
        {
            Application.LoadLevel("norr2");
        }
        else if (sceneNo == 8)
        {
            Application.LoadLevel("noise");
        }
        else if (sceneNo == 9)
        {
            Application.LoadLevel("end");
        }
        else if (sceneNo == 10)
        {
            Application.LoadLevel("noise");
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
