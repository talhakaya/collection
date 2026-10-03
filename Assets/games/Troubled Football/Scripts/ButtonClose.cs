using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.TroubledFootball
{
	public class ButtonClose : MonoBehaviour {

		void Start ()
		{
			transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0f)) + new Vector3(-0.75f, -0.75f, 5f);
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
			GlobalInputManager.ReturnToMainMenu(); // In the collection: was Application.Quit()
		}
	}
}
