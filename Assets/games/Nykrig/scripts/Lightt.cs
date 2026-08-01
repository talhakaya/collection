using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class Lightt : MonoBehaviour {
	    public SpriteRenderer lightt;
	    private float lightScale;
	    private Vector3 lightPos;
	    private Vector3 splatVector;
	    private float timer;
	    public float fadeInPeriod = 0.5f;
	    public float period = 2f;
	    private float scale;
	    private bool disappear;

	    void Awake () {
	        lightScale = lightt.transform.localScale.x;
	    }
		
		void Update () {
	        if (disappear) {
	            if (timer < fadeInPeriod) {
	                lightt.transform.localScale = new Vector3(lightScale * scale, lightScale * scale, 1f);
	                lightt.color = new Color(lightt.color.r, lightt.color.g, lightt.color.b, timer / fadeInPeriod);
	                timer += Game.dt;
	            }
	            else if (timer < period) {
	                float animRatio = Easing.SineEaseOut(timer - fadeInPeriod, 1f, -1f, period - fadeInPeriod);
	                lightt.transform.localScale = new Vector3(lightScale * scale * animRatio, lightScale * scale * animRatio, 1f);
	                lightt.color = new Color(lightt.color.r, lightt.color.g, lightt.color.b, 1f);
	                timer += Game.dt / scale;
	            }
	            else {
	                gameObject.SetActive(false);
	            }
	        }
	        else {
	            if (timer < period) {
	                timer += Game.dt / scale;
	                if (timer < period) {
	                    float animRatio = Easing.SineEaseOut(timer, 0f, 1f, period);
	                    lightt.transform.localScale = new Vector3(lightScale * scale * animRatio, lightScale * scale * animRatio, 1f);
	                    transform.position = lightPos + splatVector * animRatio;
	                }
	                else {
	                    lightt.transform.localScale = new Vector3(lightScale * scale, lightScale * scale, 1f);
	                    transform.position = lightPos + splatVector;
	                }
	            }
	        }
	    }

	    public void Set(float scale, bool disappear = true, float splatMult = 1f) {
	        this.disappear = disappear;
	        this.scale = Mathf.Sqrt(Mathf.Max(0f, scale));
	        
	        timer = 0f;
	        lightPos = transform.position;
	        if (!disappear) {
	            splatVector = splatMult * Geometry.createVector3(Random.Range(0f, 360f), Random.value);
	            lightt.color = new Color(0.7f, 0.7f, 0.7f);
	        }
	        else {
	            lightt.color = new Color(1f, 1f, 1f);
	        }
	    }
	}

}
