using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Collection.Controls;

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
	        if (TaloketoInputManager.GetMouseButtonDown(0))
	        {
	            SceneManager.LoadScene("Assets/games/Space Artist/main.unity");
	        }
		}
	}

}
