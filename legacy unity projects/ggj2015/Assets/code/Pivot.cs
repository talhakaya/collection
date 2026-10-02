using UnityEngine;
using System.Collections;

public class Pivot : MonoBehaviour {

	public static Pivot instance;

	void Start ()
	{
		instance = this;
		Game.fitInTileHalf (gameObject);
	}

	void Update ()
	{
	
	}
}
