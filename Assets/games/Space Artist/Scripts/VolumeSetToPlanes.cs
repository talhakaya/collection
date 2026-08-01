using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class VolumeSetToPlanes : MonoBehaviour
	{
	    private AudioSource audioSource;
	    public float maxVolume = 1f;
	    public bool invert;

		void Start ()
	    {
	        audioSource = GetComponent<AudioSource>();
		}
		
		void Update ()
	    {
	        audioSource.volume = maxVolume * (invert ? (1f - PlaneManager.openColorAlpha) : PlaneManager.openColorAlpha);
		}
	}

}
