using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class ParticleTriangle : MonoBehaviour
	{
		private float rotZ;
		private Vector3 point;

		void Start ()
		{
			transform.Rotate (new Vector3(0, 0, Random.Range (0f, 360f)));
			rotZ = Random.Range (0.5f, 1.5f);
			point = transform.position;
		}

		void Update ()
		{
			transform.Rotate (new Vector3(0, 0, rotZ));

			if (PlayerScript.instance != null)
			{
				Vector3 deltaPosition = PlayerScript.instance.transform.position - point;
				float distance = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);
				transform.position = point + deltaPosition / distance;
			}
		}
	}
}
