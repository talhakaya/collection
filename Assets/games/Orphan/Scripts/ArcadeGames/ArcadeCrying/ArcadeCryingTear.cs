using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class ArcadeCryingTear : MonoBehaviour {

		private tk2dSprite sprite;
		private Vector2 direction;
		private float speed;

		void Start ()
		{
			sprite = gameObject.GetComponent<tk2dSprite>();
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
			direction = new Vector2(Random.Range(-1f, 1f), -5);
			speed = Random.Range (0.8f, 1f);
		}

		void Update ()
		{
			if (sprite.color.a < 1)
			{
				sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a + Time.deltaTime);
			}
			transform.position = transform.position + new Vector3(direction.x, direction.y, 0) * speed * Time.deltaTime;
			if (transform.position.y - CameraScript.instance.transform.position.y < -27)
			{
				if (ArcadeGameManager.instance.arcadeGameManagerState == ArcadeGameManager.ArcadeGameManagerState.Playing)
				{
					ArcadeGameManager.instance.arcadeGameManagerState = ArcadeGameManager.ArcadeGameManagerState.Tie;
				}
				Destroy(gameObject);
			}
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.name == "Tissue")
			{
				other.gameObject.GetComponent<ArcadeCryingTissue>().tearCount++;
				Destroy(gameObject);
			}
		}
	}
}
