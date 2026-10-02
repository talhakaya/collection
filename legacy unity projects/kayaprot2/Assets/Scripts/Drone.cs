using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Gadget {
    public static Drone instance;
    private Rigidbody2D body;
    public bool onGround;
    [SerializeField] private Transform[] onGroundChecks;
    [SerializeField] private GameObject[] lights;

    void Awake() {
        body = GetComponent<Rigidbody2D>();
        body.centerOfMass = new Vector2(0f, 0f);
        instance = this;
    }
	
	void FixedUpdate () {
        onGround = false;
        RaycastHit2D hitGround = Physics2D.Raycast(onGroundChecks[0].position, onGroundChecks[1].position - onGroundChecks[0].position);
        if (hitGround.collider != null) {
            Vector3 hitPointLocal = transform.InverseTransformPoint(hitGround.point);
            onGround = (hitPointLocal.x >= onGroundChecks[0].localPosition.x && hitPointLocal.x <= onGroundChecks[1].localPosition.x);
        }
        foreach (GameObject l in lights) {
            l.SetActive(!onGround);
        }
        
        if (!onGround) {
            if (input.x == 0f) {
                body.AddTorque(Geometry.differenceOfAnglesNegative(0f, transform.eulerAngles.z) * Game.dtPhysics * 2f);
            }
            else {
                body.AddTorque(input.x * 0.2f);
            }
        }
        if (input.x != 0f || input.y != 0f) {
            body.AddForce(new Vector2(input.x * 4f, input.y * 6f));
        }

        SetInput(0f, 0f);
    }
}
