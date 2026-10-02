using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	void Start ()
    {
	
	}
	
	void Update ()
    {
        Game.dt = Time.deltaTime;
        Game.time += Game.dt;

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.anyKeyDown)
        {
            Application.LoadLevel(1);
        }
	}
}
