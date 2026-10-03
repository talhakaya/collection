using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class MovingPlatformPhysBox : ClimbablePhysBox {
	    public float speed = Simulator.MOVING_PLATFORM_SPEED;
	    public bool isUpDown;
	    [HideInInspector] public float waitTime;
	    [HideInInspector] public int iBroadPhaseMin;
	    [HideInInspector] public int jBroadPhaseMin;
	    [HideInInspector] public int iBroadPhaseMax;
	    [HideInInspector] public int jBroadPhaseMax;

	    protected override List<Property> GetPropertyTypes() {
	        List<Property> res = base.GetPropertyTypes();
	        res.Add(Property.GetCopy(PROP_SPEED));
	        res.Add(Property.GetCopy(PROP_DIR));
	        return res;
	    }

	    public override void SetProperty(string name, float val, int iDelta = 0, int jDelta = 0) {
	        base.SetProperty(name, val, iDelta, jDelta);
	        if (name == PROP_SPEED) {
	            speed = val;
	            return;
	        }
	    }

	    public override void SetProperty(string name, int val, int iDelta = 0, int jDelta = 0) {
	        base.SetProperty(name, val, iDelta, jDelta);
	        if (name == PROP_DIR) {
	            isUpDown = val == 1;
	            return;
	        }
	    }
	}
}
