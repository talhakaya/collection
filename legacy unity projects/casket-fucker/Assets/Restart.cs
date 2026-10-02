using UnityEngine;
using System.Collections;

public class Restart : MonoBehaviour {

    public int counter;

	void Start ()
    {
	
	}
	
	void Update ()
    {
	    if (Input.anyKeyDown)
        {
            counter--;
            if (counter < 0)
            {
                Application.LoadLevel(0);
            }
        }
	}
}
