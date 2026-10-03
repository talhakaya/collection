using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class Bar : MonoBehaviour {
	    public float animRatio;
	    private float scaleY;
	    private SpriteRenderer sprite;

	    void Start () {
	        scaleY = transform.localScale.y;
	        sprite = GetComponent<SpriteRenderer>();
	        transform.localScale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);
	    }

		void Update () {
	        transform.localScale = new Vector3(transform.localScale.x, animRatio * scaleY, transform.localScale.z);
	        sprite.color = new Color(1f - animRatio * 0.8f, 0.2f + animRatio * 0.8f, 0.2f);
	    }
	}
}
