using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class LogoPopup : Popup
	{
	    private void Update()
	    {
	        if (Game.input)
	        {
	            Hide();
	        }
	    }
	}

}
