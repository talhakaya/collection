using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class Wall : MonoBehaviour {
	    public static float lightAlpha = 1f;
	    public Transform scale;
	    public SpriteRenderer spriteRenderer;
	    public SpriteRenderer lightt;
	    private float scaleTimer;
	    private float scalePeriod;

	    void OnEnable () {
	        scaleTimer = 0f;
	        scalePeriod = Random.Range(0.3f, 0.6f);
	    }
		
		// Update is called once per frame
		void Update () {
	        if (scaleTimer < scalePeriod) {
	            scaleTimer += Game.dt;
	            if (scaleTimer >= scalePeriod) {
	                scale.localScale = new Vector3(1f, 1f, 1f);
	            }
	            else {
	                float animRatio0 = Easing.SineEaseOut(scaleTimer, 0f, 1f, scalePeriod);
	                float animRatio1 = Easing.BackEaseOut(scaleTimer, 0f, 1f, scalePeriod);
	                if (transform.localScale.x > transform.localScale.y || (transform.localScale.x == transform.localScale.y && Random.value < 0.5f)) {
	                    scale.localScale = new Vector3(animRatio0, animRatio1, 1f);
	                }
	                else {
	                    scale.localScale = new Vector3(animRatio1, animRatio0, 1f);
	                }
	            }
	        }
	        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f - lightAlpha);
	        if (Player.nightVisionTimer > 0f) {
	            lightt.color = new Color(lightt.color.r, lightt.color.g, lightt.color.b, 1f);
	        }
	        else {
	            lightt.color = new Color(lightt.color.r, lightt.color.g, lightt.color.b, lightAlpha);
	        }
	    }
	}

}
