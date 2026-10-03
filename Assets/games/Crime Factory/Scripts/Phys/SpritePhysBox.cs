using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class SpritePhysBox : PhysBox {
	    public Sprite[] sprites;

	    protected override List<Property> GetPropertyTypes() {
	        List<Property> res = base.GetPropertyTypes();
	        PropertyInt prop = Property.GetCopy(PROP_SPRITE) as PropertyInt;
	        prop.pMaxValue = sprites.Length - 1;
	        res.Add(prop);
	        return res;
	    }

	    public override void SetProperty(string name, int val, int iDelta = 0, int jDelta = 0) {
	        base.SetProperty(name, val, iDelta, jDelta);
	        if (name == PROP_SPRITE) {
	            GetComponent<SpriteRenderer>().sprite = sprites[val];
	            return;
	        }
	    }
	}
}
