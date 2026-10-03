using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Orphan
{
	public class DraggableObject : MonoBehaviour {

		public static bool MouseHoldingAnObject;

		public bool hold;

		private Vector2 mouseOffset;

		void Start ()
		{

		}

		void Update ()
		{
			if (!hold)
			{
				if (!MouseHoldingAnObject && TaloketoInputManager.GetButtonDown("Fire"))
				{
					Ray ray = Camera.main.ScreenPointToRay(TaloketoInputManager.mousePosition);
					RaycastHit hit = new RaycastHit();

					if(Physics.Raycast(ray, out hit))
					{
						if (transform == hit.transform)
						{
							MouseHoldingAnObject = true;
							hold = true;
							mouseOffset = ray.origin - transform.position;
						}
					}
				}
			}
			if (hold)
			{
				if (TaloketoInputManager.GetButtonUp("Fire"))
				{
					MouseHoldingAnObject = false;
					hold = false;
				}
				else
				{
					Ray ray = Camera.main.ScreenPointToRay(TaloketoInputManager.mousePosition);
					transform.position = new Vector3(ray.origin.x - mouseOffset.x, ray.origin.y - mouseOffset.y ,transform.position.z);
				}
			}
		}
	}
}
