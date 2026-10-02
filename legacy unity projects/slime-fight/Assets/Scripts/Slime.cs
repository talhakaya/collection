using UnityEngine;
using System.Collections;

public class Slime : MonoBehaviour {

	public static GameObject prefab = Resources.Load ("Slime") as GameObject;
	private float direction;
	private float speed;
	private float damageCounter = 0f;
	private int turning = 0;
	public AudioSource audioSource;

	void Start ()
	{
		GetComponent<DoubleSprite> ().selfColor = new Color (Random.Range (0.7f, 1f), Random.Range (0.7f, 1f), Random.Range (0.7f, 1f));
		direction = Random.Range(0f, 1f) * 360;
		speed = Random.Range (0.2f, 0.5f);
		
	}

	void Update ()
	{
		transform.position += Geometry.createVector3 (direction, speed * Time.deltaTime);
		speed = Random.Range (0.2f, 0.5f);
		if (Random.Range (0f, 1f) < Time.deltaTime * 5)
		{
			turning = Random.Range(-2, 2);
		}
		if (turning == -1)
		{
			direction += Time.deltaTime * 240;
		}
		else if (turning == 1)
		{
			direction -= Time.deltaTime * 240;
		}

		while (damageCounter > 0.15f)
		{
			damageCounter -= 0.15f;
			SlimeDead.create(Random.Range (0.5f, 2.5f)).transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
		}

		GameManager.totalSlimeSize += transform.localScale.x;
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
			if (transform.localScale.x > other.transform.localScale.x)
			{
				getDamage(-Time.deltaTime);
				other.GetComponent<Slime>().getDamage(Time.deltaTime);
			}
		}
		else if (other.name == "Fire")
		{
			getDamage(Time.deltaTime / 3);
		}
		else if (other.name == "Explosion")
		{
			getDamage(Time.deltaTime);
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
		damageCounter += damage;
		Vector3 deltaScale = new Vector3(1f, 1f, 1f) * damage;
		transform.localScale -= deltaScale;
		if (transform.localScale.x <= 0f)
		{
			Destroy(gameObject);
		}
	}
	
	public static Slime create(float size)
	{
		Slime slime = (Instantiate (prefab) as GameObject).GetComponent<Slime> ();
		slime.name = "Slime";
		slime.transform.localScale = new Vector3 (size, size, size);
		slime.transform.parent = GameManager.game;

		return slime;
	}
}
