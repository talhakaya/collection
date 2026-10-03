using UnityEngine;
using System.Collections;

namespace Games.Gridsplosion
{
	public class PlayerHead : MonoBehaviour {

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {
	        float goalEuler = transform.parent.eulerAngles.z + Geometry.differenceOfAnglesNegative(Geometry.angleOfVector3(MousePosition.get - transform.position), transform.parent.eulerAngles.z) * 0.45f;
	        transform.eulerAngles += Vector3.forward * Geometry.differenceOfAnglesNegative(goalEuler, transform.eulerAngles.z) * 4f * Game.dt;
		}
	}
}
