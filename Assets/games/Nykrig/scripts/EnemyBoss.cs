using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class EnemyBoss : Enemy {
	    private const float period = 0.2f;
	    protected float speed = 2f;
	    protected float turnSpeed = 15f;
	    public Rigidbody2D[] plus;
	    public Enemy[] enemies;

	    void Awake() {
	        AwakeEnemy();
	    }

	    void Update() {
	        UpdateEnemy();
	    }

	    void FixedUpdate() {
	        FixedUpdateEnemy();
	        if (body.position.x > Game.levelMaxX - 1f) {
	            body.position = new Vector2(Game.levelMaxX - 1f, body.position.y);
	        }
	        if (body.position.y > Game.levelMaxY - 1f) {
	            body.position = new Vector2(body.position.x, Game.levelMaxY - 1f);
	        }
	        if (body.position.x < Game.levelMinX + 1f) {
	            body.position = new Vector2(Game.levelMinX + 1f, body.position.y);
	        }
	        if (body.position.y < Game.levelMinY + 1f) {
	            body.position = new Vector2(body.position.x, Game.levelMinY + 1f);
	        }
	        Vector2 vec = Geometry.normalizeVector2(Game.getPlayer(transform.position).transform.position - transform.position, speed * Game.dtPhysics);
	        //vec.y = 0f;
	        body.linearVelocity += vec;
	        if (Geometry.lengthOfVector2(body.linearVelocity) > speed) {
	            body.linearVelocity = Geometry.normalizeVector2(body.linearVelocity, speed);
	        }
	        plus[0].transform.eulerAngles += new Vector3(0f, 0f, Game.dtPhysics * turnSpeed);
	        plus[1].transform.eulerAngles += new Vector3(0f, 0f, Game.dtPhysics * Geometry.lengthOfVector2(body.linearVelocity) * turnSpeed);
	    }

	    public override float GetDamage(float damage, Vector2 hitPos) {
	        Vector2 pos = transform.position;
	        //body.velocity += Geometry.normalizeVector2(pos - hitPos, speed * damage * 0.1f);
	        return base.GetDamage(damage, hitPos);
	    }

	    public override void Set() {
	        base.Set();
	        health = 30f;
	        speed = 4f;
	        transform.eulerAngles = new Vector3(0f, 0f, Random.value < 0.5f ? 0f : 45f);
	        for (int i = 0, len = enemies.Length; i < len; i++) {
	            enemies[i].Set();
	        }
	    }
	}

}
