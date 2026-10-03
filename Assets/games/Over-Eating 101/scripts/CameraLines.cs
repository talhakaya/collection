using UnityEngine;
using System.Collections;

namespace Games.OverEating101
{
	public class CameraLines : MonoBehaviour {
		public GameObject noisePrefab;
		private SpriteRenderer[] cameraLines;
		private int noOfLines = 10;
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
				cameraLines[i].color = new Color(1f, 1f, 1f, Random.Range (0f, 0.2f));
			}
			if (GetComponent<Camera>().orthographicSize < 5f)
			{
				GetComponent<Camera>().orthographicSize += Game.dt;
			}
			if (transform.eulerAngles.z > 180f)
			{
				transform.Rotate(Vector3.forward * 10 * Game.dt);
			}
			else if (transform.eulerAngles.z > 0f)
			{
				transform.Rotate(-Vector3.forward * 10 * Game.dt);
			}
			if (transform.eulerAngles.z < 0.1f || 360 - transform.eulerAngles.z < 0.1f)
			{
				transform.rotation = Quaternion.identity;
			}
			Game.instance.aMusic.pitch = 1f + ((5f - GetComponent<Camera>().orthographicSize) / 5f);
			transform.localScale = Vector3.one * (GetComponent<Camera>().orthographicSize / 5f);
		}
	}
}
