using UnityEngine;
using System.Collections;

public class Okra : MonoBehaviour {

	void Start ()
    {
        GetComponent<TalhaAnimation>().period = Random.Range(0.2f, 0.4f);
	}
	
	void Update ()
    {
        if (Physics2D.gravity == Vector2.zero)
        {
            transform.rotation = Quaternion.identity;
            float angle = (Mathf.Atan2(Penguin.instance.transform.position.y - transform.position.y, Penguin.instance.transform.position.x - transform.position.x) * 180 / Mathf.PI);
            transform.Rotate(Vector3.forward * angle);
            rigidbody2D.AddForce(Geometry.createVector2(angle, 1000f * Game.dt));
        }
	}
}
