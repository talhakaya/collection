using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class StaticObjectZScript : MonoBehaviour {
		public static float zDivider = 10000f;

		public float zOffset;

		void Start ()
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / zDivider + zOffset);
		}

		void Update ()
		{

		}
	}
}
