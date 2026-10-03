using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class BossVulnerableAnim : AnimBehavior {
	    private BossVulnerablePhysBox phys;

	    protected new void Start() {
	        sprites = new Sprite[1] { GetComponent<SpriteRenderer>().sprite };
	        period = 0.3f;
	        phys = GetComponent<BossVulnerablePhysBox>();
	        base.Start();
	    }

	    protected new void Update() {
	        base.Update();
	        if (phys.invincibilityTime > 0f) {
	            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Random.Range(0f, 1f));
	        }
	        else {
	            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
	        }
	    }
	}
}
