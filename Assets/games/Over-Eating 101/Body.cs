using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.OverEating101
{
	public class Body : MonoBehaviour {

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {
			if (TaloketoInputManager.GetAxisRaw("Horizontal") != 0)
			{
				transform.position += Vector3.right * TaloketoInputManager.GetAxisRaw("Horizontal") * 5 * Game.dt;
			}
		}
	}
}
