using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Herbie
{
	public class Fight : MonoBehaviour {

		public GameObject herbie;
		public GameObject fist;
		public GameObject crosshair;
		private Vector3 herbiePosition;
		private Vector3 fistScale;
		private float time = 0f;
		private bool decided;
		private bool fistTurningBack;
		private bool isRight;
		private float dt;
		private float dtMultiplier = 1f;


		void Start ()
		{
			Game.miniGame = gameObject;
			Game.isThereMiniGame = true;
			herbiePosition = herbie.transform.position;
			fistScale = fist.transform.localScale;
		}

		private float axisX;
		private float axisY;

		private static float smooth(float value, float target)
		{
			if (target != 0f && value != 0f && Mathf.Sign(target) != Mathf.Sign(value))
			{
				value = 0f;
			}
			return Mathf.MoveTowards(value, target, 3f * Time.deltaTime);
		}

		void Update ()
		{
			dt = Game.dt * dtMultiplier;
			time += dt;

			// In the collection: the old Input.GetAxis eased a key press in and out, which is
			// what made Herbie slide to the side rather than jump there. The collection's
			// GetAxis is the raw value, so the easing is done here, with the Input Manager's
			// default settings (sensitivity and gravity 3, snap on).
			axisX = smooth(axisX, TaloketoInputManager.GetAxisRaw("Horizontal"));
			axisY = smooth(axisY, TaloketoInputManager.GetAxisRaw("Vertical"));
			herbie.transform.position = herbiePosition + Vector3.right * axisX * 2f + Vector3.up * axisY * 1.5f;
			crosshair.transform.Rotate(Vector3.forward * 100 * dt);

			if (time < 1f)
			{
				fist.transform.position = new Vector3(fist.transform.position.x, 10, fist.transform.position.z);
				crosshair.transform.position = new Vector3(crosshair.transform.position.x, 10, crosshair.transform.position.z);
				decided = false;
				fistTurningBack = false;
			}
			else if (time < 3f)
			{
				if (!decided)
				{
					decided = true;
					dtMultiplier += 0.2f;
					if (Random.Range(0f, 1f) < 0.5f)
					{
						isRight = true;
						fist.transform.localScale = fistScale;
						crosshair.transform.position = new Vector3(2f, Random.Range(2f, -1f), crosshair.transform.position.z);
						fist.transform.position = new Vector3(8f, crosshair.transform.position.y, fist.transform.position.z);
					}
					else
					{
						isRight = false;
						fist.transform.localScale = new Vector3(-fistScale.x, fistScale.y, fistScale.z);
						crosshair.transform.position = new Vector3(-2f, Random.Range(2f, -1f), crosshair.transform.position.z);
						fist.transform.position = new Vector3(-8f, crosshair.transform.position.y, fist.transform.position.z);
					}
				}
				if (isRight)
				{
					fist.transform.position -= Vector3.right * 4f * dt;
				}
				else
				{
					fist.transform.position += Vector3.right * 4f * dt;
				}
				if (!fistTurningBack && time > 1.6f)
				{
					fistTurningBack = true;
					Game.instance.aFist.pitch = 0.75f + dtMultiplier / 4f;
					Game.instance.aFist.Play ();
				}
			}
			else if (time < 5f)
			{
				if (isRight)
				{
					fist.transform.position += Vector3.right * 4f * dt;
				}
				else
				{
					fist.transform.position -= Vector3.right * 4f * dt;
				}
			}
			else
			{
				time -= 5f;
			}
		}
	}
}
