using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    public enum State {
        Closed,
        Open
    }
    public State state;
    private bool openToRight;
    private float timer;
    private const float period = 0.5f;
    private const float beginScale = 0.25f;
    public bool canBeTriggered;
    [SerializeField] private Collider2D trigger;
    [SerializeField] private Collider2D collider;

    void Start () {
        ResetState();
	}

    public void ResetState() {
        timer = 0f;
        state = State.Closed;
        transform.localScale = new Vector3(beginScale * transform.localScale.y, transform.localScale.y, transform.localScale.z);
        trigger.enabled = collider.enabled = true;
    }
	
	void Update () {
		if (state == State.Closed) {
            if (timer > 0f) {
                timer -= Game.dt;
                if (timer <= 0f) {
                    timer = 0f;
                }
            }
        }
        else if (state == State.Open) {
            if (timer < period) {
                timer += Game.dt;
                if (timer >= period) {
                    timer = period;
                }
            }
        }
        else {
            throw new System.NotImplementedException();
        }
        collider.enabled = (state == State.Closed);
        trigger.enabled = timer < period * 0.5f;
        transform.localScale = new Vector3(Easing.Linear(timer, beginScale, (openToRight ? 1f : -1f) - beginScale, period) * transform.localScale.y, transform.localScale.y, transform.localScale.z);
        GetComponent<SpriteRenderer>().color = canBeTriggered ? Color.white : new Color(1f, 0.5f, 0.5f);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (canBeTriggered && state == State.Closed) {
            bool isPlayer = other.CompareTag("Player");
            if (isPlayer || (other.CompareTag("Enemy") && (other.GetComponent<Person>().state == Person.State.Dead || other.GetComponent<Guard>().state != Guard.State.Idle))) {
                if (isPlayer) {
                    Noise.Create(transform.TransformPoint(new Vector3(0f, 1f, 2f)), new Vector3(1f, 1f, 1f) * Noise.BulletHit);
                }
                Open();
                openToRight = other.transform.position.x < transform.position.x;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        OnTriggerEnter2D(other);
    }

    public void Open() {
        state = State.Open;
    }

    public void Close() {
        state = State.Closed;
    }
}
