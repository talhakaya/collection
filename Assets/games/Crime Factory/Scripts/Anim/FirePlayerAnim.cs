using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class FirePlayerAnim : AnimBehavior {
	    private const float PeriodFadeIn = 0.2f;
	    private const float PeriodFadeOut = 0.3f;
	    private float timerFadeIn;
	    private float timerFadeOut;
	    private PhysBox physBox;
	    public Sprite[] anim;
	    public float angle;
	    private float deathAnimVecAngle;
	    private float deathAnimVecLength;
	    private float deathAnimAngle;
	    private float visualAngleOffset;
	    private float scaleY;

	    private new void Start() {
	        sprites = anim;
	        period = 0.2f;
	        if (transform.localScale.y > 0f) scaleY = transform.localScale.y;
	        base.Start();
	    }

		void OnEnable () {
	        physBox = GetComponent<PhysBox>();
	        timerFadeIn = PeriodFadeIn;
	        timerFadeOut = 0f;
	        if (scaleY == 0f) scaleY = transform.localScale.y;
	        transform.localScale = new Vector3(0f, transform.localScale.y, 1f);
	        deathAnimVecAngle = Random.Range(90f, 270f);
	        deathAnimVecLength = Random.Range(2f, 4f);
	        deathAnimAngle = Random.Range(-720f, 720f);
	        physBox.visualPosOffset = new Vector2(0f, 0f);
	        visualAngleOffset = 0f;
	        physBox.vel = new Vector2(0f, 0f);
	    }

		private new void Update () {
	        base.Update();
	        if (Simulator.IsPaused) return;
	        physBox.vel = Geometry.createVector2(angle, physBox.MaxSpeedHor);
	        if (timerFadeIn > 0f) {
	            timerFadeIn -= Time.deltaTime;
	            if (timerFadeIn <= 0f) {
	                transform.localScale = new Vector3(scaleY, transform.localScale.y, 1f);
	            }
	            else {
	                float animRatio = 1f - (timerFadeIn / PeriodFadeIn);
	                transform.localScale = new Vector3(scaleY * animRatio, transform.localScale.y, 1f);
	            }
	        }
	        if (timerFadeOut > 0f) {
	            timerFadeOut -= Time.deltaTime;
	            if (timerFadeOut <= 0f) {
	                gameObject.SetActive(false);
	            }
	            else {
	                float animRatio = (timerFadeOut / PeriodFadeOut);
	                transform.localScale = new Vector3(scaleY * animRatio, transform.localScale.y, 1f);
	                visualAngleOffset += Time.deltaTime * deathAnimAngle;
	                physBox.visualPosOffset += Time.deltaTime * Geometry.createVector2(angle + deathAnimVecAngle, deathAnimVecLength);

	                transform.position = new Vector3(physBox.rect.x + physBox.visualPosOffset.x, physBox.rect.y + physBox.visualPosOffset.y, transform.position.z);
	            }
	        }
	        transform.localEulerAngles = new Vector3(0f, 0f, angle + visualAngleOffset);
	        physBox.enabled = (timerFadeOut <= 0f);
		}

	    public void Die() {
	        timerFadeOut = PeriodFadeOut;
	        AudioPlayer.instance.Play(AudioPlayer.instance.clipBulletHit);
	    }
	}
}
