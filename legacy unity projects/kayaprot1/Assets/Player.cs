using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private float angle;
    private Rigidbody2D body;
    private LineRenderer lineRenderer;

    void Start () {
        angle = 90f;
        body = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

    }
	
	void Update () {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        body.velocity = Geometry.normalizeVector2(input, 5f * Game.timeSpeed);
        
        angle = Geometry.angleOfVector3(MousePosition.get - transform.position);
        body.angularVelocity = 5f * (angle - transform.eulerAngles.z);

        Vector3 dir = Geometry.createVector3(angle, 1f);
        Vector3 aimPoint0 = transform.position;
        aimPoint0 += dir * 0.26f;
        lineRenderer.SetPosition(0, aimPoint0);
        RaycastHit2D hit = Physics2D.Raycast(aimPoint0, dir);
        if (hit.collider != null) {
            lineRenderer.SetPosition(1, hit.point);
        }
        else {
            lineRenderer.SetPosition(1, aimPoint0 + dir * 1000f);
        }
    }
}
