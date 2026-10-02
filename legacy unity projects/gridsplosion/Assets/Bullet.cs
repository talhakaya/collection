using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    Rigidbody2D body;

	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        body.velocity = Geometry.createVector2(transform.eulerAngles.z + Random.Range(-5f, 5f), 35f + Random.value * 15f);
	}
	
	void Update ()
    {
	    if (Geometry.lengthOfVector3(Camera.main.transform.position - transform.position) > 18f)
        {
            Destroy(gameObject);
        }
	}
}
