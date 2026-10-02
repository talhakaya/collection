using UnityEngine;
using System.Collections;

public class TimeCounter : MonoBehaviour
{
    private TextMesh textMesh;
    public float timer;

	void Start ()
    {
        textMesh = GetComponent<TextMesh>();
	}
	
	void Update ()
    {
	    if (timer > 0f)
        {
            timer -= Game.dt;
            if (timer <= 0f)
            {
                transform.parent.GetComponent<MiniGame>().end();
            }
        }
        else
        {
            timer = 0f;
        }
        textMesh.text = "Time left: " + timer.ToString("f2");
	}
}
