using UnityEngine;
using System.Collections;

namespace Games.ToEveryoneIllNeverMeet
{
	public class LevelPass : MonoBehaviour
	{
	    public static int no = 0;

	    void OnTriggerEnter(Collider other)
	    {
	        if (other.tag == "Player")
	        {
	            Game.fadeOut = true;
	        }
	    }
	}
}
