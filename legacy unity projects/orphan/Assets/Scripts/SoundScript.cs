using UnityEngine;
using System.Collections;

public class SoundScript : MonoBehaviour
{
	private AudioSource audioSource;
	
	void Start ()
	{
		audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	void Update ()
	{
		if (Random.Range(0f,1f) > 0.5f)
		{
			audioSource.pitch += 0.001f;
		}
		else
		{
			audioSource.pitch -= 0.001f;
		}
	}
}
