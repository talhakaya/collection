using UnityEngine;
using System.Collections;

public class Bush : MonoBehaviour {
	public Sprite[] sprites;
	private SpriteRenderer spriteRenderer;
//	private TintScript tint;
	private float period = 5f;
	private float maxAngle = 15f;
	private float maxScaleX = 0.5f;
	private float maxScaleY = 0.5f;
	private float minScaleX = 1f;
	private float minScaleY = 1f;
	private float randomTime = 0f;

	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
//		tint = GetComponent<TintScript> ();
		spriteRenderer.sprite = sprites [Random.Range (0, sprites.Length)];
		minScaleX = transform.localScale.x;
		minScaleY = transform.localScale.y;
		randomTime = Random.Range(0f, 2f);
	}

	void Update ()
	{
		float ratio = (Game.time % (period * 2)) / period;
		float ratio2 = (ratio + 0.5f) % 2f;
		float ratio3 = (ratio + 1.5f) % 2f;
		ratio = (ratio + randomTime) % 2f;
		ratio2 = (ratio2 + randomTime) % 2f;
		float scaleX = 1f;
		float scaleY = 1f;
		float angle = 0f;
		if (ratio < 1f)
		{
			scaleX = minScaleX + (maxScaleX) * ratio;
		}
		else
		{
			scaleX = minScaleX + (maxScaleX) * (2 - ratio);
		}
		if (ratio3 < 1f)
		{
			angle = 2 * ratio3 * maxAngle - maxAngle;
		}
		else
		{
			angle = 2 * (2 - ratio3) * maxAngle - maxAngle;
		}
		if (ratio2 < 1f)
		{
			scaleY = minScaleY + (maxScaleY) * ratio2;
		}
		else
		{
			scaleY = minScaleY + (maxScaleY) * (2 - ratio2);
		}
		transform.localScale = new Vector3(scaleX, scaleY, 1f);
		transform.eulerAngles = Vector3.forward * angle;
	}
}
