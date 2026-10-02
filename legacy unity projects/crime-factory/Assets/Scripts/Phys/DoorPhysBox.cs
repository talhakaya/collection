using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPhysBox : PhysBox {
    public string nextLevel;

    protected override List<Property> GetPropertyTypes() {
        List<Property> res = base.GetPropertyTypes();
        res.Add(Property.GetCopy(PROP_NEXT_LEVEL));
        return res;
    }

    public override void SetProperty(string name, string val, int iDelta = 0, int jDelta = 0) {
        base.SetProperty(name, val, iDelta, jDelta);
        if (name == PROP_NEXT_LEVEL) {
            nextLevel = val;
            return;
        }
    }

    public override void SetProperty(string name, int val, int iDelta = 0, int jDelta = 0) {
        base.SetProperty(name, val, iDelta, jDelta);
        if (name == PROP_NEXT_LEVEL) {
            nextLevel = val.ToString();
            return;
        }
    }
}
