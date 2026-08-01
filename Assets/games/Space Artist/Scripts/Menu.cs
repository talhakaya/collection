using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Games.SpaceArtist
{
	public class Menu : MonoBehaviour
	{

		void Start ()
	    {
		
		}
		
		void Update ()
	    {
	        Game.dt = Time.deltaTime;
	        if (Input.GetMouseButtonDown(0))
	        {
	            SceneManager.LoadScene("Assets/games/Space Artist/main.unity");
	        }
		}
	}

}
