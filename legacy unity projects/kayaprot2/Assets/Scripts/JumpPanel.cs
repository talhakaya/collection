using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPanel : Gadget {
    public const float forcePeriod = 0.5f;
    public const float force = 200f;
    public Transform direction;
    public JumpPanelTrigger trigger;

    void Start () {
		
	}
	
	void Update () {
        if (input.x != 0f) {
            direction.localEulerAngles += new Vector3(0f, 0f, -input.x * 45f * Game.dt);
            float dir = Geometry.differenceOfAnglesNegative(direction.localEulerAngles.z, 0f);
            if (dir > 75f) {
                direction.localEulerAngles = new Vector3(0f, 0f, 75f);
            }
            else if (dir < -75f) {
                direction.localEulerAngles = new Vector3(0f, 0f, -75f);
            }
        }
	}

    public void Trigger(Collider2D other) {
        if (other.GetComponent<Bug>() != null) {
            other.GetComponent<Person>().SetForce(Geometry.createVector2(direction.eulerAngles.z, force * forcePeriod * 5f));
        }
        else if (other.GetComponent<Person>() != null) {
            other.GetComponent<Person>().SetForce(Geometry.createVector2(direction.eulerAngles.z, force * forcePeriod));
        }
        else if (other.GetComponent<Rigidbody2D>() != null) {
            other.GetComponent<Rigidbody2D>().AddForce(Geometry.createVector2(direction.eulerAngles.z, force * 3f));
        }
    }
}
