using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour
{
	void Start ()
    {
        if (!Game.musicOn)
        {
            Destroy(GetComponent<AudioSource>());
        }
	}
}
