using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public float inputMult = 1f;
    public ReflectionMan reflect;
    public Animator anim;

    private string currentAnim;

    void Update()
    {
        Vector3 deltaPos = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f) * Time.deltaTime * inputMult;
        transform.position += deltaPos;
        if (reflect != null)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                reflect.scaleSpeed = Mathf.Clamp(reflect.scaleSpeed + Time.deltaTime * 0.01f, 0.01f, 0.1f);
                PlayAnim("attack");
            }
            else if (deltaPos.magnitude > 0f)
            {
                reflect.scaleSpeed = Mathf.Clamp(reflect.scaleSpeed + Time.deltaTime * 0.01f, 0.01f, 0.1f);
                PlayAnim("run");
                if (anim != null) anim.transform.DOLocalRotate(new Vector3(0f, 90f * Input.GetAxisRaw("Horizontal"), 0f), 0.2f);
            }
            else
            {
                reflect.scaleSpeed = Mathf.Clamp(reflect.scaleSpeed - Time.deltaTime * 0.1f, 0.01f, 0.1f);
                PlayAnim("wait");
                if (anim != null) anim.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.2f);
            }
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
