using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Games.SpaceArtist
{
	public class PlanePlayerFollow : MonoBehaviour
	{
	    private Collider collider;
	    private Renderer rendererr;
	    private float minA = 0.3f;
	    private float maxA = 0.7f;
	    private float aTimer;
	    private bool neverPlayedSound;
	    public AudioSource musicToShut;
	    private float lastTimer;
		void Start ()
	    {
	        collider = GetComponent<Collider>();
	        rendererr = GetComponent<Renderer>();
	        neverPlayedSound = true;
	        lastTimer = 0f;
		}
		
		void Update ()
	    {
	        collider.enabled = (Game.level == 26);
	        if (Game.level == 26)
	        {
	            if (neverPlayedSound)
	            {
	                neverPlayedSound = false;
	                musicToShut.Stop();
	                GetComponent<AudioSource>().Play();
	            }
	            else if (!GetComponent<AudioSource>().isPlaying)
	            {
	                lastTimer += Game.dt;
	                if (lastTimer > 1f)
	                {
	                    SceneManager.LoadScene("Assets/games/Space Artist/menu.unity");
	                }
	            }
	            float a = rendererr.material.GetColor("_TintColor").a;
	            if (a < minA)
	            {
	                a += Game.dt * 0.1f;
	            }
	            else
	            {
	                aTimer += Game.dt * 0.1f;
	                aTimer = aTimer % ((maxA - minA) * 2f);
	                if (aTimer > (maxA - minA))
	                {
	                    a = minA + (((maxA - minA) * 2f) - aTimer);
	                }
	                else
	                {
	                    a = minA + aTimer;
	                }
	            }
	            rendererr.material.SetColor("_TintColor", new Color(0.5f - lastTimer * 0.5f, 0.5f - lastTimer * 0.5f, 0.5f - lastTimer * 0.5f, a + lastTimer * 0.2f));
	        }
	        else
	        {
	            rendererr.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
	        }
		}
	}

}
