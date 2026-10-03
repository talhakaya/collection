using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class HoldablePhysBox : PhysBox {
	    public bool isAutoHoldLadder;
	    [HideInInspector] public bool isAlsoPlatform;
	    [HideInInspector] public bool isHoldableHanger;
	}
}
