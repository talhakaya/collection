using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Mover : MonoBehaviour {

    public Transform ceiling;
    public Transform wall;
    private float timer;
    public Image foreground;
    public Sprite fore;
    public Sprite wallstains;

	// Use this for initialization
	void Start () {

        GetComponent<AudioSource>().volume = 0.7f;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (LightStart.candles > 6 && wall.transform.eulerAngles.x < 80)
        {
            ceiling.position -= Game.dt * Vector3.forward * 0.5f;
            wall.Rotate(Vector3.right * 5 * Game.dt);
            GetComponent<AudioSource>().pitch += Game.dt / 32f;
        }

        if (Suicide.done)
        {
            GetComponent<AudioSource>().volume = 1f;
            GetComponent<AudioSource>().pitch = 3f;
            timer += Game.dt;
            if (timer % 0.2f < 0.1f)
            {
                foreground.sprite = fore;
            }
            else
            {
                foreground.sprite = wallstains;
            }
            if (timer > 2f)
            {
                Application.Quit();
            }
        }
	}
}
