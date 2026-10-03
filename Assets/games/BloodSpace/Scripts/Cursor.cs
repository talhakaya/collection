using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.BloodSpace
{
	public class Cursor : MonoBehaviour {

		private SpriteRenderer spriteRenderer;

		void Start ()
		{
			// In the collection: was Screen.showCursor = false. The collection draws the pointer,
			// so it is told to stop while this game draws its own.
			GlobalInputManager.HideGameCursor();
			spriteRenderer = GetComponent<SpriteRenderer> ();
			if (PlayerPrefs.GetInt ("BloodSpace.mouseMode", 0) == 0)
			{
				Game.mouseMode = false;
			}
			else
			{
				Game.mouseMode = true;
			}
		}

		void Update ()
		{
			transform.position = MousePosition.get ();
			if (TaloketoInputManager.GetButtonDown("MouseOnOff"))
			{
				Game.mouseMode = !Game.mouseMode;
				if (Game.mouseMode)
				{
					PlayerPrefs.SetInt("BloodSpace.mouseMode", 1);
				}
				else
				{
					PlayerPrefs.SetInt("BloodSpace.mouseMode", 0);
				}
			}

			float alpha = 1f;
			if (Game.mouseMode)
			{
				if (PlayerScript.instance != null)
				{
					float distance = Geometry.lengthOfVector3 (new Vector2(transform.position.x - PlayerScript.instance.position.x, transform.position.y - PlayerScript.instance.position.y));
					if (distance < 1f)
					{
						alpha = distance;
					}
				}
			}
			else if (PlayerScript.instance != null)
			{
				alpha = 0f;
			}

			spriteRenderer.color = new Color (spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
		}
	}
}
