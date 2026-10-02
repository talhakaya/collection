using UnityEngine;
using System.Collections;

public class Bg : MonoBehaviour {

    private float timeCounter;
    private float turnSpeed;
    private static float period = 3.2f;

	void Start ()
    {
        
	}
	
	void Update ()
    {
        timeCounter -= Game.dt;
        transform.Rotate(Vector3.forward * turnSpeed * Game.dt);
        if (timeCounter <= 0f)
        {
            Reset();
        }
	}

    void Reset()
    {
        turnSpeed = Random.Range(-100f, 100f);
        timeCounter += period;
    }
}
