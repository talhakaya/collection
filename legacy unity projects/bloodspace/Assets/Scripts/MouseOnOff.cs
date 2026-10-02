using UnityEngine;
using System.Collections;

public class MouseOnOff : MonoBehaviour {

	private TintScript tint;
	private SpriteRenderer spriteRenderer;
	public Sprite off;
	public Sprite on;
	private bool mouseModeOld;

	void Start ()
	{
		tint = GetComponent<TintScript> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update ()
	{
		if (tint.selfColor.a > 0)
		{
			tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, tint.selfColor.a - 0.3f * Time.deltaTime);
		}

		if (mouseModeOld != Game.mouseMode)
		{
			tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, 1f);
		}

		if (Game.mouseMode)
		{
			spriteRenderer.sprite = on;
		}
		else
		{
			spriteRenderer.sprite = off;
		}

		mouseModeOld = Game.mouseMode;
	}
}
