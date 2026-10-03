using UnityEngine;
using System.Collections;

namespace Games.ToEveryoneIllNeverMeet
{
	public class SettingUpStars : MonoBehaviour {

		void Start ()
	    {
		    foreach (Transform child in transform)
	        {
	            child.eulerAngles = new Vector3(Random.value * 360f, Random.value * 360f, Random.value * 360f);
	            child.localScale *= Random.Range(1f, 16f);
	            foreach (Transform child2 in child)
	            {
	                child2.localPosition *= Random.Range(0.25f, 4f);
	            }
	        }
		}
	}
}
