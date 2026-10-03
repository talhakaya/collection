using UnityEngine;
using System.Collections;

namespace Games.CasketFucker
{
	public class TapToAnimate : MonoBehaviour {
	    public Sprite[] sprites;
	    public float period = 0.2f;
	    private int i = 0;
	    private SpriteRenderer spriteRenderer;
	    public bool autoTapped;
	    public float dtMultiplier;
	    public float shakeAmount;
	    public Vector3 shakeDirection;
	    public float rotation;
	    public float nextState;
	    private float stateCounter;
	    private float time;
	    private Vector3 firstPos;
	    public int[] audioPlayFrames;
	    public float deltaPitch;
	    public int[] activateObjectFrames;
	    public GameObject objectToActivate;

	    void Start()
	    {
	        spriteRenderer = GetComponent<SpriteRenderer>();
	        spriteRenderer.sprite = sprites[0];
	        firstPos = transform.position;
	    }

	    void Update()
	    {
	        // In the collection: clamped; the float maths can land on Length.
	        int newI = Mathf.Min(Mathf.FloorToInt((stateCounter % (period * sprites.Length)) / period), sprites.Length - 1);
	        if (i != newI)
	        {
	            i = newI;
	            if (GetComponent<AudioSource>() != null)
	            {
	                bool playSound = false;
	                for (int j = 0; j < audioPlayFrames.Length; j++)
	                {
	                    if (audioPlayFrames[j] == i)
	                    {
	                        playSound = true;
	                        break;
	                    }
	                }
	                if (playSound)
	                {
	                    GetComponent<AudioSource>().pitch += deltaPitch;
	                    GetComponent<AudioSource>().Play();
	                }
	            }
	            if (objectToActivate != null)
	            {
	                bool activate = false;
	                for (int j = 0; j < activateObjectFrames.Length; j++)
	                {
	                    if (activateObjectFrames[j] == i)
	                    {
	                        activate = true;
	                        break;
	                    }
	                }
	                objectToActivate.SetActive(activate);
	            }
	        }
	        spriteRenderer.sprite = sprites[i];

	        if (Game.anyKeyDown || autoTapped)
	        {
	            time += Game.dt * dtMultiplier;
	        }

	        if (time > 0f)
	        {
	            time -= Game.dt;
	            transform.position = firstPos + Geometry.createVector3(Random.value * 360f, Random.Range(0f, shakeAmount)) + shakeDirection * Random.value;
	            transform.eulerAngles += Vector3.forward * rotation * Game.dt;
	            stateCounter += Game.dt;
	            if (stateCounter >= nextState)
	            {
	                State.next();
	            }
	        }
	    }
	}
}
