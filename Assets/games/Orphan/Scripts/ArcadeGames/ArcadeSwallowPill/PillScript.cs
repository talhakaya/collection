using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class PillScript : MonoBehaviour {

		private DraggableObject draggableScript;

		void Start ()
		{
			draggableScript = GetComponent<DraggableObject>();
		}

		void Update ()
		{
			if (ArcadeGameManager.instance.arcadeGameManagerState == ArcadeGameManager.ArcadeGameManagerState.Playing)
			{
				if (draggableScript.hold && transform.position.x - CameraScript.instance.transform.position.x > 35)
				{
					transform.position = new Vector3(CameraScript.instance.transform.position.x + 35, transform.position.y, transform.position.z);
				}

				if ((transform.position.y - CameraScript.instance.transform.position.y < -27) 
					&& (transform.position.x - CameraScript.instance.transform.position.x <= 45))
				{
					ArcadeGameManager.instance.arcadeGameManagerState = ArcadeGameManager.ArcadeGameManagerState.Lost;
				}
				else
				{
					bool continuePlaying = false;
					foreach (Transform child in transform.parent)
					{
						if (child.name == "WaterDrop")
						{
							continuePlaying = true;
						}
						else if (child.name == "Glass")
						{
							foreach (Transform child2 in child)
							{
								if (child2.name == "WaterDrop")
								{
									continuePlaying = true;
									break;
								}
							}
						}

						if (continuePlaying)
						{
							break;
						}
					}
					if (!continuePlaying)
					{
						ArcadeGameManager.instance.arcadeGameManagerState = ArcadeGameManager.ArcadeGameManagerState.Lost;
					}
				}
			}
		}

		void OnCollisionStay(Collision other)
		{
			if (other.gameObject.name == "WaterDrop" && transform.position.x - CameraScript.instance.transform.position.x > 45)
			{
				ArcadeGameManager.instance.arcadeGameManagerState = ArcadeGameManager.ArcadeGameManagerState.Won;
			}
		}
	}
}
