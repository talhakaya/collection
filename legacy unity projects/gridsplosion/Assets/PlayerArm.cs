using UnityEngine;
using System.Collections;

public class PlayerArm : MonoBehaviour {

	void Start ()
    {
	
	}
	
	void Update ()
    {
        transform.eulerAngles += Vector3.forward * Geometry.differenceOfAnglesNegative(Geometry.angleOfVector3(MousePosition.get - transform.position), transform.eulerAngles.z) * 18f * Game.dt;
	}
}
