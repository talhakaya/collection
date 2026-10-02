using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Person : MonoBehaviour {
    [SerializeField] private Collider2D[] colAlive;
    [SerializeField] private Collider2D[] colDead;
    public Tilt[] tilts;
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private Transform[] onGroundChecks;
    [SerializeField] private Transform[] leftChecks;
    [SerializeField] private Transform[] rightChecks;
    public Transform eyes;
    public Transform bodyTransform;
    private Rigidbody2D body;
    public bool onGround;
    public bool leftTouch;
    public bool rightTouch;
    public bool isRight;
    public float speedH;
    public float maxSpeedV;
    private float speedV;
    public float jumpForceFirst;
    public float jumpForceCont;
    [HideInInspector] public Vector2 input;
    [HideInInspector] public bool interactInput;
    [HideInInspector] public bool fireInput;
    [HideInInspector] public bool fire2Input;
    [HideInInspector] public float weaponSelect;
    [HideInInspector] public int newWeaponId;
    [HideInInspector] public Transform aimTarget;
    [HideInInspector] public Vector2 force;
    [HideInInspector] public float forceTimer;
    public GameObject[] tools;
    public int toolIndex;
    public WeaponSelect weaponUi;
    [HideInInspector] public Transform aimAt;
    [HideInInspector] public Transform camAt;
    private float weaponSelectOld;
    private Player player;
    [HideInInspector] public float normalJumpForceCont;
    public Vector3 defGadgetPos;
    public float defGadgetAngle;
    public enum State {
        Alive,
        Dead,
    }
    private State _state;
    public State state {
        get { return _state; }
        set {
            if (!inited) {
                inited = true;
                Init();
            }
            foreach (Collider2D c in colAlive) {
                c.enabled = value == State.Alive;
            }
            foreach (Collider2D c in colDead) {
                c.enabled = value == State.Dead;
            }
            switch (value) {
                case State.Alive:
                    body.constraints = RigidbodyConstraints2D.FreezeRotation;
                    transform.eulerAngles = new Vector3(0f, 0f, 0f);
                    break;
                case State.Dead:
                    body.constraints = RigidbodyConstraints2D.None;
                    float angularVel = (Random.value > 0.5f ? (Random.value + 1f) : -(Random.value + 1f)) * 100f;
                    body.angularVelocity = angularVel;
                    body.AddForceAtPosition(new Vector2(angularVel, 10f), transform.position + new Vector3(0f, 1f, 0f));
                    tilts[1].aimAt = null;
                    tilts[2].aimAt = null;
                break;
            }

            _state = value;
        }
    }
    public bool canDie;
    private bool inited;
    public bool createNoise;

    void Start() {
        if (!inited) {
            inited = true;
            Init();
        }
    }

    void Init () {
        body = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        normalJumpForceCont = jumpForceCont;
        ResetState();
    }

    public void ResetState() {
        if (!inited) {
            inited = true;
            Init();
        }
        state = State.Alive;
        camAt = transform;

        toolIndex = 0;
        SelectWeapon();
        for (int i = 0, len = tools.Length; i < len; i++) {
            if (tools[i] == null) {

            }
            else if (tools[i].GetComponent<Remote>() != null) {
                tools[i].GetComponent<Remote>().ResetState();
            }
        }
        
        if (weaponUi != null) {
            weaponUi.ResetState();
        }
    }

    void Update() {
        camAt = transform;

        if (state == State.Alive) {
            if (tools.Length <= toolIndex || tools[toolIndex] == null) {
                if (fireInput) {
                    aimAt = aimTarget;
                }
                else {
                    aimAt = null;
                }
            }
            else if (tools[toolIndex].GetComponent<Remote>() != null) {
                Remote remote = tools[toolIndex].GetComponent<Remote>();
                if (fireInput && !remote.gadget.wearable) {
                    remote.SetInput(this, input.x, input.y, interactInput, fireInput);
                    input = new Vector2(0f, 0f);
                    if (remote.isGadgetOut && !remote.gadget.wearable) {
                        camAt = aimAt = remote.gadget.transform;
                    }
                    else {
                        aimAt = aimTarget;
                    }
                }
                else {
                    remote.SetInput(this, 0f, 0f, interactInput, fireInput);
                    aimAt = null;
                }
            }
            else if (tools[toolIndex].GetComponent<Gun>() != null) {
                tools[toolIndex].GetComponent<Gun>().SetInput(this, fireInput, fire2Input);
                aimAt = aimTarget;
            }

            bool activeHandRight = aimAt != null && (isRight == aimAt.position.x > transform.position.x);
            if (activeHandRight) {
                tilts[1].aimAt = aimAt;
                tilts[2].aimAt = null;
            }
            else {
                tilts[1].aimAt = null;
                tilts[2].aimAt = aimAt;
            }
            if (aimAt != null && tools.Length > toolIndex && tools[toolIndex] != null && activeHandRight != (tools[toolIndex].transform.parent == tilts[1].transform)) {
                Vector3 remoteLocalPos = tools[toolIndex].transform.localPosition;
                Vector3 remoteLocalEul = tools[toolIndex].transform.localEulerAngles;
                tools[toolIndex].transform.parent = activeHandRight ? tilts[1].transform : tilts[2].transform;
                tools[toolIndex].transform.localPosition = remoteLocalPos;
                tools[toolIndex].transform.localEulerAngles = remoteLocalEul;
                tools[toolIndex].transform.localScale = new Vector3(1f, 1f, 1f);
            }

            if (weaponSelectOld == 0f) {
                if (weaponSelect > 0) {
                    toolIndex++;
                    toolIndex = toolIndex % tools.Length;
                    SelectWeapon();
                }
                else if (weaponSelect < 0) {
                    toolIndex--;
                    if (toolIndex < 0) toolIndex += tools.Length;
                    SelectWeapon();
                }
            }

            if (newWeaponId >= 0 && newWeaponId < tools.Length) {
                toolIndex = newWeaponId;
                SelectWeapon();
            }

            if (fire2Input) {
                GetGadgetBack();
            }

            weaponSelectOld = weaponSelect;

            //ACTUAL PHYSICS
            bool onGroundOld = onGround;
            onGround = false;
            RaycastHit2D hitGround = Physics2D.Raycast(onGroundChecks[0].position, onGroundChecks[1].position - onGroundChecks[0].position);
            if (hitGround.collider != null && !hitGround.collider.isTrigger) {
                Vector3 hitPointLocal = transform.InverseTransformPoint(hitGround.point);
                onGround = (hitPointLocal.x >= onGroundChecks[0].localPosition.x && hitPointLocal.x <= onGroundChecks[1].localPosition.x);
                if (onGround && !onGroundOld) {
                    //if (this == Player.person) {
                    //    ObjectPool.noisePool.get(new Vector3(hitGround.point.x, hitGround.point.y, 2f), new Vector3(1f, 1f, 1f) * Noise.JumpHitGround);
                    //}
                }
                if (onGround) {
                    Level.SetLatestGroundY(hitGround.point.y);
                }
            }
            leftTouch = false;
            RaycastHit2D hitLeft = Physics2D.Raycast(leftChecks[0].position, leftChecks[1].position - leftChecks[0].position);
            if (hitLeft.collider != null && !hitLeft.collider.isTrigger && !Game.CanWalkInto(hitLeft.collider.gameObject)) {// && hitLeft.collider.CompareTag("Wall")) {
                Vector3 hitPointLocal = transform.InverseTransformPoint(hitLeft.point);
                leftTouch = (hitPointLocal.y >= leftChecks[0].localPosition.y && hitPointLocal.y <= leftChecks[1].localPosition.y);
            }
            rightTouch = false;
            RaycastHit2D hitRight = Physics2D.Raycast(rightChecks[0].position, rightChecks[1].position - rightChecks[0].position);
            if (hitRight.collider != null && !hitRight.collider.isTrigger && !Game.CanWalkInto(hitRight.collider.gameObject)) {// && hitRight.collider.CompareTag("Wall")) {
                Vector3 hitPointLocal = transform.InverseTransformPoint(hitRight.point);
                rightTouch = (hitPointLocal.y >= rightChecks[0].localPosition.y && hitPointLocal.y <= rightChecks[1].localPosition.y);
            }

            Vector2 forceDt = force * Game.dt * forceTimer / JumpPanel.forcePeriod;
            if (forceTimer > 0f) {
                forceTimer -= Game.dt;
                if (forceTimer < 0f) {
                    forceTimer = 0f;
                }
                //SetInput(0f, 0f, false);
            }

            speedV = body.velocity.y + forceDt.y;
            speedV -= Game.dtPhysics * 19.87f;

            if (input.y > 0f) {
                if (onGround) {
                    speedV = jumpForceFirst;
                }
                else {
                    speedV += jumpForceCont * Game.dtPhysics;
                }
            }

            float bodyVelY = Mathf.Clamp(speedV, -maxSpeedV, maxSpeedV);
            if ((input.x > 0f && rightTouch) || (input.x < 0f && leftTouch)) {
                body.velocity = new Vector2(0f, bodyVelY);
            }
            else {
                if (input.x == 0f) {
                    if (forceDt.x != 0f) {
                        body.velocity = new Vector2(body.velocity.x + forceDt.x, bodyVelY);
                    }
                    else {
                        if (body.velocity.x > 0f) {
                            body.velocity = new Vector2(Mathf.Max(0f, body.velocity.x - speedH * Game.dt * 4f), bodyVelY);
                        }
                        else {
                            body.velocity = new Vector2(Mathf.Min(0f, body.velocity.x + speedH * Game.dt * 4f), bodyVelY);
                        }

                    }
                }
                else {
                    body.velocity = new Vector2(Mathf.Clamp(body.velocity.x + input.x * speedH * Game.dt * 4f, -speedH, speedH) + forceDt.x, bodyVelY);
                }
            }

            if (input.x < 0f) {
                isRight = false;
            }
            else if (input.x > 0f) {
                isRight = true;
            }

            if (isRight) {
                bodyTransform.localScale = new Vector3(Mathf.Abs(bodyTransform.localScale.x), bodyTransform.localScale.y, bodyTransform.localScale.z);
            }
            else {
                bodyTransform.localScale = new Vector3(-Mathf.Abs(bodyTransform.localScale.x), bodyTransform.localScale.y, bodyTransform.localScale.z);
            }

            bool moving = body.velocity.x != 0f;
            foreach (Tilt t in tilts) {
                t.active = moving;
            }

            input = new Vector2(0f, 0f);
            interactInput = false;
        }
        else {
            foreach (Tilt t in tilts) {
                t.active = false;
            }
        }
    }

    public void GetShot(RaycastHit2D hit) {
        if (canDie) {
            ObjectPool.bloodPool.Create(new Vector3(hit.point.x, hit.point.y, -1));
            Die();
        }
    }

    public void SetInput(float horizontalInput, float verticalInput, bool interactInput, bool fireInput = false, bool fire2Input = false, float weaponSelect = 0f, int newWeaponId = -1, Transform aimTarget = null) {
        input = new Vector2(horizontalInput, verticalInput);
        this.interactInput = interactInput;
        this.interactInput = interactInput;
        this.fireInput = fireInput;
        this.fire2Input = fire2Input;
        this.weaponSelect = weaponSelect;
        this.newWeaponId = newWeaponId;
        this.aimTarget = aimTarget;
    }

    public void SetForce(Vector2 force) {
        this.force = force;
        forceTimer = JumpPanel.forcePeriod;
    }

    public bool IsUsingGadget() {
        return tools.Length > toolIndex && tools[toolIndex] != null && tools[toolIndex].GetComponent<Remote>() != null;
    }

    public Gadget GetGadget() {
        if (!IsUsingGadget()) return null;
        return tools[toolIndex].GetComponent<Remote>().gadget;
    }

    public Remote GetRemote() {
        if (!IsUsingGadget()) return null;
        return tools[toolIndex].GetComponent<Remote>();
    }

    public bool IsUsingGun() {
        return tools.Length > toolIndex && tools[toolIndex] != null && tools[toolIndex].GetComponent<Gun>();
    }

    public Gun GetGun() {
        if (!IsUsingGun()) return null;
        return tools[toolIndex].GetComponent<Gun>();
    }

    void SelectWeapon() {
        if (tools.Length >= toolIndex) {
            for (int i = 0, len = tools.Length; i < len; i++) {
                if (tools[i] != null) {
                    tools[i].SetActive(i == toolIndex);
                }
            }
        }

        if (weaponUi != null) {
            weaponUi.weaponUi.alpha = 1f;
            for (int i = 0, len = tools.Length; i < len; i++) {
                if (i == toolIndex) {
                    weaponUi.weaponCanvasGroups[i].alpha = 1f;
                }
                else {
                    weaponUi.weaponCanvasGroups[i].alpha = WeaponSelect.WeaponAlpha;
                }
            }
        }
    }

    void GetGadgetBack() {
        bool found = false;
        if (tools[toolIndex] != null && tools[toolIndex].GetComponent<Remote>() != null) {
            Remote remote = tools[toolIndex].GetComponent<Remote>();
            if (remote.isGadgetOut && Geometry.lengthOfVector3(remote.transform.position - remote.gadget.transform.position) < 2f) {
                remote.ResetState();
                found = true;
            }
        }
        if (!found) {
            for (int i = 0, len = tools.Length; i < len; i++) {
                if (tools[i] != null && tools[i].GetComponent<Remote>() != null) {
                    Remote remote = tools[i].GetComponent<Remote>();
                    if (remote.isGadgetOut && Geometry.lengthOfVector3(remote.transform.position - remote.gadget.transform.position) < 2f) {
                        tools[i].GetComponent<Remote>().ResetState();
                        found = true;
                        break;
                    }
                }
            }
        }
    }

    private Color setC;
    public void SetColor(Color c) {
        if (c.r != setC.r || c.g != setC.g || c.b != setC.b) {
            setC = c;
            for (int i = 0, len = spriteRenderers.Length; i < len; i++) {
                spriteRenderers[i].color = setC;
            }
        }
    }

    public void Die() {
        state = State.Dead;
    }

    public void Undie() {
        state = State.Alive;
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }
}
