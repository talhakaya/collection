using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private float timer;
    public float speed;
    Rigidbody2D body; 

	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
        body.velocity = speed * new Vector2(Game.inputHorizontal, Game.inputVertical);
        if (Geometry.lengthOfVector2(body.velocity) > speed * 0.2f)
        {
            transform.eulerAngles += Vector3.forward * Geometry.differenceOfAnglesNegative(Geometry.angleOfVector2(body.velocity), transform.eulerAngles.z) * 2f * Game.dt;
        }

        timer += Game.dt;
        if (timer >= 0.4f)
        {
            timer -= 0.4f;
            Game.explosion(new ExplosionData(transform.position, Color.white, 3f, 0.5f, 2f));
        }
	}
}
