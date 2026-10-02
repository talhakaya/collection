using UnityEngine;
using System.Collections;

public class FirstScreen : MonoBehaviour {

	bool clicked = false;

	void Start ()
	{
		SpriteEffect.blurConst = 0;
		SpriteEffect.make (Effect.Blur, gameObject, false, false, null, Vector2.zero);
		Screen.showCursor = false;
	}

	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

		if (Input.GetMouseButtonDown(0))
		{
			clicked = true;
		}

		if (clicked)
		{
			SpriteEffect.blurConst += 0.5f * Time.deltaTime;

			if (SpriteEffect.blurConst >= 2f)
			{
				Application.LoadLevel(1);
			}
		}
	}
}
