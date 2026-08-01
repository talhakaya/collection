using UnityEngine;
using System.Collections;
using System;

namespace Games.Nykrig
{
	public class Enemy : MonoBehaviour {
	    public TintScript[] tints;
	    protected Vector3 localScale;
	    public bool isDead;
	    public float health;
	    private float damageColorTimer;
	    protected Rigidbody2D body;
	    protected int score;

	    public void AwakeEnemy () {
	        body = GetComponent<Rigidbody2D>();
	        localScale = transform.localScale;
	        score = 1;
	    }
		
		public void UpdateEnemy() {
	        if (isDead) {
	            transform.localScale -= new Vector3(1f, 1f, 0f) * Game.dt * 4f;
	            if (transform.localScale.x <= 0f) {
	                gameObject.SetActive(false);
	            }
	        }
	        else {

	        }

	        if (damageColorTimer > 1f) {
	            damageColorTimer = 1f;
	            for (int i = 0, len = tints.Length; i < len; i++) {
	                tints[i].spriteEffectColor = new Color(1f, 0f, 0f, 1f);
	            }
	        }
	        if (damageColorTimer > 0f) {
	            damageColorTimer -= Game.dt * 5f;
	            for (int i = 0, len = tints.Length; i < len; i++) {
	                tints[i].spriteEffectColor = new Color(1f, 1f - damageColorTimer, 1f - damageColorTimer, 1f);
	            }
	        }
	        else {
	            for (int i = 0, len = tints.Length; i < len; i++) {
	                tints[i].spriteEffectColor = new Color(1f, 1f, 1f, 1f);
	            }
	        }
	    }

	    public void FixedUpdateEnemy() {
	        if (body.position.x > Game.levelMaxX) {
	            body.position = new Vector2(Game.levelMaxX, body.position.y);
	        }
	        if (body.position.y > Game.levelMaxY) {
	            body.position = new Vector2(body.position.x, Game.levelMaxY);
	        }
	        if (body.position.x < Game.levelMinX) {
	            body.position = new Vector2(Game.levelMinX, body.position.y);
	        }
	        if (body.position.y < Game.levelMinY) {
	            body.position = new Vector2(body.position.x, Game.levelMinY);
	        }
	    }

	    public virtual void Set() {
	        isDead = false;
	        damageColorTimer = 0f;
	        transform.localScale = localScale;
	        health = 1f;
	    }

	    public virtual float GetDamage(float damage, Vector2 hitPos) {
	        if (!isDead) {
	            health -= damage * 0.5f;
	            damageColorTimer = damage * 0.3f;
	            if (health <= 0f) {
	                isDead = true;
	                health = 0f;
	                TalhaAudioSource.PlayInstance(AudioResources.instance.hitPlastic, AudioType.Effect, null, 0.2f * damage, 1.5f);
	                TalhaAudioSource.PlayInstance(AudioResources.instance.breakStick, AudioType.Effect);
	                if (this is EnemyBoss) {
	                    Game.enemyCount = 0;
	                }
	                if (!(this is EnemySimpleSmall)) {
	                    Game.score += score;
	                    Game.enemyCount--;
	                }
	            }
	            else {
	                TalhaAudioSource.PlayInstance(AudioResources.instance.hitPlastic, AudioType.Effect, null, 0.2f * damage, 1.5f);
	            }
	        }
	        return health;
	    }
	}

}
