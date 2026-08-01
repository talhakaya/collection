using UnityEngine;
using System.Collections;
using System;

namespace Games.Nykrig
{
	public class Player : MonoBehaviour {
	    private TintScript tint;
	    private Rigidbody2D body;
	    public SpriteRenderer lightCone;
	    public SpriteRenderer lightCircle;
	    private Vector3 lightConeScale;
	    private Vector3 lightCircleScale;
	    public float speed = 3f;
	    private bool gamepad;
	    private float mouseAngle;
	    private float mouseDistance;
	    private float fireTimer;
	    public float firePeriod = 0.25f;
	    private bool isDead;
	    public static float nightVisionTimer;
	    public const float nightVisionPeriod = 10f;
	    public CanvasGroup nightVisionCanvasGroup;
	    public int id;
	    public GameObject text;

	    void OnEnable() {
	        nightVisionTimer = 0f;
	        fireTimer = firePeriod * 2f;
	        mouseAngle = 0f;
	        mouseDistance = 1f;
	        isDead = false;
	        lightCone.transform.localScale = lightConeScale;
	        if (!Game.instance.twoPlayers) {
	            gamepad = Game.gamepad;
	        }
	    }

	    void Awake () {
	        tint = GetComponent<TintScript>();
	        body = GetComponent<Rigidbody2D>();
	        lightConeScale = lightCone.transform.localScale;
	        lightCircleScale = lightCircle.transform.localScale;
	        //SpriteEffect.make(Effect.MotionBlur, gameObject);
	        SpriteEffect.make(Effect.MotionBlur, lightCone.gameObject);
	        //SpriteEffect.make(Effect.MotionBlur, lightCircle.gameObject);
	        //SpriteEffect.make(Effect.RGBSplit, gameObject);
	    }
		
		void FixedUpdate () {
	        if (!isDead) {
	            if (!Game.instance.twoPlayers) {
	                if (Input.GetAxis("Horizontal1") != 0f || Input.GetAxis("Vertical1") != 0f) {
	                    gamepad = true;
	                }
	                if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f) {
	                    gamepad = false;
	                }
	            }
	            else {
	                gamepad = (id != 0);
	            }

	            if (gamepad) {
	                Vector2 gamepadVector = new Vector2(Input.GetAxis("Horizontal1"), Input.GetAxis("Vertical1"));
	                float gamepadVectorLength = Geometry.lengthOfVector2(gamepadVector);
	                if (gamepadVectorLength >= 0.2f) {
	                    mouseAngle = Geometry.angleOfVector2(gamepadVector);
	                    mouseDistance = gamepadVectorLength * 5f;
	                }
	                body.linearVelocity = new Vector2(Input.GetAxis("Horizontal0"), Input.GetAxis("Vertical0")) * speed;
	            }
	            else {
	                mouseAngle = Geometry.angleOfVector3(MousePosition.get - transform.position);
	                mouseDistance = Geometry.lengthOfVector3(MousePosition.get - transform.position);
	                body.linearVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
	            }
	            if (Geometry.lengthOfVector2(body.linearVelocity) > 0f) {
	                Wall.lightAlpha = Mathf.Max(0f, Wall.lightAlpha - 5f * Game.dt);
	            }
	            float mouseDistanceClamped = Mathf.Clamp(mouseDistance, 1f, 5f);
	            transform.eulerAngles = new Vector3(0f, 0f, mouseAngle - 90);
	            Game.shadowVector = Geometry.createVector3(mouseAngle, 0.1f * mouseDistanceClamped);
	            //lightCone.transform.localScale = new Vector3(lightConeScale.x * (1.1f - mouseDistanceClamped * 0.1f), lightConeScale.y * (0.9f + mouseDistanceClamped * 0.1f), lightConeScale.z);
	            //lightCone.transform.localPosition = new Vector3(0f, -lightCone.transform.localScale.y * 0.5f, lightCone.transform.localPosition.z);
	            //lightCircle.transform.localScale = lightCircleScale * (1.1f - mouseDistanceClamped * 0.1f);

	            fireTimer += Game.dt;
	            if ((!gamepad && Input.GetButton("Fire")) || (gamepad && (Input.GetAxisRaw("FireAxis") > 0.5f || Input.GetAxisRaw("FireAxisMac") > 0.5f))) {
	                float fPeriod = firePeriod * (nightVisionTimer > 0f ? 1f : 2f);
	                if (fireTimer >= fPeriod) {
	                    float firePower = Mathf.Clamp(fireTimer / fPeriod, 1f, 3f);
	                    fireTimer = 0f;
	                    GameObject go = ObjectPool.firePool.get(transform.position);
	                    go.GetComponent<Fire>().Set(firePower, Geometry.createVector2(mouseAngle, 5f), body.linearVelocity, transform.position);
	                    TalhaAudioSource.PlayInstance(AudioResources.instance.ballHitWall, AudioType.Effect, null, 0.3f * firePower, 1.25f - firePower * 0.25f);
	                }
	            }
	            
	            if (nightVisionTimer > 0f) {
	                nightVisionTimer -= Game.dt;
	                if (nightVisionTimer < 1f) {
	                    nightVisionCanvasGroup.alpha = nightVisionTimer * 0.3f;
	                }
	                else if (nightVisionTimer < nightVisionPeriod - 1f) {
	                    nightVisionCanvasGroup.alpha = 0.3f;
	                }
	                else {
	                    nightVisionCanvasGroup.alpha = (nightVisionPeriod - nightVisionTimer) * 0.3f;
	                }
	            }
	            else {
	                nightVisionTimer = 0f;
	                nightVisionCanvasGroup.alpha = 0f;
	            }
	        }
	        else {
	            transform.localScale -= new Vector3(1f, 1f, 0f) * Game.dt * 10f;
	            if (transform.localScale.x <= 0f) {
	                gameObject.SetActive(false);
	            }
	        }
	        if (body.position.x > Game.levelMaxX) {
	            body.position = new Vector2(Game.levelMaxX, body.position.y);
	        }
	        if (body.position.y > Game.levelMaxY) {
	            body.position = new Vector2(body.position.x, Game.levelMaxY);
	        }
	        if (body.position.x < Game.levelMinX) {
	            body.position = new Vector2(Game.levelMinX, body.position.y);
	        }
	        if (body.position.y < Game.levelMinY) {
	            body.position = new Vector2(body.position.x, Game.levelMinY);
	        }

	        text.SetActive(Game.instance.twoPlayers);
	    }

	    public void Die() {
	        if (!isDead) {
	            isDead = true;
	            TalhaAudioSource.PlayInstance(AudioResources.instance.hitMetal, AudioType.Effect, null, 0.5f, 1f);
	            body.linearVelocity = new Vector2(0f, 0f);
	            transform.localScale = new Vector3(transform.localScale.x * 5f, transform.localScale.y * 5f, 1f);
	            lightCone.transform.localScale = new Vector3(0f, 0f, 1f);
	        }
	    }
	}

}
