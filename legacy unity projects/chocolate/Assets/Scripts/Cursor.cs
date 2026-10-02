using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour {

	public Sprite right;
	public Sprite left;
	public Sprite up;
	private bool onDoor;
	private Door door;
	private SpriteRenderer spriteRenderer;

	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update ()
	{
		transform.position = MousePosition.get ();

		if (Girl.instance != null)
		{
			if (onDoor)
			{
				spriteRenderer.sprite = up;
				if (Input.GetMouseButton(0) && door != null && door.ready)
				{
					Game.done = true;
				}
			}
			else
			{
				if (Girl.instance.position.x < transform.position.x)
				{
					spriteRenderer.sprite = right;
				}
				else
				{
					spriteRenderer.sprite = left;
				}
			}

			if (Girl.instance.position.x < transform.position.x)
			{
				if (Input.GetMouseButton(0))
				{
					Girl.instance.position += Vector3.right * Girl.speed * Time.deltaTime;
					Girl.script.walking = true;
					Girl.script.isRight = true;
				}
				else
				{
					Girl.script.walking = false;
				}
			}
			else
			{
				if (Input.GetMouseButton(0))
				{
					Girl.instance.position -= Vector3.right * Girl.speed * Time.deltaTime;
					Girl.script.walking = true;
					Girl.script.isRight = false;
				}
				else
				{
					Girl.script.walking = false;
				}
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "door")
		{
			onDoor = true;
			door = other.GetComponent<Door>();
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.name == "door")
		{
			onDoor = false;
		}
	}
}
