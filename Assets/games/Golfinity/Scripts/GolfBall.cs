using UnityEngine;
using System.Collections;

namespace Games.Golfinity
{
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
	    public LayerMask groundLayerMask;
	    public LayerMask mudLayerMask;
	    private bool inMud;

	    void Awake ()
	    {
	        instance = this;
	        body = GetComponent<Rigidbody2D>();
	        aimLine.localScale = Vector3.zero;
	        lastPos = transform.position;
	        audioSource = GetComponent<AudioSource>();
	        collisionPlace = transform.position;
		}
		
		void Update ()
	    {
	        if (UIReferences.cheatPopup.gameObject.activeSelf || UIReferences.levelScorePopup.gameObject.activeSelf || UIReferences.optionsPopup.gameObject.activeSelf || UIReferences.upgradePopup.gameObject.activeSelf)
	        {
	            draggingMouse = false;
	            lastPos = transform.position;
	            return;
	        }
	        bool canHit = Geometry.lengthOfVector3(lastPos - transform.position) < MinSpeed * Game.dt;
	        if (canHit)
	        {
	            var raycastHitGroundRight = Physics2D.Raycast(transform.position + new Vector3(-0.5f, -0.75f, 0f), Vector3.right, 1f, groundLayerMask);
	            var raycastHitGroundDown = Physics2D.Raycast(transform.position, Vector3.down, 0.6f, groundLayerMask);
	            canHit = (raycastHitGroundRight.transform != null || raycastHitGroundDown.transform != null);
	        }
	        if (canHit)
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
	                        Game.lastHitPos = transform.position;
	                        body.AddForce(Geometry.createVector2(aimAngle, aimLength * 5000f));
	                        if (!inMud) CreateTerrainParticle(false);
	                        Game.noOfStrokes++;
	                        Game.noOfStrokesSinceBeginningOfLevel++;
	                        PlayerPrefs.SetInt("noOfStrokes", Game.noOfStrokes);
	                        if (Game.soundOn)
	                        {
	                            audioSource.volume = 1.5f * aimLength / AimMaxLength;
	                            audioSource.pitch = 0.8f + 0.3f * aimLength / AimMaxLength + Random.value * 0.1f;
	                            audioSource.Play();
	                        }
	                        if (UIReferences.tutorial.gameObject.activeSelf && aimLength >= AimMinLength * 2f) UIReferences.tutorial.Hide();
	                    }
	                }
	            }
	        }
	        else
	        {
	            draggingMouse = false;
	            aimLine.localScale = Vector3.zero;
	        }
	        var raycastHitMud = Physics2D.Raycast(transform.position + Vector3.left * 0.05f, Vector3.right, 0.1f, mudLayerMask);
	        bool inMudNew = (raycastHitMud.transform != null);
	        if (inMudNew != inMud)
	        {
	            CreateTerrainParticle(true);
	            inMud = inMudNew;
	        }
	        lastPos = transform.position;
		}

	    private void FixedUpdate()
	    {
	        if (inMud) body.linearVelocity *= 0.5f;
	    }

	    public void ResetBall()
	    {
	        transform.position = startPos;
	    }

	    void OnCollisionEnter2D(Collision2D other)
	    {
	        if (Geometry.lengthOfVector3(transform.position - collisionPlace) > 2f)
	        {
	            float speed = Geometry.lengthOfVector2(body.linearVelocity);
	            float angle = Geometry.angleOfVector2(body.linearVelocity);
	            if (Game.terrainEffectOn)
	            {
	                TerrainParticle.create(5, transform.position + Vector3.forward * 2f, angle, 2f, 2f + speed * 0.1f, 80f);
	            }
	            Game.instance.SoundBallHitWall();
	        }
	        collisionPlace = transform.position;
	    }

	    void CreateTerrainParticle(bool mud)
	    {
	        if (Game.terrainEffectOn)
	        {
	            if (mud)
	            {
	                TerrainParticle.create(10, transform.position + Vector3.forward * 2f + Vector3.down * 0.5f, 90f, body.linearVelocity.magnitude * 0.1f, 3f + body.linearVelocity.magnitude * 0.1f, 40f, mud);
	            }
	            else
	            {
	                TerrainParticle.create(10, transform.position + Vector3.forward * 2f + Vector3.down * 0.5f, aimAngle, 2.25f * aimLength * 0.2f, 7f + aimLength * 0.3f, 40f, mud);
	            }
	        }
	    }
	}

}
