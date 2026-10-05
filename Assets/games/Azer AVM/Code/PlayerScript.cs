using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.AzerAVM
{
	public enum PlayerNo { P1, P2, P3, P4 }

	public class PlayerScript : MonoBehaviour {

	    public static PlayerScript[] list;
	    public bool isRight;
	    public PlayerNo playerNo;
	    public float speedH;
	    public float jumpForceFirst;
	    public float jumpForceCont;
	    public float jumpFixSpeed;
	    private bool justPressedJump;
	    private bool justReleasedJump;
	    private bool pressingJump;
	    private bool onGround = false;
	    private float horizontalAxis;
	    private float walkAnimCount;
	    public Transform arm0;
	    public Transform arm1;
	    public Transform leg0;
	    public Transform leg1;


		void Start ()
	    {
	        list[(int) playerNo] = this;
	        SpriteEffect.make(Effect.Blur, gameObject, false, true, transform);
		}

		void Update ()
	    {
	        if (playerNo == PlayerNo.P1)
	        {
	            horizontalAxis = TaloketoInputManager.GetAxisRaw("P1Horizontal");
	            justPressedJump = TaloketoInputManager.GetButtonDown("P1Jump");
	            pressingJump = TaloketoInputManager.GetButton("P1Jump");
	            justReleasedJump = TaloketoInputManager.GetButtonUp("P1Jump");
	        }
	        else if (playerNo == PlayerNo.P2)
	        {
	            horizontalAxis = TaloketoInputManager.GetAxisRaw("P2Horizontal");
	            justPressedJump = TaloketoInputManager.GetButtonDown("P2Jump");
	            pressingJump = TaloketoInputManager.GetButton("P2Jump");
	            justReleasedJump = TaloketoInputManager.GetButtonUp("P2Jump");
	        }
	        else if (playerNo == PlayerNo.P3)
	        {
	            horizontalAxis = TaloketoInputManager.GetAxisRaw("P3Horizontal");
	            justPressedJump = TaloketoInputManager.GetButtonDown("P3Jump");
	            pressingJump = TaloketoInputManager.GetButton("P3Jump");
	            justReleasedJump = TaloketoInputManager.GetButtonUp("P3Jump");
	        }
	        else if (playerNo == PlayerNo.P4)
	        {
	            horizontalAxis = TaloketoInputManager.GetAxisRaw("P4Horizontal");
	            justPressedJump = TaloketoInputManager.GetButtonDown("P4Jump");
	            pressingJump = TaloketoInputManager.GetButton("P4Jump");
	            justReleasedJump = TaloketoInputManager.GetButtonUp("P4Jump");
	        }

	        padInput();

	        if (GetComponent<Rigidbody2D>().linearVelocity.x < -0.1f)
	        {
	            isRight = true;
	        }
	        else if (GetComponent<Rigidbody2D>().linearVelocity.x > 0.1f)
	        {
	            isRight = false;
	        }

		    if (isRight)
	        {
	            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
	        }
	        else
	        {
	            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
	        }

	        if (pressingJump)
	        {
	            fixRotation();
	        }
	        if (onGround)
	        {
	            // In the collection: this force is added every frame, so it is scaled to the frame
	            // time (as at 60 frames a second) to stay the same at any frame rate.
	            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForceFirst * Game.dt * 60f);

	        }
	        else
	        {
	            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForceCont * Game.dt);
	        }
	        GetComponent<Rigidbody2D>().AddForce(Vector2.right * horizontalAxis * speedH * Game.dt);
	        if (horizontalAxis == 0)
	        {
	            leg0.localEulerAngles = Vector3.zero;
	            leg1.localEulerAngles = Vector3.zero;
	        }
	        else
	        {
	            walkAnimCount += Game.dt;
	            if (walkAnimCount > 0.1f)
	            {
	                walkAnimCount = 0f;
	                leg0.localEulerAngles = Vector3.forward * Random.Range(-60f, 60f);
	                leg1.localEulerAngles = Vector3.forward * Random.Range(-60f, 60f);
	                arm0.localEulerAngles = Vector3.forward * Random.Range(-60f, 60f);
	                arm1.localEulerAngles = Vector3.forward * Random.Range(-60f, 60f);
	            }
	        }
		}

	    // In the collection: gamepads, which the original never had. With two pads each
	    // player has one (left stick or d-pad to move, A to straighten up). With one pad
	    // the two players share it: left stick and LB/LT for the first, right stick and
	    // RB/RT for the second.
	    private bool padJumpOld;

	    void padInput()
	    {
	        var pads = UnityEngine.InputSystem.Gamepad.all;
	        int no = (int) playerNo;
	        float axis = 0f;
	        bool jump = false;
	        if (pads.Count >= 2 && no < pads.Count)
	        {
	            var pad = pads[no];
	            axis = pad.leftStick.x.ReadValue();
	            if (pad.dpad.left.isPressed) axis = -1f;
	            if (pad.dpad.right.isPressed) axis = 1f;
	            jump = pad.buttonSouth.isPressed;
	        }
	        else if (pads.Count == 1 && no < 2)
	        {
	            var pad = pads[0];
	            if (no == 0)
	            {
	                axis = pad.leftStick.x.ReadValue();
	                if (pad.dpad.left.isPressed) axis = -1f;
	                if (pad.dpad.right.isPressed) axis = 1f;
	                jump = pad.leftShoulder.isPressed || pad.leftTrigger.isPressed;
	            }
	            else
	            {
	                axis = pad.rightStick.x.ReadValue();
	                jump = pad.rightShoulder.isPressed || pad.rightTrigger.isPressed;
	            }
	        }
	        if (Mathf.Abs(axis) >= 0.19f)
	        {
	            horizontalAxis = Mathf.Sign(axis);
	        }
	        justPressedJump = justPressedJump || (jump && !padJumpOld);
	        justReleasedJump = justReleasedJump || (!jump && padJumpOld);
	        pressingJump = pressingJump || jump;
	        padJumpOld = jump;
	    }

	    // In the collection: stands in for Input.anyKeyDown.
	    public static bool anyKeyDown()
	    {
	        // In the collection: not while the pause screen is up, nor the press that opens it.
	        if (Collection.Controls.TaloketoInputManager.Blocked) return false;
	        var keyboard = UnityEngine.InputSystem.Keyboard.current;
	        if (keyboard != null && keyboard.anyKey.wasPressedThisFrame) return true;
	        var mouse = UnityEngine.InputSystem.Mouse.current;
	        if (mouse != null && mouse.leftButton.wasPressedThisFrame) return true;
	        foreach (var pad in UnityEngine.InputSystem.Gamepad.all)
	        {
	            if (pad.buttonSouth.wasPressedThisFrame || pad.buttonEast.wasPressedThisFrame || pad.buttonWest.wasPressedThisFrame || pad.buttonNorth.wasPressedThisFrame
	                || pad.leftShoulder.wasPressedThisFrame || pad.rightShoulder.wasPressedThisFrame) return true;
	        }
	        return false;
	    }

	    // In the collection: Restart is not to fire as part of the exit chord.
	    public static bool exitChordHeld()
	    {
	        var keyboard = UnityEngine.InputSystem.Keyboard.current;
	        if (keyboard != null && keyboard.shiftKey.isPressed) return true;
	        foreach (var pad in UnityEngine.InputSystem.Gamepad.all)
	        {
	            if (pad.selectButton.isPressed) return true;
	        }
	        return false;
	    }

	    void fixRotation()
	    { 
	        if (transform.eulerAngles.z != 0)
	        {
	            if (transform.eulerAngles.z < 180f)
	            {
	                if (transform.eulerAngles.z < jumpFixSpeed * Game.dt)
	                {
	                    transform.eulerAngles -= Vector3.forward * transform.eulerAngles.z;
	                }
	                else
	                {
	                    transform.eulerAngles -= Vector3.forward * jumpFixSpeed * Game.dt;
	                }
	            }
	            else
	            {
	                if (transform.eulerAngles.z > 360 - jumpFixSpeed * Game.dt)
	                {
	                    transform.eulerAngles += Vector3.forward * transform.eulerAngles.z;
	                }
	                else
	                {
	                    transform.eulerAngles += Vector3.forward * jumpFixSpeed * Game.dt;
	                }
	            }

	            if (transform.eulerAngles.z < 2f || transform.eulerAngles.z > 358f)
	            {
	                transform.eulerAngles = Vector3.zero;
	            }
	        }
	    }

	    void OnCollisionEnter2D(Collision2D other)
	    {
	        if (other.gameObject.name == "Platform" && other.contacts[0].point.y < transform.position.y)
	        {
	            onGround = true;
	            SpriteEffect.blurConst = 1f;
	            Game.instance.tasak.pitch = Random.Range(0.9f, 1.1f);
	            Game.instance.tasak.Play();
	        }
	    }

	    void OnCollisionExit2D(Collision2D other)
	    {
	        // In the collection: Unity 6 gives no contact points when a collision ends, so the
	        // side that was touching cannot be tested here any more.
	        if (other.gameObject.name == "Platform")
	        {
	            onGround = false;
	        }
	    }
	}
}
