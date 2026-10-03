using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.OverEating101
{
	public class Arm : MonoBehaviour {
		public bool goingUp;
	//	private float maxAngle = 45;
	//	private float minAngle = -45;
		private float rotationSpeed = 100;

		void Start () {

		}

		// Update is called once per frame
		void Update () {
			if (TaloketoInputManager.GetAxisRaw("Vertical") != 0)
			{
				if (goingUp)
				{
					transform.Rotate(-TaloketoInputManager.GetAxisRaw("Vertical") * Vector3.forward * Game.dt * rotationSpeed);
	//				if (transform.eulerAngles.z >= maxAngle)
	//				{
	//					goingUp = false;
	//				}
				}
				else
				{
					transform.Rotate(TaloketoInputManager.GetAxisRaw("Vertical") * Vector3.forward * Game.dt * rotationSpeed);
	//				if (transform.eulerAngles.z <= minAngle)
	//				{
	//					goingUp = true;
	//				}
				}
			}
		}
	}
}
