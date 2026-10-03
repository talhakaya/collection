using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class Guard : MonoBehaviour {
	    public enum State {
	        Idle,
	        Suspicious,
	        Attack
	    }
	    public State state;
	    [HideInInspector] public Person person;
	    private float timer;
	    private const float idlePeriod = 1f;
	    private const float suspiciousPeriod = 3f;
	    private const float attackPeriod = 0.3f;
	    private int direction;
	    private Vector2 suspiciousPos;
	    private Transform suspiciousTransform;
	    private float SeeDistance;
	    private static Color colorIdle = new Color(1f, 1f, 1f);
	    private static Color colorSuspicious = new Color(1f, 1f, 0f);
	    private static Color colorAttack = new Color(1f, 0f, 0f);
	    private static Color colorDead = new Color(0.5f, 0.5f, 0.5f);
	    public bool dontMove;
	    private bool dontMoveReal;

	    void Awake() {
	        person = GetComponent<Person>();
	    }

	    void Start() {
	        suspiciousTransform = new GameObject(name + "SuspiciousTransform").transform;
	        suspiciousTransform.SetParent(Game.currentLevel.transform);
	        dontMoveReal = dontMove;
	        SetSeeDistance();
	        bool found = false;
	        foreach (Guard g in Game.currentLevel.guards) {
	            if (g == this) {
	                found = true;
	                break;
	            }
	        }
	        if (!found) {
	            Debug.LogWarning("did you forget to put a guard in Level.guards?");
	        }
	    }

	    public void ResetState() {
	        state = State.Idle;
	        person.SetColor(colorIdle);
	        dontMove = dontMove || dontMoveReal;
	        SetSeeDistance();
	        person.isRight = false;
	    }

	    void Update() {
	        if (person.state == Person.State.Dead) {
	            person.SetColor(colorDead);
	            return;
	        }
	        CalculateCanSeePlayer();
	        timer += Game.dt;
	        if (state == State.Idle) {
	            if (timer >= idlePeriod) {
	                timer = 0f;
	                RandomDirection();
	            }
	            if (CanSeePlayer()) {
	                state = State.Suspicious;
	                timer = 0f;
	                person.SetColor(colorSuspicious);
	            }
	            if (dontMove) {
	                if (seeing == Seeing.Drone || seeing == Seeing.Bug) {
	                    dontMove = false;
	                    SetSeeDistance();
	                }
	                else {
	                    direction = 0;
	                }
	            }
	            person.SetInput(direction * 0.5f, 0f, false);
	        }
	        else if (state == State.Suspicious) {
	            Game.currentLevel.isStealthy = false;
	            if (timer >= attackPeriod) {
	                timer = 0f;
	                RandomDirection();
	                if (CanSeePlayer()) {
	                    state = State.Attack;
	                    timer = attackPeriod;
	                    person.SetColor(colorAttack);
	                }
	                else {
	                    state = State.Idle;
	                    timer = 0f;
	                    person.SetColor(colorIdle);
	                }
	            }
	            if (dontMove) {
	                direction = 0;
	            }
	            else if (transform.position.x < suspiciousPos.x) {
	                direction = 1;
	            }
	            else {
	                direction = -1;
	            }
	            suspiciousTransform.position = suspiciousPos;
	            person.SetInput(direction, 1f, true, false, false, 0, -1, suspiciousTransform);
	        }
	        else if (state == State.Attack) {
	            bool fire = false;
	            if (timer >= attackPeriod) {
	                timer = 0f;
	                fire = true;
	            }
	            if (!CanSeePlayer()) {
	                state = State.Suspicious;
	                timer = 0f;
	                person.SetColor(colorSuspicious);
	            }
	            suspiciousTransform.position = suspiciousPos;
	            person.SetInput(0f, 0f, true, fire, false, 0, -1, suspiciousTransform);
	        }
	        else {
	            throw new System.NotImplementedException();
	        }
	    }

	    public void Hear(Vector3 position) {
	        state = State.Suspicious;
	        timer = 0f;
	        person.SetColor(colorSuspicious);
	        suspiciousPos = position;
	        suspiciousTransform.position = suspiciousPos;
	    }

	    void RandomDirection() {
	        float r = Random.value * 3f;
	        if (r < 1f) {
	            direction = -1;
	        }
	        else if (r < 2f) {
	            direction = 0;
	        }
	        else {
	            direction = 1;
	        }
	    }

	    private Seeing seeing;
	    private enum Seeing { None, Player, Drone, Bug }
	    private bool CanSeePlayer() {
	        return seeing != Seeing.None;
	    }

	    private void CalculateCanSeePlayer() {
	        if (seeing != Seeing.None) {
	            string lastTag = "";
	            Transform t = null;
	            if (seeing == Seeing.Player) {
	                t = Player.instance.transform;
	                lastTag = "Player";
	            }
	            else if (seeing == Seeing.Drone) {
	                t = Drone.instance.transform;
	                lastTag = "Drone";
	            }
	            else if (seeing == Seeing.Bug) {
	                t = Bug.instance.transform;
	                lastTag = "Bug";
	            }
	            else {
	                throw new System.NotImplementedException();
	            }
	            RaycastHit2D hit = Physics2D.Raycast(person.eyes.position, t.position - transform.position);
	            if (hit.collider != null && hit.collider.gameObject.CompareTag(lastTag) && Geometry.lengthOfVector3(new Vector3(hit.point.x, hit.point.y, 0f) - person.eyes.position) < SeeDistance) {
	                suspiciousPos = hit.point;
	                return;
	            }
	            hit = Physics2D.Raycast(person.eyes.position, suspiciousTransform.position - transform.position);
	            if (hit.collider != null && hit.collider.gameObject.CompareTag(lastTag) && Geometry.lengthOfVector3(new Vector3(hit.point.x, hit.point.y, 0f) - person.eyes.position) < SeeDistance) {
	                suspiciousPos = hit.point;
	                return;
	            }
	        }
	        if (person.isRight == (transform.position.x < Player.instance.transform.position.x)) {
	            RaycastHit2D hit = Physics2D.Raycast(person.eyes.position, Player.instance.transform.position - transform.position);
	            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player") && Geometry.lengthOfVector3(new Vector3(hit.point.x, hit.point.y, 0f) - person.eyes.position) < SeeDistance) {
	                suspiciousPos = hit.point;
	                seeing = Seeing.Player;
	                return;
	            }
	        }
	        if (Drone.instance != null && Drone.instance.gameObject.activeSelf && person.isRight == (transform.position.x < Drone.instance.transform.position.x)) {
	            RaycastHit2D hit = Physics2D.Raycast(person.eyes.position, Drone.instance.transform.position - transform.position);
	            if (hit.collider != null && hit.collider.gameObject.CompareTag("Drone") && Geometry.lengthOfVector3(new Vector3(hit.point.x, hit.point.y, 0f) - person.eyes.position) < SeeDistance) {
	                suspiciousPos = hit.point;
	                seeing = Seeing.Drone;
	                return;
	            }
	        }
	        if (Bug.instance != null && Bug.instance.gameObject.activeSelf && person.isRight == (transform.position.x < Bug.instance.transform.position.x)) {
	            RaycastHit2D hit = Physics2D.Raycast(person.eyes.position, Bug.instance.transform.position - transform.position);
	            if (hit.collider != null && hit.collider.gameObject.CompareTag("Bug") && Geometry.lengthOfVector3(new Vector3(hit.point.x, hit.point.y, 0f) - person.eyes.position) < SeeDistance) {
	                suspiciousPos = hit.point;
	                seeing = Seeing.Bug;
	                return;
	            }
	        }
	        seeing = Seeing.None;
	    }

	    void SetSeeDistance() {
	        SeeDistance = 15f * (dontMove ? 1.5f : 1f);
	    }
	}
}
