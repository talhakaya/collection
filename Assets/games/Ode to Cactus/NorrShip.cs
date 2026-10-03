using UnityEngine;
using System.Collections;

namespace Games.OdeToCactus
{
	public class NorrShip : MonoBehaviour {

	    void OnTriggerEnter2D(Collider2D other)
	    {
	        Game.nextLevel();
	    }
	}
}
