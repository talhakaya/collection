using UnityEngine;
using System.Collections;

public class ArcadeCrying : MonoBehaviour {
	
	
	
	void Start ()
	{
		GameObject environment = Instantiate(Resources.Load ("Arcade/ArcadeCrying/ArcadeCryingEnvironment") as GameObject, 
			CameraScript.instance.transform.position + 1.5f * Vector3.forward, Quaternion.identity) as GameObject;
		environment.name = "ArcadeCryingEnvironment";
		environment.transform.parent = transform;
	}
	
	void Update ()
	{
		if (ArcadeGameManager.instance.arcadeGameManagerState == ArcadeGameManager.ArcadeGameManagerState.FadingOut)
		{
			foreach (Transform child in transform)
			{
				if (child.name == "ArcadeCryingEnvironment")
				{
					Destroy(child.gameObject);
					break;
				}
			}
		}
	}
}
