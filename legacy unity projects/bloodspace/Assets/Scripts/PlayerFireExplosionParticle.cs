using UnityEngine;
using System.Collections;

public class PlayerFireExplosionParticle : MonoBehaviour {

	public float direction;
	public bool isDead;
	private TintScript tint;
	
	void Start ()
	{
		gameObject.name = "Player Fire";
		transform.parent = Game.instance.transform;
		tint = GetComponent<TintScript> ();
		tint.selfColor = new Color (1f, 0.5f, 0.5f);
		SpriteEffect.make (Effect.Blur, gameObject, false, true, PlayerScript.instance);
		SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, PlayerScript.instance);
		audio.pitch = Random.Range (0.8f, 1.2f);
		audio.Play ();
	}
	
	void Update ()
	{
		if (direction != -1000)
		{
			rigidbody2D.AddForce (Geometry.createVector2(direction + 90, 300));
			transform.Rotate (Vector3.forward * direction);
			direction = -1000;
		}
		if (Mathf.Abs (transform.position.y) > Game.Y_MAX || Mathf.Abs (transform.position.x) > Game.X_MAX + 2)
		{
			Destroy(gameObject);
		}
		else
		{
			if (isDead)
			{
				if (tint.selfColor.a <= 0)
				{
					Destroy(gameObject);
				}
				else
				{
					tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, tint.selfColor.a - Time.deltaTime * 0.5f);
				}
			}
		}
	}
	
	void OnCollisionEnter2D(Collision2D coll)
	{
		
	}
	
	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.name == "Enemy")
		{
			coll.gameObject.GetComponent<EnemyScript>().hp -= 50f;
			Destroy(gameObject);
		}
	}
}
