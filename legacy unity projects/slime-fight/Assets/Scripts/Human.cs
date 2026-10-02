using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {
	
	public static GameObject prefab = Resources.Load ("Human") as GameObject;

	public Sprite dead0;
	public Sprite dead1;
	public DoubleSprite ds;
	private float direction;
	private float speed;
	private int turning = 0;
	public bool isDead = false;
	public float timeCounter = 0f;
	public AudioSource audioSource;

	void Start ()
	{
		direction = Random.Range(0f, 1f) * 360;
		speed = Random.Range (0.3f, 0.6f);
		ds.selfColor = new Color (Random.Range (0f, 0.3f), Random.Range (0f, 0.3f), Random.Range (0f, 0.3f));
	}

	void Update ()
	{
		if (!isDead)
		{
			if (Random.Range (0f, 1f) < Time.deltaTime)
			{
				turning = Random.Range(-2, 3);
			}
			if (turning == -1)
			{
				direction += Time.deltaTime * 120;
			}
			else if (turning == 1)
			{
				direction -= Time.deltaTime * 120;
			}
			transform.eulerAngles = Vector3.zero;
			transform.Rotate (Vector3.forward, direction);
			transform.position += Geometry.createVector3 (direction, speed * Time.deltaTime);
		}
		else
		{
//			timeCounter += Time.deltaTime;
//			if (timeCounter >= 3f)
//			{
//				Destroy(gameObject);
//			}
//			else if (timeCounter >= 2f)
//			{
//				ds.selfColor = new Color(ds.selfColor.r, ds.selfColor.g, ds.selfColor.b, ds.selfColor.a - 3f - timeCounter);
//			}
		}
	}

	public void die()
	{
		if (!isDead)
		{
			ds.sprite0.sprite = dead0;
			ds.sprite1.sprite = dead1;
			isDead = true;
			audioSource.pitch = transform.localScale.x;
			audioSource.Play ();
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "Slime")
		{
			die ();
		}
		else if (other.name == "Fire")
		{
			die ();
		}
		else if (other.name == "Explosion")
		{
			die ();
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

	
	public static Human create(float size)
	{
		Human human = (Instantiate (prefab) as GameObject).GetComponent<Human> ();
		human.name = "Human";
		human.transform.localScale = new Vector3 (size, size, size);
		human.transform.parent = GameManager.game;
		
		return human;
	}
}
