using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class BossVulnerablePhysBox : PhysBox {
	    [HideInInspector] public int hp;
	    [HideInInspector] public float invincibilityTime;
	}
}
