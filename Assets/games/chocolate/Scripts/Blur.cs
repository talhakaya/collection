using UnityEngine;
using System.Collections;

namespace Games.Chocolate
{
	public class Blur : MonoBehaviour {

		private bool done;
		private float timeCounter;

		void Start ()
		{
			SpriteEffect.make (Effect.Blur, gameObject);
			Game.reference.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range (-4f, 4f));
			while (Geometry.lengthOfVector2(new Vector2(Game.reference.transform.position.x - MousePosition.x (), Game.reference.transform.position.y - MousePosition.y ())) < 3f)
			{
				Game.reference.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range (-4f, 4f));
			}
		}

		void Update ()
		{
			if (!done)
			{
				if (Geometry.lengthOfVector2(new Vector2(Game.reference.transform.position.x - MousePosition.x (), Game.reference.transform.position.y - MousePosition.y ())) < 1f)
				{
					SpriteEffect.destroy(Effect.Blur, gameObject);
					done = true;
				}
			}
			else
			{
				timeCounter += Time.deltaTime;
				if (timeCounter > 2f)
				{
					Game.done = true;
				}
			}
		}
	}
}
