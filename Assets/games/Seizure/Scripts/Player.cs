using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collection.Controls;

namespace Games.Seizure
{
	public class Player : MonoBehaviour
	{
	    public float inputMult = 1f;
	    public ReflectionMan reflect;
	    public Animator anim;

	    private string currentAnim;

	    void Update()
	    {
	        Vector3 deltaPos = new Vector3(TaloketoInputManager.GetAxisRaw("Horizontal"), TaloketoInputManager.GetAxisRaw("Vertical"), 0f) * Time.deltaTime * inputMult;
	        transform.position += deltaPos;
	        if (reflect != null)
	        {
	            if (TaloketoInputManager.GetButton("Attack"))
	            {
	                reflect.scaleSpeed = Mathf.Clamp(reflect.scaleSpeed + Time.deltaTime * 0.01f, 0.01f, 0.1f);
	                PlayAnim("attack");
	            }
	            else if (deltaPos.magnitude > 0f)
	            {
	                reflect.scaleSpeed = Mathf.Clamp(reflect.scaleSpeed + Time.deltaTime * 0.01f, 0.01f, 0.1f);
	                PlayAnim("run");
	                if (anim != null) turnTo = 90f * TaloketoInputManager.GetAxisRaw("Horizontal");
	            }
	            else
	            {
	                reflect.scaleSpeed = Mathf.Clamp(reflect.scaleSpeed - Time.deltaTime * 0.1f, 0.01f, 0.1f);
	                PlayAnim("wait");
	                if (anim != null) turnTo = 0f;
	            }
	        }
	    }

	    // In the collection: the two turns were DOTween calls (DOLocalRotate over 0.2 seconds).
	    // DOTween is not part of the collection, so the turn is done here, at the same speed.
	    private float turnTo;

	    void LateUpdate()
	    {
	        if (anim != null)
	        {
	            anim.transform.localRotation = Quaternion.RotateTowards(anim.transform.localRotation, Quaternion.Euler(0f, turnTo, 0f), 450f * Time.deltaTime);
	        }
	    }

	    void PlayAnim(string animName)
	    {
	        if (anim == null) return;
	        if (currentAnim == animName) return;
	        currentAnim = animName;
	        anim.CrossFade(currentAnim, 0.2f);
	    }
	}
}
