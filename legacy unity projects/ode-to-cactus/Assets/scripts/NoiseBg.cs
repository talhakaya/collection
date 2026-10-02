using UnityEngine;
using System.Collections;

public class NoiseBg : MonoBehaviour {

    public float time;

	void Start ()
    {
        time = 0f;
	}
	
	void Update ()
    {
        time += Game.dt;
        if (time > 0.5f)
        {
            Game.nextLevel();
        }
	}
}
