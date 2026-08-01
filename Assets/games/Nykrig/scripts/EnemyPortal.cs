using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class EnemyPortal : Enemy {
	    private float timer;
	    private const float period = 2f;
	    private Vector3 firstScale;

	    void Awake() {
	        AwakeEnemy();
	        firstScale = transform.localScale;
	    }

	    void Update() {
	        UpdateEnemy();
	        if (!isDead) {
	            float scale = Mathf.Max(Mathf.Sqrt(health), 1f);
	            transform.localScale = new Vector3(firstScale.x * scale, firstScale.y * scale, 1f);
	            tints[0].transform.eulerAngles += new Vector3(0f, 0f, 50f * Game.dt);
	            tints[1].transform.eulerAngles -= new Vector3(0f, 0f, 50f * Game.dt);
	            timer += Game.dt;
	            if (timer >= period && !Game.instance.menu.gameObject.activeSelf) {
	                timer = 0f;
	                ObjectPool.enemySimpleSmallPool.get(transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0f)).GetComponent<Enemy>().Set();
	                TalhaAudioSource.PlayInstance(AudioResources.instance.spring, AudioType.Effect, null, 0.5f, 1.5f);
	            }
	        }
	    }

	    void FixedUpdate() {
	        FixedUpdateEnemy();
	    }

	    public override void Set() {
	        base.Set();
	        health = 10f;
	        timer = 0f;
	    }

	    public override float GetDamage(float damage, Vector2 hitPos) {
	        return base.GetDamage(damage, hitPos);
	    }
	}

}
