using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    private bool fireInput;
    private bool fire2Input;
    private float timer;
    public float period = 1f;
    public float kickBackPeriod = 0.03f;
    private LineRenderer lineRenderer;
    [SerializeField] private Transform firePoint;
    [HideInInspector] public float kickBackTimer;
    [HideInInspector] public float kickBackAngle;
    public Sprite icon;
    private Person person;

    void Start () {
        timer = period;
        lineRenderer = GetComponent<LineRenderer>();
    }
	
	void Update () {
        if (person.state != Person.State.Dead && timer >= period) {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, firePoint.position);
            float scaleAngle = transform.lossyScale.x > 0f ? 0f : 180f;
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, Geometry.createVector2(firePoint.eulerAngles.z + scaleAngle, 1f));
            if (hit.collider != null) {
                lineRenderer.SetPosition(1, hit.point);
            }
            else {
                lineRenderer.SetPosition(1, firePoint.position + Geometry.createVector3(firePoint.eulerAngles.z + scaleAngle, 1000f));
            }
            if (fireInput) {
                timer = 0f;
                kickBackTimer = kickBackPeriod;
                kickBackAngle = firePoint.eulerAngles.z + scaleAngle - 180f;
                if (hit.collider != null) {
                    GameObject hitGo = hit.collider.gameObject;
                    if (hitGo.GetComponent<Gadget>() != null) {
                        hitGo.GetComponent<Gadget>().GetShot(hit);
                    }
                    else if (hitGo.GetComponent<Person>() != null) {
                        hitGo.GetComponent<Person>().GetShot(hit);
                    }
                    else if (hitGo.GetComponent<Player>() != null) {
                        hitGo.GetComponent<Player>().GetShot(hit);
                    }
                    else {
                        ObjectPool.smokePool.Create(new Vector3(hit.point.x, hit.point.y, -1));
                    }

                    if (hitGo.GetComponent<Rigidbody2D>() != null) {
                        hitGo.GetComponent<Rigidbody2D>().AddForceAtPosition(Geometry.createVector3(firePoint.eulerAngles.z + scaleAngle, 1000f), hit.point);
                    }
                    if (person == Player.person) {
                        Noise.Create(new Vector3(hit.point.x, hit.point.y, 2f), new Vector3(1f, 1f, 1f) * Noise.BulletHit);
                    }
                }

                if (person == Player.person) {
                    Noise.Create(new Vector3(firePoint.position.x, firePoint.position.y, 2f), new Vector3(1f, 1f, 1f) * Noise.Gun);
                }
            }
        }
        else {
            lineRenderer.enabled = false;
            timer += Game.dt;
        }
        
        SetInput(person, false, false);
	}

    public void SetInput(Person person, bool fireInput, bool fire2Input) {
        this.person = person;
        this.fireInput = fireInput;
        this.fire2Input = fire2Input;
    }
}
