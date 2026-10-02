using UnityEngine;
using System.Collections;

public class PrologueNextLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider thing)
	{
		if (thing.name == "Player")
		{
			PlayerScript.instance.canWalk = false;
			GameManagerScript.changeLevel("Prologue");
		}
	}
}
