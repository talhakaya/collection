using UnityEngine;
using System.Collections;

namespace Games.TroubledFootball
{
	public class ButtonRestart : MonoBehaviour {

		void Start ()
		{
			transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (0, Screen.height, 0f)) + new Vector3(0.75f, -0.75f, 5f);
		}

		void Update ()
		{
			if (GameManager.clickedOn(gameObject))
			{
				OnMouseUp();
			}
		}

		void OnMouseUp()
		{
			Ball.instance.init ();
		}
	}
}
