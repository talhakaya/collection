using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	void Awake ()
	{
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
        Cursor.visible = false;
	}
	
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
		
		dt = Time.deltaTime;
		time += dt;

		MousePosition.get = Camera.main.ScreenToWorldPoint (Input.mousePosition) + Vector3.forward;
		MousePosition.x = MousePosition.get.x;
		MousePosition.y = MousePosition.get.y;

		input = Input.GetMouseButton (0);
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
