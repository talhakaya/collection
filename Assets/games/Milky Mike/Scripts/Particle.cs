using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class Particle : MonoBehaviour {
	    private float angle;
	    private float scale;
	    private float timer;
	    private float period;
	    private float speed;
	    private SpriteRenderer sprite;
	    public float minAngle = 250f;
	    public float maxAngle = 290f;
	    public float minScale = 0.6f;
	    public float maxScale = 1.4f;
	    public float minPeriod = 0.6f;
	    public float maxPeriod = 1.4f;
	    public float minSpeed = 10f;
	    public float maxSpeed = 20f;

	    void OnEnable() {
	        sprite = GetComponent<SpriteRenderer>();
	        angle = Random.Range(minAngle, maxAngle);
	        transform.eulerAngles = new Vector3(0f, 0f, angle);
	        scale = Random.Range(minScale, maxScale);
	        timer = 0f;
	        period = Random.Range(minPeriod, maxPeriod);
	        speed = Random.Range(minSpeed, maxSpeed);
	        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
	        transform.localScale = new Vector3(scale, scale, 1f);
	    }

		void Update () {
	        timer += Game.dt;
	        if (timer > period) {
	            gameObject.SetActive(false);
	        }
	        else {
	            transform.position += Geometry.createVector3(angle, speed / scale * Game.dt);
	            if (timer < period * 0.1f) {
	                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, timer / (period * 0.1f));
	            }
	            else {
	                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f - (timer - period * 0.1f) / (period * 0.9f));
	            }
	            transform.localScale = new Vector3(scale, scale, 1f) * (1f - timer / period);
	        }
	    }
	}
}
