using UnityEngine;
using System.Collections;

namespace Games.Herbie
{
	public class CameraLines : MonoBehaviour {
		public GameObject noisePrefab;
		private SpriteRenderer[] cameraLines;
		private int noOfLines = 10;
		public Herbie herbie;
		public Transform world;
		private static float maxHerbieX = 6.3f;
		private static float minHerbieX = -6.3f;
		private static float maxHerbieY = 3f;
		private static float minHerbieY = -4.3f;
		private static float maxZoomOutY = 2.5f;
		private static float maxZoomOutX = maxZoomOutY * 16 / 9;
		private static float maxCameraX = maxHerbieX - maxZoomOutX;
	//	private static float minCameraX = minHerbieX + maxZoomOutX;
		private static float maxCameraY = maxHerbieY - maxZoomOutY;
	//	private static float minCameraY = minHerbieY + maxZoomOutY;
		private static float maxDistance = Mathf.Sqrt(maxHerbieX * maxHerbieX + maxHerbieY * maxHerbieY);
		private static float zoomChange = 0.5f;
		public float herbieYOffset = 1f;
		private static float MaxAlpha = 0.15f;
		void Start ()
		{
			cameraLines = new SpriteRenderer[noOfLines];
			for (int i = 0; i < noOfLines; i++)
			{
				GameObject noise = Instantiate(noisePrefab, Vector3.up * Random.Range(-4f, 4f) + Vector3.forward * 2f, Quaternion.identity) as GameObject;
				cameraLines[i] = noise.GetComponent<SpriteRenderer>();
				noise.name = "CameraLines";
				noise.transform.parent = transform;
			}

		}

		void Update ()
		{
			for (int i = 0; i < noOfLines; i++)
			{
				cameraLines[i].transform.localPosition = Vector3.up * Random.Range(-4f, 4f) + Vector3.forward * 2f;
				cameraLines[i].color = new Color(1f, 1f, 1f, Random.Range (0f, MaxAlpha));
			}

			if (Game.isThereMiniGame)
			{
				GetComponent<Camera>().orthographicSize = 2.5f;
				transform.position = Vector3.forward * (-10f);
				transform.localScale = Vector3.one * (GetComponent<Camera>().orthographicSize / 2.5f);
			}
			else
			{
				if (herbie.transform.position.x < minHerbieX)
				{
					world.transform.position += Vector3.right * 12.8f; 
					herbie.transform.position = new Vector3(maxHerbieX, herbie.transform.position.y, herbie.transform.position.z);
				}
				else if (herbie.transform.position.x > maxHerbieX)
				{
					world.transform.position -= Vector3.right * 12.8f; 
					herbie.transform.position = new Vector3(minHerbieX, herbie.transform.position.y, herbie.transform.position.z);
				}

				if (herbie.transform.position.y < minHerbieY)
				{
					world.transform.position += Vector3.up * 7.2f; 
					herbie.transform.position = new Vector3(herbie.transform.position.x, maxHerbieY, herbie.transform.position.z);
				}
				else if (herbie.transform.position.y > maxHerbieY)
				{
					world.transform.position -= Vector3.up * 7.2f; 
					herbie.transform.position = new Vector3(herbie.transform.position.x, minHerbieY, herbie.transform.position.z);
				}

				GetComponent<Camera>().orthographicSize = maxZoomOutY - zoomChange + zoomChange * Geometry.lengthOfVector2(new Vector2(herbie.transform.position.x, herbie.transform.position.y)) / maxDistance;
				transform.position = new Vector3(maxCameraX * herbie.transform.position.x / maxHerbieX, maxCameraY * herbie.transform.position.y / maxHerbieY, transform.position.z);
				transform.localScale = Vector3.one * (GetComponent<Camera>().orthographicSize / 2.5f);
			}
		}
	}
}
