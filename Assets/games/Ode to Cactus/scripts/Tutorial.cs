using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.OdeToCactus
{
	public class Tutorial : MonoBehaviour {
	    private TintScript tint;
		void Start ()
	    {
	        tint = GetComponent<TintScript>();
		}

		void Update ()
	    {
		    if (new Vector2(TaloketoInputManager.GetAxisRaw("Horizontal"), TaloketoInputManager.GetAxisRaw("Vertical")) != Vector2.zero || TaloketoInputManager.GetButton("Fire"))
	        {
	            tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, tint.selfColor.a - Game.dt / 3f);
	            if (tint.selfColor.a <= 0f)
	            {
	                Destroy(gameObject);
	            }
	        }
		}
	}
}
