using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public GameObject text;
	private float period = 2f;
	private float maxScale = 1.1f;
	private float firstScale = 0.3f;
	private float lastTime = 0f;

	void Start ()
	{
		Game.miniGame = gameObject;
		Game.isThereMiniGame = true;
	}

	void Update () {
		float ratio = (Game.time % (period * 2)) / period;
		float ratio2 = (ratio + 0.5f) % 2f;
		float scaleX = 1f;
		float scaleY = 1f;
		if (ratio < 1f)
		{
			scaleX = 1f + (maxScale - 1f) * ratio;
		}
		else
		{
			scaleX = 1f + (maxScale - 1f) * (2 - ratio);
		}
		if (ratio2 < 1f)
		{
			scaleY = 1f + (maxScale - 1f) * ratio2;
		}
		else
		{
			scaleY = 1f + (maxScale - 1f) * (2 - ratio2);
		}
		text.transform.localScale = new Vector3(firstScale * scaleX / transform.localScale.x, firstScale * scaleY / transform.localScale.y, 1f / transform.localScale.z);
		if (Input.GetButtonDown("Interact") || Input.GetButtonDown("Select"))
		{
			Game.time = 0f;
			Game.endMiniGame();
		}
		float soundTime = Game.time % 0.5f;
		if (lastTime < 0.25f && soundTime >= 0.25f)
		{
			Game.instance.aBush.pitch = Random.Range(0.5f, 1f);
			Game.instance.aBush.Play ();
		}
		lastTime = soundTime;
	}
}
