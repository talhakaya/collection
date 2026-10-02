using UnityEngine;
using System.Collections;

public class ExitScreen : MonoBehaviour {

    public bool horizontal = true;
    public bool vertical = true;
	
	void Update ()
    {
        if (Physics2D.gravity == Vector2.zero)
        {
            if (horizontal)
            {
                if (transform.position.x < -6.4f)
                {
                    transform.position = new Vector3(6.4f, transform.position.y, transform.position.z);
                }
                else if (transform.position.x > 6.4f)
                {
                    transform.position = new Vector3(-6.4f, transform.position.y, transform.position.z);
                }
            }
            if (vertical)
            {
                if (transform.position.y < -3.6f)
                {
                    transform.position = new Vector3(transform.position.x, 3.6f, transform.position.z);
                }
                else if (transform.position.y > 3.6f)
                {
                    transform.position = new Vector3(transform.position.x, -3.6f, transform.position.z);
                }
            }
        }
	}
}
