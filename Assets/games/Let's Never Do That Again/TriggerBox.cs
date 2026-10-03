using UnityEngine;
using System.Collections;

namespace Games.LetsNeverDoThatAgain
{
	public class TriggerBox : MonoBehaviour {

	    void OnTriggerExit2D(Collider2D other)
	    {
	        if (other.name == "Penguin" && other.transform.position.x > transform.position.x)
	        {
	            GetComponent<Collider2D>().isTrigger = false;
	        }
	    }
	}
}
