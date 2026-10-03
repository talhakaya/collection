using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class ArcadeEnemyScript : MonoBehaviour
	{
		private float z;
		private float tweenTime;
		private float tweenPeriod;
		private bool touchedPlayer;
		private float stopTime;
		private float timeCount;
		private tk2dSprite sprite;
		private float realTweenTime;

		public float radius;
		public Vector3 point;
		public float zOffset;


		void Start ()
		{
			z = Random.Range(-1f, 1f);
			tweenTime = 0.6f * Random.Range (1f, 2f) * 3;
			realTweenTime = tweenTime;
			tweenPeriod = tweenTime * Random.Range (0f, 1f);
			point = transform.position;
			sprite = gameObject.GetComponent<tk2dSprite>();
			stopTime = 3f;
			timeCount = 0f;
		}

		void Update ()
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, PlayerScript.instance.transform.position.z);

			transform.Rotate(new Vector3(0, 0, z));

			if (!ArcadeAvoidEnemies.instance.makeEnemiesStop)
			{
				tweenPeriod += Time.deltaTime;

				if (tweenPeriod > realTweenTime)
				{
					tweenPeriod = 0;
					newTween();
				}

			}
			else
			{
				PathTween.Stop(gameObject); // In the collection: was iTween.Stop

				if (touchedPlayer)
				{
					sprite.scale = new Vector3(sprite.scale.x * (1 + Time.deltaTime * 1.5f), sprite.scale.y * (1 + Time.deltaTime * 1.5f), 1f);
					timeCount += Time.deltaTime;
					if (timeCount >= stopTime)
					{
						ArcadeGameManager.instance.arcadeGameManagerState = ArcadeGameManager.ArcadeGameManagerState.FadingOut;
						sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - Time.deltaTime / 2f);
					}
				}
				else
				{
					sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - Time.deltaTime);
				}

				if (sprite.color.a <= 0f)
				{
					Destroy(gameObject);
				}
			}

			if (ArcadeGameManager.instance == null)
			{
				Destroy(gameObject);
			}
		}

		void newTween()
		{
			z = Random.Range(-1f, 1f);
			Vector3[] tweenCoord = new Vector3[6];
			point = PlayerScript.instance.transform.position;
			tweenCoord[0] = (point * 1 + transform.position * 4) / 5;
			tweenCoord[1] = (point * 2 + transform.position * 3) / 5;
			tweenCoord[2] = (point * 3 + transform.position * 2) / 5;
			tweenCoord[3] = (point * 4 + transform.position * 1) / 5;
			tweenCoord[4] = point;
			tweenCoord[5] = newTweenPoint();
			realTweenTime = tweenTime * Vector3.Distance (point, transform.position) / 20f;
			PathTween.MoveTo(gameObject, realTweenTime, tweenCoord); // In the collection: was iTween.MoveTo with easeInOutCubic
		}

		Vector3 newTweenPoint()
		{
			float angle = Random.Range(0.0f, Mathf.PI * 2);
			Vector3 pointt = new Vector3(Mathf.Cos (angle), Mathf.Sin (angle), 0f);
			return point + radius * pointt;
		}

		void OnTriggerEnter (Collider collider)
		{
			if (collider.gameObject.name == "Player")
			{
				touchedPlayer = true;
				ArcadeAvoidEnemies.touchedEnemy();
			}
		}

		void OnTriggerStay (Collider collider)
		{
			if (collider.gameObject.name == "Player" && !ArcadeGameManager.instance.isPaused)
			{
				touchedPlayer = true;
				ArcadeAvoidEnemies.touchedEnemy();
			}
		}
	}
}
