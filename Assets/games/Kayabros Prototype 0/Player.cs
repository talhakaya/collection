using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collection.Controls;

namespace Games.KayabrosPrototype0
{
	public class Player : MonoBehaviour {
	    private float angle;
	    private Rigidbody2D body;
	    private const float turnSpeed = 10f;
	    private float fireTimer;
	    private float firePeriod = 1f;
	    private Vector2 firePoint0;
	    private Vector2 firePoint1;
	    private LineRenderer lineRenderer;
	    public bool visible;
	    public float fireRatio;
	    public bool invisPressed;
	    private float visibleTimer;
	    private float visiblePeriod = 2f;
	    public Light visibleLight;
	    public CanvasGroup[] canvasGroups;
	    public int slot;
	    public int maxSlot = 2;
	    public float weaponTimer;
	    public float weaponPeriod = 1f;
	    public float noiseDistance;
	    public Vector3 noisePoint;
	    public GameObject drillerWall;

	    void Start () {
	        angle = 90f;
	        body = GetComponent<Rigidbody2D>();
	        lineRenderer = GetComponent<LineRenderer>();
	        fireTimer = firePeriod;
	        weaponTimer = 0f;
	        //visible = true;
	    }

		void Update () {
	        if (!Game.failed) {
	            if (noiseDistance > 0) {
	                noiseDistance = 0f;
	            }

	            if (visibleTimer > 0f) {
	                visibleTimer -= Game.dt;
	                if (visibleTimer <= 0f && invisPressed) {
	                    visible = false;
	                    invisPressed = false;
	                }
	            }
	            if (TaloketoInputManager.GetButtonDown("Fire2")) {
	                if (visibleTimer <= 0f) {
	                    visible = !visible;
	                }
	                else if (visible) {
	                    invisPressed = true;
	                }
	            }
	            drillerWall.SetActive(false);

	            Vector2 input = new Vector2(TaloketoInputManager.GetAxisRaw("Horizontal"), TaloketoInputManager.GetAxisRaw("Vertical"));
	            body.linearVelocity = Geometry.normalizeVector2(input, 3f * Game.timeSpeed);
	            angle = Geometry.angleOfVector3(MousePosition.get - transform.position);
	            transform.eulerAngles = new Vector3(0f, 0f, angle);
	            fireTimer += Game.dt;

	            if (TaloketoInputManager.GetButtonDown("Jump")) {
	                slot++;
	                if (slot > maxSlot) {
	                    slot = 0;
	                }
	                weaponTimer = weaponPeriod;
	            }
	            float weaponAlpha = 0f;
	            if (weaponTimer > 0) {
	                weaponAlpha = weaponTimer / weaponPeriod;
	                weaponTimer -= Game.dt;
	            }
	            for (int i = 0, len = canvasGroups.Length; i < len; i++) {
	                canvasGroups[i].alpha = (slot == i ? 1f : 0.4f) * (0.3f + weaponAlpha * 0.7f);
	            }

	            if (slot == 0) {
	                lineRenderer.enabled = fireTimer < firePeriod;
	            }
	            else if (slot == 1) {
	                if (fireTimer >= firePeriod && TaloketoInputManager.GetButtonDown("Fire1")) {
	                    noiseDistance = 10f;
	                    noisePoint = transform.position;
	                    visible = true;
	                    visibleTimer = visiblePeriod;
	                    fireTimer = 0f;
	                    Vector2 dir = Geometry.createVector3(angle, 1f);
	                    firePoint0 = transform.position;
	                    firePoint0 += dir * 0.51f;
	                    RaycastHit2D hit = Physics2D.Raycast(firePoint0, dir);
	                    if (hit.collider != null) {
	                        firePoint1 = hit.point;
	                        if (hit.collider.gameObject.CompareTag("Enemy")) {
	                            hit.collider.gameObject.GetComponent<Enemy>().state = Enemy.State.DEAD;
	                        }
	                    }
	                    else {
	                        firePoint1 = firePoint0 + dir * 1000f;
	                    }
	                    lineRenderer.SetPosition(0, firePoint0);
	                    lineRenderer.SetPosition(1, firePoint1);
	                }

	                if (fireTimer >= firePeriod) {
	                    Vector2 dir = Geometry.createVector3(angle, 1f);
	                    Vector2 aimPoint0 = transform.position;
	                    aimPoint0 += dir * 0.51f;
	                    lineRenderer.SetPosition(0, aimPoint0);
	                    RaycastHit2D hit = Physics2D.Raycast(aimPoint0, dir);
	                    if (hit.collider != null) {
	                        lineRenderer.SetPosition(1, hit.point);
	                    }
	                    else {
	                        lineRenderer.SetPosition(1, aimPoint0 + dir * 1000f);
	                    }
	                    Color c = new Color(0f, 1f, 0f, Random.Range(0f, 0.6f));
	                    lineRenderer.startColor = c;
	                    lineRenderer.endColor = c;
	                    lineRenderer.startWidth = 0.1f;
	                    lineRenderer.endWidth = 0.1f;
	                    fireRatio = 0f;
	                }
	                lineRenderer.enabled = true;
	            }
	            else if (slot == 2) {
	                lineRenderer.enabled = true;
	                if (fireTimer >= firePeriod) {
	                    Vector2 dir = Geometry.createVector3(angle, 1f);
	                    Vector2 aimPoint0 = transform.position;
	                    aimPoint0 += dir * 0.51f;
	                    lineRenderer.SetPosition(0, aimPoint0);
	                    Color c = new Color(1f, 1f, 1f);
	                    lineRenderer.startColor = c;
	                    lineRenderer.endColor = c;
	                    lineRenderer.startWidth = 0.8f;
	                    lineRenderer.endWidth = 0.1f;
	                    RaycastHit2D hit = Physics2D.Raycast(aimPoint0, dir);
	                    if (hit.collider != null && hit.collider.GetComponent<Wall>() != null && Geometry.lengthOfVector3(aimPoint0 - hit.point) < 2f && hit.collider.GetComponent<Wall>().Destroyable()) {
	                        lineRenderer.SetPosition(1, hit.point);

	                        if (TaloketoInputManager.GetButtonDown("Fire1")) {
	                            noiseDistance = 30f;
	                            noisePoint = transform.position;
	                            hit.collider.GetComponent<Wall>().Destroy();
	                            visible = true;
	                            fireTimer = 0f;
	                        }
	                        drillerWall.SetActive(true);
	                        drillerWall.transform.position = hit.collider.gameObject.transform.position;
	                        drillerWall.transform.localScale = new Vector3(hit.collider.gameObject.transform.localScale.x, hit.collider.gameObject.transform.localScale.y, drillerWall.transform.localScale.z);
	                    }
	                    else {
	                        lineRenderer.SetPosition(1, aimPoint0 + dir * 2f);
	                    }
	                }

	            }
	            else {
	                throw new System.NotImplementedException();
	            }

	            if (fireTimer < firePeriod) {
	                Color c = new Color(Random.value, Random.value, Random.value);
	                fireRatio = c.a = (firePeriod - fireTimer) / firePeriod;
	                lineRenderer.startColor = c;
	                lineRenderer.endColor = c;
	                lineRenderer.startWidth = 0.1f;
	                lineRenderer.endWidth = 0.1f;
	            }
	            if (visible) {
	                if (invisPressed) {
	                    visibleLight.color = new Color(
	                        1f,
	                        1f - Easing.QuadEaseOut(visibleTimer, 0f, 1f, visiblePeriod),
	                        1f);
	                }
	                else {
	                    visibleLight.color = new Color(1f, 0f, 1f);
	                }
	            }
	            else {
	                visibleLight.color = new Color(0f, 1f, 0f);
	            }
	        }
	        else {
	            body.linearVelocity = new Vector2(0f, 0f);
	        }
	        body.angularVelocity = 0f;
	    }
	}
}
