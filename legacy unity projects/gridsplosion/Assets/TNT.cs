using UnityEngine;
using System.Collections;

public class TNT : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Bullet>() != null)
        {
            Destroy(other.gameObject);
            CameraScript.zoom(0.5f);
            Game.explosion(new ExplosionData(transform.position, Color.red));
        }
    }
}
