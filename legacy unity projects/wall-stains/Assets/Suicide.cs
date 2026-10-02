using UnityEngine;
using System.Collections;

public class Suicide : MonoBehaviour {

    public static bool done;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        done = true;
    }
}
