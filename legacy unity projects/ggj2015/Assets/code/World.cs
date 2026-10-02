using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {

	void Start ()
	{
	
	}

	void Update ()
	{
		transform.localPosition = -transform.parent.localPosition;
	}
}
