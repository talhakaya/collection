using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.identity;
		transform.Rotate (Vector3.forward * Random.Range (-5f, 5f));
	}
}
