using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class DynamicPhysBox : PhysBox {
	    [HideInInspector] public Vector2 input;
	    [HideInInspector] public Vector2 inputOld;
	    [HideInInspector] public bool inputJump;
	    [HideInInspector] public bool inputJumpOld;
	    [HideInInspector] public bool inputHoldUp;
	    [HideInInspector] public bool inputHoldDown;
	    [HideInInspector] public int airCounter;
	    [HideInInspector] public bool isGroundedOld;
	}
}
