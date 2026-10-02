using UnityEngine;
using System.Collections;

public class SlimeDead : MonoBehaviour {
	
	public static GameObject prefab = Resources.Load ("SlimeDead") as GameObject;
	private float direction;
	private float speed;
	public AudioSource audioSource;
	
	void Start ()
	{
		direction = Random.Range(0f, 1f) * 360;
		GetComponent<DoubleSprite> ().selfColor = new Color (Random.Range (0.7f, 1f), Random.Range (0.7f, 1f), Random.Range (0.7f, 1f));
		transform.Rotate (Vector3.forward, direction);
		speed = Random.Range (1f, 2f);
		audioSource.pitch = Random.Range(0.7f, 1.3f);
		audioSource.Play ();
	}
	
	void Update ()
	{
		transform.position += Geometry.createVector3 (direction, speed * Time.deltaTime);
		speed -= Time.deltaTime;
		if (speed <= 0)
		{
			speed = 0;
		}
	}
	
	
	public static SlimeDead create(float size)
	{
		SlimeDead slimeDead = (Instantiate (prefab) as GameObject).GetComponent<SlimeDead> ();
		slimeDead.name = "SlimeDead";
		slimeDead.transform.localScale = new Vector3 (size, size, size);
		slimeDead.transform.parent = GameManager.game;
		
		return slimeDead;
	}
}
