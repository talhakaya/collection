using UnityEngine;
using System.Collections;

public class FaceLookAround : MonoBehaviour {

    public float maxSpeed;
    public float acceleration;
    private float speed;
    private Vector3 localPos;
	void Start ()
    {
        localPos = transform.localPosition;
	}
	
	void Update () {
        Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetMouseButton(0))
        {
            dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        }
        if (dir.x != 0f || dir.y != 0f)
        {
            speed = acceleration;
        }
        else
        {
            speed = 0f;
            //speed -= Game.dt * acceleration;
            //if (speed < 0f)
            //{
            //}
            transform.localPosition += (localPos - transform.localPosition) * Game.dt;
        }
        transform.localPosition += Geometry.normalizeVector3(dir, speed);
        if (Geometry.lengthOfVector3(localPos - transform.localPosition) > maxSpeed)
        {
            transform.localPosition = localPos + Geometry.normalizeVector3(transform.localPosition - localPos, maxSpeed);
        }
	}
}
