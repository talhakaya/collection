using UnityEngine;
using System.Collections;

public class EnemySide : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    if (Random.value < 0.2f)
        {
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
	    if (transform.parent == null)
        {
            transform.position += Vector3.down * 1f * Game.dt;

            if (transform.position.y < -10f)
            {
                Destroy(gameObject);
            }
        }
	}
}
