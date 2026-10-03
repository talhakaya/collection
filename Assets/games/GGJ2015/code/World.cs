using UnityEngine;
using System.Collections;

namespace Games.GGJ2015
{
	public class World : MonoBehaviour {

		void Start ()
		{

		}

		void Update ()
		{
			transform.localPosition = -transform.parent.localPosition;
		}
	}
}
