using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	void Start ()
	{
		Screen.showCursor = false;
		spriteRenderer = GetComponent<SpriteRenderer> ();
		if (PlayerPrefs.GetInt ("mouseMode", 0) == 0)
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
		if (Input.GetButtonDown("MouseOnOff"))
		{
			Game.mouseMode = !Game.mouseMode;
			if (Game.mouseMode)
			{
				PlayerPrefs.SetInt("mouseMode", 1);
			}
			else
			{
				PlayerPrefs.SetInt("mouseMode", 0);
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
