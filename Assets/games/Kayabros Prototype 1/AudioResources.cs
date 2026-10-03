using UnityEngine;
using System.Collections;

namespace Games.KayabrosPrototype1
{
	public class AudioResources : MonoBehaviour {
	    public static AudioResources instance;
	    public AudioClip example;

	    void Awake () {
	        instance = this;
	    }
	}
}
