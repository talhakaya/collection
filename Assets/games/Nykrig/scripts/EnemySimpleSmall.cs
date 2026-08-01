using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class EnemySimpleSmall : EnemySimple {
	    public override void Set() {
	        base.Set();
	        health = 1f;
	        speed = 2f;
	        score = 0;
	    }
	}

}
