using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Chocolate
{
	public class Cursor : MonoBehaviour {

		public Sprite right;
		public Sprite left;
		public Sprite up;
		private bool onDoor;
		private Door door;
		private SpriteRenderer spriteRenderer;

		// How many of these are on screen. Consecutive slides each carry their own cursor, and
		// the outgoing slide is only destroyed at the end of the frame the incoming one appears
		// in - so its OnDisable runs after the new one's OnEnable. Counting is what stops that
		// late OnDisable handing the collection's cursor back while the new arrow is up.
		private static int activeCount;

		// This arrow is the pointer on the slides that have it, so the collection must not
		// draw its own on top. Slides without one (the blurred ones) get the normal cursor
		// back. Leaving the game mid-slide needs nothing here: the collection resets it.
		void OnEnable ()
		{
			if (activeCount++ == 0)
			{
				GlobalInputManager.HideGameCursor();
			}
		}

		void OnDisable ()
		{
			if (--activeCount == 0)
			{
				GlobalInputManager.ClearGameCursor();
			}
		}

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
					if (TaloketoInputManager.GetMouseButton(0) && door != null && door.ready)
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
					if (TaloketoInputManager.GetMouseButton(0))
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
					if (TaloketoInputManager.GetMouseButton(0))
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
}
