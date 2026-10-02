using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	
	public static GameObject prefab = Resources.Load ("Explosion") as GameObject;
	public AudioSource audioSource;
	
	void Start ()
	{
		transform.Rotate (Vector3.forward, Random.Range (0f, 360f));
		GetComponent<DoubleSprite> ().selfColor = new Color (Random.Range (0.7f, 1f), Random.Range (0.7f, 1f), Random.Range (0.7f, 1f));
		for (int i = 0; i < transform.localScale.x * 5; i++)
		{
			Fire.create(Random.Range (0.5f, 1.5f)).transform.position = transform.position;
		}
		audioSource.pitch = 3.5f - transform.localScale.x;
		audioSource.volume = transform.localScale.x;
		audioSource.Play ();
	}
	
	void Update ()
	{
		getDamage (Time.deltaTime);
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

	
	public static Explosion create(float size)
	{
		Explosion explosion = (Instantiate (prefab) as GameObject).GetComponent<Explosion> ();
		explosion.name = "Explosion";
		explosion.transform.localScale = new Vector3 (size, size, size);
		explosion.transform.parent = GameManager.game;
		
		return explosion;
	}
}
