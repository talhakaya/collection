using UnityEngine;
using Collection.Controls;
using System.Collections;

namespace Games.WhereIsHe
{
	public class Villain : MonoBehaviour {

	    public GameObject[] disableWhenEnter;

		void Update ()
	    {
		    if (TaloketoInputManager.GetButtonDown("Submit"))
	        {
	            for (int i = 0; i < disableWhenEnter.Length; i++)
	            {
	                disableWhenEnter[i].SetActive(false);
	            }
	        }
		}
	}
}
