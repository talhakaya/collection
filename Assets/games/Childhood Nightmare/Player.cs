using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.ChildhoodNightmare
{
	public class Player : MonoBehaviour {

		public static Player script;
		public static GameObject instance;
		public static Transform trans;

		public float speed;
		public float jumpSpeed;
		public float currentJumpSpeed;
		public float jumpPeriod;
		private CharacterController controller;
		public GameObject audioHolder;

		void Awake ()
		{
			script = this;
			instance = gameObject;
			trans = transform;
		}

		void Start ()
		{
			controller = GetComponent<CharacterController> ();
		}

		void Update ()
		{
			float js = -jumpSpeed;
			currentJumpSpeed -= Game.dt * jumpSpeed / jumpPeriod;
			if (TaloketoInputManager.GetButton ("Jump"))
			{
				js = currentJumpSpeed;

				if (currentJumpSpeed <= -jumpSpeed)
				{
					currentJumpSpeed = jumpSpeed;
				}
			}

			controller.Move(Game.dt * (speed * transform.TransformDirection(TaloketoInputManager.GetAxis("Horizontal"), 0f, TaloketoInputManager.GetAxis("Vertical")) + Vector3.up * js));

			audioHolder.transform.Rotate (Vector3.up * 360 * Game.dt);
		}

		void OnControllerColliderHit(ControllerColliderHit hit) {
			if (hit.collider.tag == "Ground")
			{
				currentJumpSpeed = jumpSpeed;
			}
		}
	}
}
