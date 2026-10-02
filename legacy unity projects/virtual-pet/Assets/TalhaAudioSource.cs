using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TalhaAudioSource : MonoBehaviour
{
    public static List<AudioSource> sources;
    public static int noOfSources;
    public static int index;

	// Use this for initialization
	void Start () {
	    if (sources == null)
        {
            sources = new List<AudioSource>();
            index = 0;
            noOfSources = 0;
        }
        sources.Add(GetComponent<AudioSource>());
        noOfSources++;
	}
	
	public static AudioSource getSource ()
    {
	    var toreturn =  sources[index];
        index++;
        if (index >= noOfSources)
        {
            index = 0;
        }
        return toreturn;
	}
}
