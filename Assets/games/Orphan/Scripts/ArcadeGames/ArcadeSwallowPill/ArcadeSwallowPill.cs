using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class ArcadeSwallowPill : MonoBehaviour {

		private GameObject swallowPill;
		private bool endedFirstFrame;

		void Start ()
		{
			swallowPill = Instantiate(Resources.Load ("Orphan/ArcadeSwallowPillEnvironment") as GameObject, 
				CameraScript.instance.transform.position + 1.5f * Vector3.forward, Quaternion.identity) as GameObject;
			swallowPill.transform.parent = transform;
			swallowPill.name = "ArcadeSwallowPillEnvironment";
		}

		void Update ()
		{
			if (ArcadeGameManager.instance.arcadeGameManagerState == ArcadeGameManager.ArcadeGameManagerState.FadingOut)
			{
				if (!endedFirstFrame)
				{
					endedFirstFrame = true;
					foreach (Transform child in transform)
					{
						if (child.name == "ArcadeSwallowPillEnvironment")
						{
							Destroy(child.gameObject);
						}
					}
					PlayerScript.instance.canWalk = true;
				}
			}
		}
	}
}
