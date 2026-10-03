using UnityEngine;
using System.Collections;

namespace Games.AzerAVM
{
	public class Menu : MonoBehaviour {

		void Start ()
	    {

		}

		void Update ()
	    {
	        Game.dt = Time.deltaTime;
	        Game.time += Game.dt;

	        // In the collection: the Escape-quit is gone (the collection has its own exit).

	        if (PlayerScript.anyKeyDown())
	        {
	            UnityEngine.SceneManagement.SceneManager.LoadScene(Game.ScenePath); // In the collection: was LoadLevel(1)
	        }
		}
	}
}
