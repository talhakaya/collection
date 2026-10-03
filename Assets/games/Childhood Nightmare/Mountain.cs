using UnityEngine;
using System.Collections;

namespace Games.ChildhoodNightmare
{
	public class Mountain : MonoBehaviour {

		public TintScript tint;

		void Start ()
		{
			float size = Random.Range (5f, 8f);
			tint.transform.localScale = Vector3.one * size;
			tint.transform.position = new Vector3 (tint.transform.position.x, size, tint.transform.position.z);
			tint.selfColor = new Color (Random.Range (0.6f, 1f), Random.Range (0.6f, 1f), Random.Range (0.6f, 1f));
		}

		void Update ()
		{

		}
	}
}
