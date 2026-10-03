using UnityEngine;
using System.Collections;

namespace Games.ToEveryoneIllNeverMeet
{
	public class StepIntoMyEyes : MonoBehaviour {


	    void OnTriggerEnter(Collider other)
	    {
	        if (other.tag == "Player")
	        {
	            FadeInUI.globalText.text.text = "Step into my eyes";
	            FadeInUI.globalText.alpha = 6f;
	        }
	    }
	}
}
