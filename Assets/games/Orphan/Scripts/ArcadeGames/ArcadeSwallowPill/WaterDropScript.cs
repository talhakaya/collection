using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class WaterDropScript : MonoBehaviour {

		private float counter;
		private const float timeToDie = 5f;

		public bool outOfGlass;

		void Start ()
		{

		}

		void Update ()
		{
			if (outOfGlass)
			{
				counter += Time.deltaTime;
				if (counter > timeToDie)
				{
					Destroy (gameObject);
				}
			}
		}
	}
}
