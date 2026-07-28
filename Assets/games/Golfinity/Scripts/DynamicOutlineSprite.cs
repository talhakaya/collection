using UnityEngine;
using System.Collections;

namespace Games.Golfinity
{
	public class DynamicOutlineSprite : OutlineSprite
	{
	    private void Update()
	    {
	        if (!isOn) return;
	        if (gos == null) return;
	        for (int i = 0; i < noOfSprites; i++)
	        {
	            gos[i].transform.localScale = Vector3.one;
	            gos[i].transform.position = transform.position + Vector3.forward * 5f + Geometry.createVector3(i * 360f / noOfSprites, outlineWidth);
	            gos[i].transform.localEulerAngles = Vector3.zero;
	        }
	    }
	}

}
