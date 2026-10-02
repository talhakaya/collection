using UnityEngine;
using System.Collections;

public class FlickeringObject : MonoBehaviour {
	
	private bool isTk2d;
	private bool isRendered;
	private float originalAlpha;
	private tk2dSprite sprite;
	
	public float minAlpha;
	public float maxAlpha;
	public bool isFlickering;
	
	void Start ()
	{
		if (maxAlpha <= minAlpha)
		{
			maxAlpha = 1;
		}
		if (gameObject.GetComponent<tk2dSprite>() != null)
		{
			isTk2d = true;
			sprite = gameObject.GetComponent<tk2dSprite>();
			originalAlpha = sprite.color.a;
		}
		else if (renderer != null && renderer.material != null)
		{
			isRendered = true;
		}
	}
	
	void Update ()
	{
		if (isTk2d)
		{
			if (isFlickering)
			{
				sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Random.Range(minAlpha, maxAlpha));
			}
			else if (sprite.color.a != originalAlpha)
			{
				sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, originalAlpha);
			}
		}
		else if (isRendered)
		{
			if (isFlickering)
			{
				renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, Random.Range(minAlpha, maxAlpha));
			}
			else if (renderer.material.color.a != originalAlpha)
			{
				renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, originalAlpha);
			}
		}
	}
}
