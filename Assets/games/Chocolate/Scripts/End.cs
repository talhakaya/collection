using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Chocolate
{
	public class End : MonoBehaviour {

		// Use this for initialization
		void Start () {
			// In the collection: this is the game's end, which in the story mode is what wins
			// its artifact.
			if (!Collection.Story.StoryGames.Finish())
			{
				GlobalInputManager.ReturnToMainMenu();
			}
		}

		// Update is called once per frame
		void Update () {

		}
	}
}
