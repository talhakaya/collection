using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {
	
	public static GameObject prefab = Resources.Load ("Fire") as GameObject;
	private float direction;
	private float speed;
	public AudioSource audioSource;
	private bool soundPlayed;
	
	void Start ()
	{
		direction = Random.Range(0f, 1f) * 360;
		GetComponent<DoubleSprite> ().selfColor = new Color (Random.Range (0.7f, 1f), Random.Range (0.7f, 1f), Random.Range (0.7f, 1f));
		transform.Rotate (Vector3.forward, direction);
		speed = Random.Range (0.2f, 3f);
	}
	
	void Update ()
	{
		transform.position += Geometry.createVector3 (direction, speed * Time.deltaTime);
		speed -= Time.deltaTime;
		if (speed <= 0)
		{
			if (!soundPlayed)
			{
				audioSource.pitch = Random.Range(0.7f, 1.3f);
				audioSource.Play ();
				soundPlayed = true;
			}
			speed = 0;
			getDamage(Time.deltaTime / 2);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "Slime")
		{
			
		}
		else if (other.name == "Fire")
		{
			
		}
		else if (other.name == "Explosion")
		{
			
		}
		else if (other.name == "Human")
		{
			
		}
	}
	
	void OnTriggerStay2D(Collider2D other)
	{
		if (other.name == "Slime")
		{

		}
		else if (other.name == "Fire")
		{
			
		}
		else if (other.name == "Explosion")
		{
			
		}
		else if (other.name == "Human")
		{
			
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.name == "Slime")
		{
			
		}
		else if (other.name == "Fire")
		{
			
		}
		else if (other.name == "Explosion")
		{
			
		}
		else if (other.name == "Human")
		{
			
		}
	}
	
	public void getDamage(float damage)
	{
		Vector3 deltaScale = new Vector3(1f, 1f, 1f) * damage;
		transform.localScale -= deltaScale;
		if (transform.localScale.x <= 0f)
		{
			Destroy(gameObject);
		}
	}

	
	public static Fire create(float size)
	{
		Fire fire = (Instantiate (prefab) as GameObject).GetComponent<Fire> ();
		fire.name = "Fire";
		fire.transform.localScale = new Vector3 (size, size, size);
		fire.transform.parent = GameManager.game;
		
		return fire;
	}
}
