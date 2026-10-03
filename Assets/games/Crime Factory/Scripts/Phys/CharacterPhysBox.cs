using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class CharacterPhysBox : DynamicPhysBox {
	    [HideInInspector] public float fireTime;
	    [HideInInspector] public float fireCoolingTime;
	    [HideInInspector] public bool onLadder;
	    [HideInInspector] public PhysBox ladderActive;
	    [HideInInspector] public ClimbableTile ladderActiveClimbable;
	    [HideInInspector] public List<PhysBox> ladders;
	    [HideInInspector] public List<ClimbableTile> ladderClimbables;
	    [HideInInspector] public float dontGoOnLadderTimer;
	    [HideInInspector] public int hp;
	    [HideInInspector] public float invincibilityTime;
	    [HideInInspector] public float waitTime;
	    public int hpMax;
	    public int moneyToDrop;
	    public float invincibilityTimeMax = 0.5f;
	}
}
