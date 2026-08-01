using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class EnemyScale : Enemy {
	    public SpriteRenderer lightCircle;
	    private Vector3 lightCircleScale;
	    private TintScript tint;

	    void Awake() {
	        AwakeEnemy();
	        lightCircleScale = lightCircle.transform.localScale;
	        tint = tints[0];
	    }

	    void Update() {
	        UpdateEnemy();
	        if (isDead) {
	            tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, 1f);
	            lightCircle.transform.localScale = new Vector3(0f, 0f, 1f);
	        }
	        else {
	            if (health < 50) {
	                health += Game.dt;
	            }
	            float realScale = Mathf.Sqrt(health);
	            if (float.IsNaN(realScale) || float.IsInfinity(realScale)) {
	                realScale = 0f;
	            }
	            lightCircle.transform.localScale = new Vector3(lightCircleScale.x * realScale, lightCircleScale.y * realScale, 1f);
	            tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, Mathf.Min(1f, realScale));
	        }
	    }

	    void FixedUpdate() {
	        FixedUpdateEnemy();
	    }

	    public override void Set() {
	        base.Set();
	    }

	    public override float GetDamage(float damage, Vector2 hitPos) {
	        return base.GetDamage(damage, hitPos);
	    }
	}

}
