using UnityEngine;
using System.Collections;

namespace Games.ChildhoodNightmare
{
	public class MountainCreator : MonoBehaviour {

		public GameObject mountainPrefab;

		void Start ()
		{
			int length = 8;
			for (int i = 0; i < length; i++)
			{
				GameObject mountain = Instantiate(mountainPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				mountain.transform.Rotate(Vector3.up * (i * 360 / length + Random.Range(-5f, 5f)));
				mountain.name = "Mountain";
				mountain.transform.parent = transform;
			}
		}

		void Update ()
		{

		}
	}
}
