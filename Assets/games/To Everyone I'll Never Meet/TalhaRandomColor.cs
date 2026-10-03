using UnityEngine;
using System.Collections;

namespace Games.ToEveryoneIllNeverMeet
{
	public class TalhaRandomColor : MonoBehaviour {

		// Use this for initialization
		void Start () {
	        GetComponent<TintScript>().selfColor = Game.colors[Random.Range(0, Game.colors.Length)];
		}

		// Update is called once per frame
		void Update () {

		}
	}
}
