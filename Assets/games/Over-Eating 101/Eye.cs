using UnityEngine;
using System.Collections;

namespace Games.OverEating101
{
	public class Eye : MonoBehaviour {

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {
			transform.Rotate(Vector3.forward * Game.dt * 360);
		}
	}
}
