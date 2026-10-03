using UnityEngine;

namespace Games.CrimeFactory
{
	public class DynamicSolidPhysBox : CharacterPhysBox {
	    [HideInInspector] public int iBroadPhaseMin;
	    [HideInInspector] public int jBroadPhaseMin;
	    [HideInInspector] public int iBroadPhaseMax;
	    [HideInInspector] public int jBroadPhaseMax;
	}
}
