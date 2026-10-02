using UnityEngine;
using System.Collections;

public class bgTile : MonoBehaviour {

    public float speed;
	
	void Update ()
    {
        transform.position += Vector3.down * speed * Game.dt;

        if (transform.position.y < -8f)
        {
            Destroy(gameObject);
        }
	}
}
