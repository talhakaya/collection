using UnityEngine;
using System.Collections;

namespace Games.IncrediblePenis
{
	public class Belir : MonoBehaviour {
		private float time;
		private Vector3 normalScale;
		// Use this for initialization
		void Start () {
			normalScale = transform.localScale;

		}

		// Update is called once per frame
		void Update () {
			time += Time.deltaTime;
			if (time > 51f && time < 61f)
			{
				transform.localScale = normalScale;
			}
			else
			{
				transform.localScale = Vector3.zero;
			}
		}
	}
}
