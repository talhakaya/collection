using UnityEngine;
using System.Collections;

public class GolfBall : MonoBehaviour
{
    public static GolfBall instance;
    private Rigidbody2D body;
    private const float MinSpeed = 30f;
    public bool draggingMouse;
    private Vector3 mouseDragStartPos;
    public Transform aimLine;
    private const float AimMinLength = 1f;
    private const float AimMaxLength = 15f;
    public Vector3 startPos;
    private float aimLength;
    private float aimAngle;
    private Vector3 lastPos;
    private AudioSource audioSource;
    private Vector3 collisionPlace;
    //private float collisionTime;

	void Awake ()
    {
        instance = this;
        body = GetComponent<Rigidbody2D>();
        aimLine.localScale = Vector3.zero;
        lastPos = transform.position;
        audioSource = GetComponent<AudioSource>();
        collisionPlace = transform.position;
        //collisionTime = 0f;
	}

    void Start()
    {
        updateTrailEffect();
    }

    public void updateTrailEffect()
    {
        GetComponent<TrailRenderer>().enabled = Game.ballTrailEffectOn;
    }
	
	void Update ()
    {
        //if (collisionTime > 0f)
        //{
        //    collisionTime -= Game.dt;
        //}
        if (Geometry.lengthOfVector3(lastPos - transform.position) < MinSpeed * Game.dt)
        {
            if (draggingMouse)
            {
                aimLength = Geometry.lengthOfVector3(MousePosition.get - mouseDragStartPos);
                if (aimLength < AimMinLength)
                {
                    aimLine.localScale = Vector3.zero;
                    aimLength = 0;
                }
                else
                {
                    if (aimLength > AimMaxLength)
                    {
                        aimLength = AimMaxLength;
                    }
                    if (Game.reverseShooting)
                    {
                        aimAngle = Geometry.angleOfVector3(mouseDragStartPos - MousePosition.get);
                    }
                    else
                    {
                        aimAngle = Geometry.angleOfVector3(MousePosition.get - mouseDragStartPos);
                    }
                    aimLine.localScale = new Vector3(aimLength, 1f, 1f);
                    aimLine.eulerAngles = aimAngle * Vector3.forward;
                    aimLine.position = transform.position + Geometry.createVector3(aimAngle, 1.2f) - Vector3.forward;
                }
            }
            else
            {
                aimLength = 0;
                aimLine.localScale = Vector3.zero;
            }

            if (Game.cameraMove)
            {
                draggingMouse = false;
                aimLine.localScale = Vector3.zero;
            }
            else if (Game.inputDown)
            {
                draggingMouse = true;
                mouseDragStartPos = MousePosition.get;
            }
            else if (Game.inputUp)
            {
                if (draggingMouse)
                {
                    draggingMouse = false;
                    if (aimLength >= AimMinLength)
                    {
                        body.AddForce(Geometry.createVector2(aimAngle, aimLength * 5000f));
                        if (Game.terrainEffectOn)
                        {
                            TerrainParticle.createDust(10, transform.position + Vector3.forward * 2f + Vector3.down * 0.5f, aimAngle, 2.25f * aimLength * 0.2f, 7f + aimLength * 0.3f);
                        }
                        if (!Game.instance.isMenu)
                        {
                            Game.noOfStrokes++;
                            PlayerPrefs.SetInt("noOfStrokes", Game.noOfStrokes);
                        }
                        if (Game.soundOn)
                        {
                            audioSource.volume = 1.5f * aimLength / AimMaxLength;
                            audioSource.pitch = 0.8f + 0.3f * aimLength / AimMaxLength + Random.value * 0.1f;
                            audioSource.Play();
                        }
                    }
                }
            }
        }
        else
        {
            draggingMouse = false;
            aimLine.localScale = Vector3.zero;
        }
        lastPos = transform.position;
	}

    public void ResetBall()
    {
        transform.position = startPos;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (Geometry.lengthOfVector3(transform.position - collisionPlace) > 2f)// && collisionTime <= 0f)
        {
            float speed = Geometry.lengthOfVector2(body.velocity);
            float angle = Geometry.angleOfVector2(body.velocity);
            if (Game.terrainEffectOn)
            {
                TerrainParticle.createDust(5, transform.position + Vector3.forward * 2f, angle, 2f, 2f + speed * 0.1f, 80f);
            }
            if (Game.soundOn)
            {
                Game.instance.SoundBallHitWall();
            }
        }
        collisionPlace = transform.position;
        //collisionTime = 0.05f;
    }
}
