using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public enum State {
        INACTIVE,
        AGGRO,
        DEAD
    }
    public State state;
    private float collisionTimer;
    private Rigidbody2D body;
    private bool turnRight;
    private LineRenderer lineRenderer;
    private Vector2 firePoint0;
    private Vector2 firePoint1;
    private float seeingTimer;
    private float seeingPeriod = 1f;
    private Vector3 lastSeenPlayerPos;

    void Start () {
        body = GetComponent<Rigidbody2D>();
        turnRight = Random.value > 0.5f;
        lineRenderer = GetComponent<LineRenderer>();
        transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
    }

    void Update () {
        collisionTimer += Game.dt;
        Vector3 dir = Geometry.normalizeVector3(Game.instance.player.transform.position - transform.position, 1f);
        firePoint0 = transform.position + dir * 0.51f;
        RaycastHit2D hit = Physics2D.Raycast(firePoint0, dir);
        bool seeingPlayer = state != State.DEAD && Game.instance.player.visible && hit.collider != null && hit.collider.CompareTag("Player");
        if (seeingPlayer) {
            lastSeenPlayerPos = Game.instance.player.transform.position;
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, firePoint0);
            if (hit.collider != null) {
                lineRenderer.SetPosition(1, hit.point);
            }
            else {
                lineRenderer.SetPosition(1, transform.position + dir * 1000f);
            }
        }
        else {
            lineRenderer.enabled = false;
        }

        if (state == State.INACTIVE) {
            if (seeingPlayer) {
                body.velocity = new Vector2(0f, 0f);
                transform.eulerAngles = new Vector3(0f, 0f, Geometry.angleOfVector3(lastSeenPlayerPos - transform.position));
                seeingTimer += Game.dt;
                if (seeingTimer >= seeingPeriod) {
                    state = State.AGGRO;
                    seeingTimer = 0f;
                }
                else {
                    Color c = new Color(1f, 0f, 0f, seeingTimer / seeingPeriod);
                    lineRenderer.startColor = c;
                    lineRenderer.endColor = c;
                }
            }
            else {
                if (Game.instance.player.noiseDistance > 0f && Geometry.lengthOfVector3(Game.instance.player.noisePoint - transform.position) <= Game.instance.player.noiseDistance) {
                    state = State.AGGRO;
                    lastSeenPlayerPos = Game.instance.player.noisePoint;
                    seeingTimer = 0f;
                }
                else {
                    seeingTimer = 0f;
                    body.velocity = Geometry.createVector3(transform.eulerAngles.z, 3f) * Game.timeSpeed;
                }
            }
        }
        else if (state == State.AGGRO) {
            Color c = new Color(1f, Random.value, Random.value);
            lineRenderer.startColor = c;
            lineRenderer.endColor = c;
            body.velocity = Geometry.createVector3(transform.eulerAngles.z, 8f) * Game.timeSpeed;
            if (Geometry.lengthOfVector3(lastSeenPlayerPos - transform.position) < 0.3f) {
                state = State.INACTIVE;
            }
            else {
                if (!seeingPlayer) {
                    seeingTimer -= 0.2f * Game.dt;
                    if (seeingTimer <= 0f) {
                        state = State.INACTIVE;
                    }
                }
                transform.eulerAngles = new Vector3(0f, 0f, Geometry.angleOfVector3(lastSeenPlayerPos - transform.position));
            }
        }
        else if (state == State.DEAD) {
            GetComponent<Collider2D>().enabled = false;
            body.velocity = new Vector2(0f, 0f);
            if (transform.localScale.x <= Game.dt) {
                Destroy(gameObject);
            }
            else {
                transform.localScale -= new Vector3(Game.dt, Game.dt, Game.dt);
                transform.eulerAngles += new Vector3(0f, 0f, (turnRight ? 1f : -1f) * 720f * Game.dt);
            }
        }
        body.angularVelocity = 0f;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (state == State.INACTIVE) {
            if (collisionTimer > 0.2f && other.gameObject.CompareTag("Wall")) {
                collisionTimer = 0f;
                transform.eulerAngles += new Vector3(0f, 0f, (turnRight ? 1f : -1f) * 30f);
            }
        }
        else if (state == State.AGGRO) {
            if (other.gameObject.CompareTag("Player")) {
                Game.failed = true;
            }
        }
    }

    void OnCollisionStay2D(Collision2D other) {
        OnCollisionEnter2D(other);
    }
}
