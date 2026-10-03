using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.ChildhoodNightmare
{
	public class FirstScreen : MonoBehaviour {

		bool clicked = false;

		void Start ()
		{
			SpriteEffect.blurConst = 0;
			SpriteEffect.make (Effect.Blur, gameObject, false, false, null, Vector2.zero);
			GlobalInputManager.HideGameCursor(); // In the collection: was Screen.showCursor = false
			// In the collection: the process outlives the game, so the statics start over here.
			TintScript.weatherColor = new Color(1f, 1f, 1f);
			SpriteEffect.rgbSplitConst = 1f;
		}

		void Update ()
		{
			// In the collection: the Escape-quit is gone (the collection has its own exit).

			// In the collection: the gamepad's A (and Space) start it as well as the mouse.
			if (TaloketoInputManager.GetMouseButtonDown(0) || TaloketoInputManager.GetButtonDown("Jump"))
			{
				clicked = true;
			}

			if (clicked)
			{
				SpriteEffect.blurConst += 0.5f * Time.deltaTime;

				if (SpriteEffect.blurConst >= 2f)
				{
					UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/games/Childhood Nightmare/scene1.unity"); // In the collection: was LoadLevel(1)
				}
			}
		}
	}
}
