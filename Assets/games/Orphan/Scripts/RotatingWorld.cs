using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class RotatingWorld : MonoBehaviour {


		void Start ()
		{

		}

		void Update ()
		{
			Vector2 deltaPosition = new Vector2(PlayerScript.instance.transform.position.x - transform.position.x,
				PlayerScript.instance.transform.position.y - transform.position.y);
			float deltaDistance = Mathf.Sqrt (deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);
			if (deltaDistance < 81)
			{
				transform.rotation = Quaternion.identity;
				transform.Rotate(Vector3.forward * Mathf.Atan2(deltaPosition.y, deltaPosition.x) * 180f / Mathf.PI);
			}

			//transform.localScale = Vector3.one * (deltaDistance + 20f) / 100f;
		}
	}
}
