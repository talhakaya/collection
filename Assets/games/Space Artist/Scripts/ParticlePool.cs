using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.SpaceArtist
{
	public class ParticlePool : MonoBehaviour
	{
	    private static ParticlePool instance;
	    private List<Particle> particles;
	    private int index;

		void Start ()
	    {
	        instance = this;
	        particles = new List<Particle>();
	        foreach (Transform child in transform)
	        {
	            particles.Add(child.GetComponent<Particle>());
	        }
		}
		
		void Update ()
	    {
		
		}

	    public static void get(Vector3 pos, float rotation, int recursionCount = 0, float scale = 1f)
	    {
	        Particle p = instance.particles[instance.index];
	        p.transform.position = pos;
	        p.transform.eulerAngles = Vector3.forward * rotation;
	        p.recursionCount = recursionCount;
	        p.scale = scale;
	        p.gameObject.SetActive(true);
	        instance.index++;
	        if (instance.index >= instance.particles.Count)
	        {
	            instance.index = 0;
	        }
	    }
	}

}
