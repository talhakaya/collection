using UnityEngine;
using System.Collections;

namespace Games.TroubledFootball
{
	public class ButtonEarsore : MonoBehaviour {

		void Start ()
		{
			transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (0, 0, 0f)) + new Vector3(0.5f, 0.5f, 5f);
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
			AudioManager.musicOnOff = !AudioManager.musicOnOff;
		}
	}
}
