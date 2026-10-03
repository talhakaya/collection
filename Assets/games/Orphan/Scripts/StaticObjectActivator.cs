using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class StaticObjectActivator : MonoBehaviour {
		public static StaticObjectActivator instance;
		public string nextLevel;
		void Start ()
		{
			instance = this;
			gameObject.SetActive(false);
		}

		void Update ()
		{

		}

		public static void ActivateInstance()
		{
			instance.gameObject.SetActive(true);
		}

		public void OnTriggerEnter(Collider other)
		{
			if (nextLevel != "")
			{
				GameManagerScript.changeLevel(nextLevel);
			}
			else
			{
				Debug.Log("WTF do you want me to do? Which scene?");
			}
		}
	}
}
