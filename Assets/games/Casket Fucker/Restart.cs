using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Games.CasketFucker
{
	public class Restart : MonoBehaviour {

	    public int counter;

		void Start ()
	    {

		}

		void Update ()
	    {
		    if (Game.anyKeyDown)
	        {
	            counter--;
	            if (counter < 0)
	            {
	                SceneManager.LoadScene(SceneManager.GetActiveScene().path);
	            }
	        }
		}
	}
}
