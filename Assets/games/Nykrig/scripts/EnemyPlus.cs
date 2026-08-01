using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class EnemyPlus : Enemy {
	    public enum State {
	        Move,
	        Scale
	    }
	    public State state;
	    private bool moving;
	    private int movingCounter;
	    private float movingTimer;
	    private const float period = 0.2f;
	    protected float speed = 4f;
	    private Vector2 bodyVel;
	    public Transform[] plus;
	    public Transform[] plusLight;
	    private float scaleBegin;
	    private float scaleTimer;
	    private float scalePeriod;
	    private float scaleGoal;
	    public DeathTrigger[] deathTrigger;

	    void Awake() {
	        AwakeEnemy();
	        scaleBegin = plus[0].localScale.x;
	    }

	    void Update() {
	        UpdateEnemy();
	        movingTimer += Game.dt;
	        if (state == State.Move && movingTimer >= period) {
	            movingTimer = 0f;
	            Vector2 pos = transform.position;
	            moving = !isDead && Game.arePosLit(new Vector2[4] {
	                pos + new Vector2(0.25f, 0.25f),
	                pos + new Vector2(-0.25f, 0.25f),
	                pos + new Vector2(0.25f, -0.25f),
	                pos + new Vector2(-0.25f, -0.25f)
	            }, true);
	            movingCounter = (moving ? movingCounter + 1 : 0);
	            for (int i = 0, len = deathTrigger.Length; i < len; i++) {
	                deathTrigger[i].enabled = movingCounter > 1;
	            }
	        }
	    }

	    void FixedUpdate() {
	        FixedUpdateEnemy();
	        if ((moving || state == State.Scale) && !isDead) {
	            if (state == State.Move) {
	                float playerDist = Geometry.lengthOfVector3(Game.getPlayer(transform.position).transform.position - transform.position);
	                if (playerDist < 4) {
	                    body.linearVelocity = new Vector2(0f, 0f);
	                    state = State.Scale;
	                    scaleGoal = playerDist * 2f;
	                    scaleTimer = 0f;
	                    scalePeriod = 0.1f * scaleGoal;
	                }
	                else {
	                    body.linearVelocity = bodyVel + Geometry.normalizeVector2(Game.getPlayer(transform.position).transform.position - transform.position, speed * Game.dtPhysics);
	                    if (Geometry.lengthOfVector2(body.linearVelocity) > speed) {
	                        body.linearVelocity = Geometry.normalizeVector2(body.linearVelocity, speed);
	                    }
	                    bodyVel = body.linearVelocity;
	                }
	            }
	            else if (state == State.Scale) {
	                body.linearVelocity = new Vector2(0f, 0f);
	                scaleTimer += Game.dtPhysics;
	                if (scaleTimer < scalePeriod * 2f) {
	                    float animTimer = (scaleTimer < scalePeriod ? scaleTimer : 2f * scalePeriod - scaleTimer);
	                    float scale = Easing.SineEaseOut(animTimer, scaleBegin, scaleGoal - scaleBegin, scalePeriod);
	                    plus[0].localScale = new Vector3(scale, plus[0].localScale.y, plus[0].localScale.z);
	                    plus[1].localScale = new Vector3(plus[1].localScale.x, scale, plus[1].localScale.z);
	                    plusLight[0].localScale = new Vector3(1f, animTimer / scalePeriod, plusLight[0].localScale.z);
	                    plusLight[1].localScale = new Vector3(animTimer / scalePeriod, 1f, plusLight[1].localScale.z);
	                }
	                else {
	                    plus[0].localScale = new Vector3(scaleBegin, plus[0].localScale.y, plus[0].localScale.z);
	                    plus[1].localScale = new Vector3(plus[1].localScale.x, scaleBegin, plus[1].localScale.z);
	                    plusLight[0].localScale = new Vector3(0f, 0f, plusLight[0].localScale.z);
	                    plusLight[1].localScale = new Vector3(0f, 0f, plusLight[1].localScale.z);
	                    state = State.Move;
	                }
	            }
	        }
	        else {
	            body.linearVelocity = new Vector2(0f, 0f);
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
	        state = State.Move;
	        plus[0].localScale = new Vector3(scaleBegin, plus[0].localScale.y, plus[0].localScale.z);
	        plus[1].localScale = new Vector3(plus[1].localScale.x, scaleBegin, plus[1].localScale.z);
	        plusLight[0].localScale = new Vector3(0f, 0f, plusLight[0].localScale.z);
	        plusLight[1].localScale = new Vector3(0f, 0f, plusLight[1].localScale.z);
	        transform.eulerAngles = new Vector3(0f, 0f, Random.value < 0.5f ? 0f : 45f);
	    }
	}

}
