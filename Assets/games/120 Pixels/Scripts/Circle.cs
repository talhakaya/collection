using UnityEngine;
using System.Collections;

namespace Games.Pixels120
{
	public class Circle : MonoBehaviour {

		public Sprite red;
		public Sprite white;
		public Sprite talha;
		private bool createdAnotherCircle;
		private SpriteRenderer spriteRenderer;
		private TintScript tint;
		private static int count = 0;

		// In the collection: the process outlives the game, so Game resets this at start.
		public static void resetCount() { count = 0; }

		void Start ()
		{
			spriteRenderer = GetComponent<SpriteRenderer> ();
			tint = GetComponent<TintScript> ();
			transform.localScale = Vector3.forward;
		}

		void Update ()
		{
			transform.localScale += 0.5f * Game.dt * (Vector3.right + Vector3.up) * (spriteRenderer.sprite == talha ? 20f : 1f);
			transform.position += Vector3.forward * Game.dt * (spriteRenderer.sprite == talha ? -0.001f : 0.001f);

			if (spriteRenderer.sprite == talha)
			{
				transform.rotation = Quaternion.identity;
				transform.Rotate(Vector3.forward * Random.Range(-10f, 10f) * 10 / (transform.localScale.x != 0 ? transform.localScale.x : 1f));
			}

			if (spriteRenderer.sprite != talha && !createdAnotherCircle && transform.localScale.x >= 0.3f)
			{
				count++;

				createdAnotherCircle = true;
				if (count < 20)
				{
					Circle circle = (Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<Circle>();
					circle.name = "Circle";
					if (spriteRenderer.sprite == red)
					{
						circle.GetComponent<SpriteRenderer> ().sprite = white;
					}
					else
					{
						circle.GetComponent<SpriteRenderer> ().sprite = red;
					}
				}

				if (count == 10)
				{
					Circle player = (Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<Circle>();
					player.name = "Circle";
					player.GetComponent<SpriteRenderer> ().sprite = talha;
				}
			}
			else if (transform.localScale.x >= (spriteRenderer.sprite == talha ? 25 : 2))
			{
				if (spriteRenderer.sprite != talha)
				{
					tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, tint.selfColor.a - Game.dt);
					if (tint.selfColor.a <= 0)
					{
						Destroy(gameObject);
					}
				}
				else
				{
					Instantiate(Resources.Load("120Pixels/Talha"), transform.position, Quaternion.identity);

					Destroy(gameObject);
				}
			}
		}
	}
}
