using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class EnemySimple : Enemy {
	    private bool moving;
	    private int movingCounter;
	    private float movingTimer;
	    private const float period = 0.2f;
	    protected float speed = 4f;
	    private Vector2 bodyVel;
	    private DeathTrigger deathTrigger;

	    void Awake() {
	        AwakeEnemy();
	        deathTrigger = GetComponent<DeathTrigger>();
	    }
		
		void Update () {
	        UpdateEnemy();
	        movingTimer += Game.dt;
	        if (movingTimer >= period) {
	            movingTimer = 0f;
	            Vector2 pos = transform.position;
	            moving = !isDead && Game.arePosLit(new Vector2[4] {
	                pos + new Vector2(0.25f, 0.25f),
	                pos + new Vector2(-0.25f, 0.25f),
	                pos + new Vector2(0.25f, -0.25f),
	                pos + new Vector2(-0.25f, -0.25f)
	            }, true);
	            movingCounter = (moving ? movingCounter + 1 : 0);
	            deathTrigger.enabled = movingCounter > 1;
	        }
	    }

	    void FixedUpdate() {
	        FixedUpdateEnemy();
	        if (moving && !isDead) {
	            body.linearVelocity = bodyVel + Geometry.normalizeVector2(Game.getPlayer(transform.position).transform.position - transform.position, speed * Game.dtPhysics);
	            if (Geometry.lengthOfVector2(body.linearVelocity) > speed) {
	                body.linearVelocity = Geometry.normalizeVector2(body.linearVelocity, speed);
	            }
	            bodyVel = body.linearVelocity;
	            body.angularVelocity = 90f;
	        }
	        else {
	            body.linearVelocity = new Vector2(0f, 0f);
	            body.angularVelocity = 0f;
	        }
	    }

	    public override float GetDamage(float damage, Vector2 hitPos) {
	        Vector2 pos = transform.position;
	        body.linearVelocity += Geometry.normalizeVector2(pos - hitPos, speed * damage * 0.1f);
	        return base.GetDamage(damage, hitPos);
	    }

	    public override void Set() {
	        base.Set();
	        movingTimer = 0f;
	        moving = false;
	        movingCounter = 0;
	        bodyVel = new Vector2(0f, 0f);
	        health = 5f;
	        speed = 4f;
	    }
	}

}
