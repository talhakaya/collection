using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.KayabrosPrototype1
{
	public class Tile : MonoBehaviour {

		// Use this for initialization
		void Start () {
	        GetComponent<TintScript>().selfColor = GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.4f, 0.6f), Random.Range(0.0f, 0.2f), Random.Range(0.4f, 0.6f));
		}

		// Update is called once per frame
		void Update () {

		}
	}
}
