using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class Fire : MonoBehaviour {
	    private float power;
	    private Rigidbody2D body;
	    public float speed = 2f;
	    private bool isDead;
	    private float timer;

	    void Awake() {
	        body = GetComponent<Rigidbody2D>();
	    }

	    void FixedUpdate() {
	        if (transform.position.x > Game.levelMaxX + 1f) {
	            gameObject.SetActive(false);
	        }
	        if (transform.position.y > Game.levelMaxY + 1f) {
	            gameObject.SetActive(false);
	        }
	        if (transform.position.x < Game.levelMinX - 1f) {
	            gameObject.SetActive(false);
	        }
	        if (transform.position.y < Game.levelMinY - 1f) {
	            gameObject.SetActive(false);
	        }
	        if (isDead) {
	            transform.localScale -= new Vector3(1f, 1f, 0f) * Game.dtPhysics * 4f;
	            if (transform.localScale.x <= 0f) {
	                gameObject.SetActive(false);
	            }
	        }
	        timer += Game.dtPhysics;
	    }

	    public void Set(float powerSqrt, Vector2 velocity, Vector2 playerVelocity, Vector3 pos) {
	        power = powerSqrt * powerSqrt;
	        transform.localScale = new Vector3(0.5f * powerSqrt, 0.5f * powerSqrt, 1f);
	        body.linearVelocity = velocity * speed + playerVelocity;
	        float angle = Geometry.angleOfVector2(body.linearVelocity);
	        transform.eulerAngles = new Vector3(0f, 0f, angle - 90);
	        transform.position = pos;
	        timer = 0f;
	        isDead = false;
	    }

	    void OnTriggerEnter2D(Collider2D other) {
	        if (!isDead) {
	            if (other.tag == "Enemy" || other.tag == "EnemyChild") {
	                Enemy e = other.tag == "Enemy" ? other.GetComponent<Enemy>() : other.transform.parent.GetComponent<Enemy>();
	                float eHealth = e.health;
	                float eHealthRemaining = e.GetDamage(power + Geometry.lengthOfVector2(body.linearVelocity) * 0.1f, body.position);
	                if (eHealthRemaining > 0) {
	                    body.linearVelocity = new Vector2(0f, 0f);
	                    isDead = true;
	                }
	                else {
	                    power -= eHealth;
	                    if (power > 0) {
	                        float powerSqrt = Mathf.Sqrt(power);
	                        transform.localScale = new Vector3(0.5f * powerSqrt, 0.5f * powerSqrt, 1f);
	                    }
	                    else {
	                        body.linearVelocity = new Vector2(0f, 0f);
	                        isDead = true;
	                    }
	                    for (int i = 0, len = Random.Range(1, 4); i < len; i++) {
	                        ObjectPool.lightPermanentPool.get(transform.position).GetComponent<Lightt>().Set(Random.Range(1f, 4f), false);
	                    }
	                }
	                ObjectPool.lightPool.get(transform.position).GetComponent<Lightt>().Set(power);
	            }
	            else if (other.tag == "Wall" && timer > 0.06f) {
	                body.linearVelocity = new Vector2(0f, 0f);
	                isDead = true;
	            }
	        }
	    }

	    void OnTriggerStay2D(Collider2D other) {
	        if (!isDead) {
	            if (other.tag == "Wall" && timer > 0.04f) {
	                body.linearVelocity = new Vector2(0f, 0f);
	                isDead = true;
	            }
	        }
	    }
	}

}
