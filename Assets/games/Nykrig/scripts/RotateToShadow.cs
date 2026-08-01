using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class RotateToShadow : MonoBehaviour {
	    public CanvasGroup canvasGroup;
	    public float alpha;
	    public float angle;

	    void OnEnable() {
	        if (canvasGroup != null) {
	            canvasGroup.alpha = 0f;
	        }
	    }

	    void Update() {
	        if (canvasGroup != null) {
	            canvasGroup.alpha = alpha + 4f * Geometry.lengthOfVector3(Game.shadowVector);
	        }
	        transform.eulerAngles = new Vector3(0f, 0f, angle + Geometry.angleOfVector3(Game.shadowVector));
	    }
	}

}
