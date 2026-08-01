using System.Collections;
using System.Collections.Generic;
using Collection.Controls;
using UnityEngine;

namespace Games.Golfinity
{
	public class LogoPopup : Popup
	{
	    private void Update()
	    {
	        // Game.input is the mouse button only, so on a gamepad this never dismissed - the
	        // logo stayed up over the map and followed the player into the level.
	        if (Game.input || TaloketoInputManager.GetButtonDown("Throw") || TaloketoInputManager.GetButtonDown("Back"))
	        {
	            Hide();
	        }
	    }
	}

}
