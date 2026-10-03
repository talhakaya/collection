using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class Dust : MonoBehaviour {
	    private SpriteRenderer sr;
	    private float timer;
	    private float period;
	    private float speed;
	    public Sprite[] sprites;
	    public Color color;
	    public float dir;

	    void Awake() {
	        sr = GetComponent<SpriteRenderer>();
	    }

	    private void OnEnable() {
	        timer = 0f;
	        speed = Random.Range(0f, 1.5f);
	        period = Random.Range(0.8f, 1.5f);
	        sr.sprite = sprites[Random.Range(0, sprites.Length)];
	        transform.localScale = new Vector3(0f, 0f, 1f);
	    }

	    void Update() {
	        if (Simulator.IsPaused) return;
	        timer += Platformer.dt;
	        if (timer < period) {
	            float animRatio = timer / period;
	            float scale = Mathf.Pow(animRatio, 0.5f);
	            transform.localScale = new Vector3(dir * scale, scale, 1f);
	            if (animRatio > 0.5f) {
	                sr.color = new Color(color.r, color.g, color.b, (1f - (animRatio - 0.5f) * 2f));
	            }
	            else {
	                sr.color = new Color(color.r, color.g, color.b, 1f);
	            }
	            transform.position += new Vector3(speed * dir * Time.deltaTime, 0f, 0f);
	        }
	        else {
	            gameObject.SetActive(false);
	        }
	    }
	}
}
