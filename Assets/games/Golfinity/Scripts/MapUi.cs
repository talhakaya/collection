using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public abstract class MapUi : MonoBehaviour
	{
	    [HideInInspector] public bool uiUpdate;
	    public abstract void OnOver(bool isOver);
	    public abstract void OnClick();
	}

}
