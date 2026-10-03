using UnityEngine;
using System.Collections;

namespace Games.Herbie
{
	public class Pacman : MonoBehaviour {

		int[] level;
		public GameObject wallPrefab;
		public GameObject pacman;
		public GameObject herbie;
		public GameObject coke;
		private float pacmanSpeed = 1000;
		private float pacmanTimer = 0f;
		private Vector3 pacmanLastPosition;
		private bool pacmanRightWay = true;
		public static bool cokeTaken = false;

		void Start ()
		{
			Game.miniGame = gameObject;
			Game.isThereMiniGame = true;
			int width = 32;
			int height = 19;
			float xLength = 0.25f;
			float yLength = 0.25f;
			float xBegin = -xLength * width / 2 + 0.15f;
			float yBegin = yLength * height / 2 + 0.12f;
			level = new int[32 * 19]	   {0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
											0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
											0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,
											0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,
											0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,
											0,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,0,0,0,1,1,0,0,0,0,0,1,0,
											0,1,0,0,0,0,1,1,0,0,0,1,1,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0,1,0,
											0,1,0,0,0,0,1,1,0,0,0,1,1,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,1,0,
											0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,1,0,
											0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,
											0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,
											0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,1,1,0,0,0,0,1,1,0,0,0,1,0,
											0,0,0,0,0,0,0,0,1,1,1,1,0,1,1,1,0,0,0,0,1,0,0,0,1,1,0,0,0,0,1,0,
											0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,1,1,1,1,1,0,0,0,0,0,1,0,
											0,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,0,
											0,1,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,
											0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,
											0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,
											0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0};
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (level[j * width + i] == 1)
					{
						GameObject wall = Instantiate(wallPrefab, new Vector3(xBegin + i * xLength, yBegin - j * yLength, -1f), Quaternion.identity) as GameObject;
						wall.name = "wall";
						wall.transform.parent = transform;
					}
				}
			}

			transform.localScale = Vector3.one * 25f / 21f;
			pacmanLastPosition = pacman.transform.position;

			if (Game.cokeTaken)
			{
				Destroy (coke);
			}
		}

		void Update ()
		{
			pacmanTimer += Game.dt;
			if (pacmanTimer >= 0.1f)
			{
				pacmanTimer -= 0.1f;
				float pacmanDistance = Geometry.lengthOfVector3 (pacmanLastPosition - pacman.transform.position);
				if (pacmanDistance > 0.1f)
				{
					pacmanLastPosition = pacman.transform.position;
					pacmanRightWay = true;
				}
				else
				{
					pacmanRightWay = !pacmanRightWay;
				}
			}
			if (Mathf.Abs(pacman.transform.position.x - herbie.transform.position.x) > Mathf.Abs(pacman.transform.position.y - herbie.transform.position.y))
			{
				if (pacmanRightWay)
				{
					if (pacman.transform.position.x < herbie.transform.position.x)
					{
						Vector3 force = Geometry.normalizeVector2(new Vector2(1f, 0f), pacmanSpeed);
						pacman.GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
					}
					else
					{
						Vector3 force = Geometry.normalizeVector2(new Vector2(-1f, 0f), pacmanSpeed);
						pacman.GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
					}
				}
				else
				{
					if (pacman.transform.position.y > herbie.transform.position.y)
					{
						Vector3 force = Geometry.normalizeVector2(new Vector2(0f, 1f), pacmanSpeed);
						pacman.GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
					}
					else
					{
						Vector3 force = Geometry.normalizeVector2(new Vector2(0f, -1f), pacmanSpeed);
						pacman.GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
					}
				}
			}
			else
			{
				if (pacmanRightWay)
				{
					if (pacman.transform.position.y < herbie.transform.position.y)
					{
						Vector3 force = Geometry.normalizeVector2(new Vector2(0f, 1f), pacmanSpeed);
						pacman.GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
					}
					else
					{
						Vector3 force = Geometry.normalizeVector2(new Vector2(0f, -1f), pacmanSpeed);
						pacman.GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
					}
				}
				else
				{
					if (pacman.transform.position.x > herbie.transform.position.x)
					{
						Vector3 force = Geometry.normalizeVector2(new Vector2(1f, 0f), pacmanSpeed);
						pacman.GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
					}
					else
					{
						Vector3 force = Geometry.normalizeVector2(new Vector2(-1f, 0f), pacmanSpeed);
						pacman.GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
					}
				}
			}

			pacman.transform.rotation = Quaternion.identity;
			pacman.transform.Rotate (Vector3.forward * Mathf.Atan2 (pacman.GetComponent<Rigidbody2D>().linearVelocity.y, pacman.GetComponent<Rigidbody2D>().linearVelocity.x));

			if (herbie.transform.position.x < -4.5f)
			{
				if (cokeTaken)
				{
					Game.cokeTaken = true;
				}
				Game.endMiniGame();
			}
		}
	}
}
