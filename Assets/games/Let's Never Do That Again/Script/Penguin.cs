using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.LetsNeverDoThatAgain
{
	public class Penguin : MonoBehaviour
	{
	    public static Penguin instance;

	    public TalhaAnimation walk;
	    public TalhaAnimation idle;
	    public float speed;

	    private TalhaAnimation current;
	    private bool walkingOld = true;
	    private bool lookingLeft = false;
	    private bool lookingLeftOld = false;
	    private float walkingLastTime = 0f;
	    private bool hitSounded;

	    void Awake()
	    {
	        instance = this;
	    }

		void Start ()
	    {
	        // In the collection: the penguin of a new scene knew no animation yet, and only
	        // changed to one when walking started or stopped. Arriving with the keys already
	        // held, nothing changed and it slid along on one frame. It starts on the right one.
	        walk.enabled = false;
	        idle.enabled = false;
	        bool walking = TaloketoInputManager.GetAxisRaw("Horizontal") != 0 || TaloketoInputManager.GetAxisRaw("Vertical") != 0;
	        changeAnimation(walking ? walk : idle);
	        walkingOld = walking;

	        // In the collection: in the third scene (scene1.5) the penguin could be tipped
	        // onto its side as the scene began, and the scene could not be finished. Only the
	        // scene before it has the penguin fall over; here it stays upright.
	        if (Scene0.sceneCount == 2)
	        {
	            GetComponent<Rigidbody2D>().freezeRotation = true;
	        }
		}

		void Update ()
	    {
	        bool walking = false;
	        if (TaloketoInputManager.GetAxisRaw("Horizontal") != 0f)
	        {
	            lookingLeft = (TaloketoInputManager.GetAxisRaw("Horizontal") < 0);
	        }
	        if (TaloketoInputManager.GetAxisRaw("Horizontal") != 0 || TaloketoInputManager.GetAxisRaw("Vertical") != 0)
	        {
	            walking = true;
	            Vector3 force = Geometry.normalizeVector2(new Vector2(TaloketoInputManager.GetAxisRaw("Horizontal"), TaloketoInputManager.GetAxisRaw("Vertical")), speed);
	            GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
	        }
	        if (lookingLeft && !lookingLeftOld)
	        {
	            transform.localScale = new Vector3(-1, 1, 1);
	        }
	        else if (!lookingLeft && lookingLeftOld)
	        {
	            transform.localScale = new Vector3(1, 1, 1);
	        }
	        if (walking && !walkingOld)
	        {
	            changeAnimation(walk);
	        }
	        else if (!walking && walkingOld)
	        {
	            changeAnimation(idle);
	        }
	        if (walking)
	        {
	            float walkTime = Game.time % 0.8f;
	            if (walkTime >= 0.2f && walkingLastTime < 0.2f)
	            {
	                stepSound();
	            }
	            else if (walkTime >= 0.6f && walkingLastTime < 0.6f)
	            {
	                stepSound();
	            }
	            walkingLastTime = walkTime;
	        }
	        walkingOld = walking;
	        lookingLeftOld = lookingLeft;
	    }

	    void stepSound()
	    {
	        if (Scene0.sceneCount < 4)
	        {
	            Game.instance.aStep.pitch = Random.Range(0.6f, 1.2f);
	            Game.instance.aStep.volume = 0.5f;
	            Game.instance.aStep.Play();
	        }
	    }

	    void changeAnimation(TalhaAnimation newAnim)
	    {
	        if (current != null)
	        {
	            current.enabled = false;
	        }
	        newAnim.enabled = true;
	        current = newAnim;
	    }

	    void OnCollisionEnter2D(Collision2D other)
	    {
	        if (Scene0.sceneCount == 0 && !hitSounded)
	        {
	            hitSounded = true;
	            Game.instance.aHit.Play();
	        }
	        else if (Scene0.sceneCount == 1 && (transform.eulerAngles.z > 80f || transform.eulerAngles.z < -80f))
	        {
	            Game.instance.aHit.Play();
	        }
	    }
	}
}
