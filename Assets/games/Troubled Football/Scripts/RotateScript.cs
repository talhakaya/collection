using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.TroubledFootball
{
	public class RotateScript : MonoBehaviour {

		public bool rotating;
		public bool isClockwise;
		public float speed;

		void Start ()
		{

		}

		void Update ()
		{
			if (rotating)
			{
				float cons = 20f;
				if (!isClockwise)
				{
					cons = -cons;
				}
				if (TaloketoInputManager.GetMouseButton(0))
				{
					cons *= 2;
				}
				transform.Rotate(0f, 0f, speed * cons * Time.deltaTime);
			}
		}
	}
}
