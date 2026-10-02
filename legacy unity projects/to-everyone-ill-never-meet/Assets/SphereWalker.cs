using UnityEngine;
using System.Collections;

public class SphereWalker : MonoBehaviour {

    public const float SphereRadius = 50.5f;

	void Start ()
    {
	
	}
	
	void Update ()
    {
        transform.position = Geometry.normalizeVector3(transform.position, SphereRadius);

        Physics.gravity = Geometry.normalizeVector3(-transform.position, 10f);
	}
}
