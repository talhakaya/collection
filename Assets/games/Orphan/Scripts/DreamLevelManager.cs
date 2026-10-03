using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class DreamLevelManager : MonoBehaviour
	{
		public static DreamLevelManager instance;

		public int jewelCount = 0;
		public int maxJewelCount;

		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{

		}

		void Update ()
		{

		}

		public static void incrementJewelCount()
		{
			instance.jewelCount ++;
			if (instance.jewelCount == instance.maxJewelCount)
			{
				GameManagerScript.restartLevel();
			}
		}
	}
}
