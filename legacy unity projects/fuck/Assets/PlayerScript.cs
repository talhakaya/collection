using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D body;
    public float speed = 5f;
    public Transform[] hairs;
    private List<float> hairStartAngles;
    private List<float> hairMaxAngles;
    private List<float> hairPeriods;
    private List<float> hairTimers;
    private TalhaAnimation talhaAnim;
    private bool isMovingOld;

	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        SpriteEffect.make(Effect.MotionBlur, gameObject);
        talhaAnim = GetComponent<TalhaAnimation>();
        hairMaxAngles = new List<float>();
        hairPeriods = new List<float>();
        hairTimers = new List<float>();
        hairStartAngles = new List<float>();
        for (int i = 0; i < hairs.Length; i++)
        {
            hairMaxAngles.Add(Random.Range(30f, 60f));
            hairPeriods.Add(Random.Range(0.2f, 0.5f));
            hairTimers.Add(0f);
            hairStartAngles.Add(0f);
        }
	}
	
	void FixedUpdate ()
    {
        body.velocity = speed * new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMoving = (Geometry.lengthOfVector2(body.velocity) > 0f);
        talhaAnim.enabled = isMoving;
        body.angularVelocity = isMoving ? (Geometry.differenceOfAnglesNegative(Geometry.angleOfVector2(body.velocity), transform.eulerAngles.z) * 7f) : 0f;
        if (!isMovingOld && isMoving)
        {
            for (int i = 0; i < hairs.Length; i++)
            {
                if (Random.value < 0.5f)
                {
                    hairMaxAngles[i] *= -1f;
                }
                hairTimers[i] = 0f;
                hairStartAngles[i] = hairs[i].localEulerAngles.z;
            }
        }
        else if (isMoving)
        {
            for (int i = 0; i < hairs.Length; i++)
            {
                hairTimers[i] += Game.dt;
                if (hairTimers[i] > hairPeriods[i])
                {
                    hairTimers[i] = 0f;
                    hairMaxAngles[i] *= -1f;
                    hairStartAngles[i] = hairs[i].localEulerAngles.z;
                }
                else
                {
                    float deltaAngle = Geometry.differenceOfAnglesNegative(hairMaxAngles[i], hairStartAngles[i]);
                    hairs[i].localEulerAngles = Vector3.forward * Easing.SineEaseOut(hairTimers[i], hairStartAngles[i], deltaAngle, hairPeriods[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < hairs.Length; i++)
            {
                float deltaAngle = Geometry.differenceOfAnglesNegative(0f, hairs[i].localEulerAngles.z);
                hairs[i].localEulerAngles += Vector3.forward * deltaAngle * 0.03f;
            }
        }
        isMovingOld = isMoving;
	}
}
