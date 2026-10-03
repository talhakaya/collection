using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Collection.Controls;

namespace Games.Herbie
{
	public class End1 : MonoBehaviour {

		public GameObject herbie;
		public GameObject theend;
		private TintScript theendTint;

		void Start ()
		{
			theendTint = theend.GetComponent<TintScript> ();
			theendTint.selfColor = new Color(theendTint.selfColor.r, theendTint.selfColor.g, theendTint.selfColor.b, 0f);
		}

		void Update ()
		{
			if (herbie.transform.position.x > 2f)
			{
				HerbieEnd1.allowedToWalk = false;
				if (theendTint.selfColor.a < 1f)
				{
					theendTint.selfColor = new Color(theendTint.selfColor.r, theendTint.selfColor.g, theendTint.selfColor.b, theendTint.selfColor.a + Game.dt / 4f);
				}
				else
				{
					if (TaloketoInputManager.GetButtonDown("Select"))
					{
						SceneManager.LoadScene(SceneManager.GetActiveScene().path);
					}
				}
			}
		}
	}
}
