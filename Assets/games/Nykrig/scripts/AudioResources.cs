using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class AudioResources : MonoBehaviour {
	    public static AudioResources instance;
	    public AudioClip nykrig;
	    public AudioClip hitMetal;
	    public AudioClip hitPlastic;
	    public AudioClip[] breakStick;
	    public AudioClip spring;
	    public AudioClip[] ballHitWall;

	    void Awake () {
	        instance = this;
	    }
	}

}
