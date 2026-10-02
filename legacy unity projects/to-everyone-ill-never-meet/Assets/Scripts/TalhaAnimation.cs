using UnityEngine;
using System.Collections;

public class TalhaAnimation : MonoBehaviour {

	public Sprite[] sprites;
	public float period = 0.2f;
	private int i = 0;
	private SpriteRenderer spriteRenderer;
    public bool slightlyDifferentPeriod = true;

	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
		spriteRenderer.sprite = sprites[0];
        if (slightlyDifferentPeriod)
        {
            period *= Random.Range(0.8f, 1.2f);
        }
	}

	void Update ()
	{
		i = Mathf.FloorToInt((Game.time % (period * sprites.Length)) / period);
		spriteRenderer.sprite = sprites[i];
	}
}
