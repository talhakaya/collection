using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    public class Actor : MonoBehaviour
    {
        public Transform[] joints;
        public float speedStanding = 10f;
        public float speedCrouch = 5f;
        public float jumpSpeed;
        public LayerMask levelGeoLayer;
        public Weapon weapon;
        public float hp = 5;

        [HideInInspector] public float hurtTimer;
        [HideInInspector] public float stunTimer;

        private Rigidbody body;
        private Animator animator;
        private string lastAnim;
        private Renderer[] renderers;
        private const float hurtPeriod = 0.5f;
        private const float stunPeriod = 0.2f;
        private bool isPlayer;

        private float hpMax;
        private HealthBar healthBar;

        private void Start()
        {
            animator = GetComponent<Animator>();
            body = GetComponent<Rigidbody>();
            isPlayer = GetComponent<PlayerControl>() != null;
            if (!isPlayer) healthBar = Pools.Get(PoolType.HealthBar).GetComponent<HealthBar>();
            hpMax = hp;
        }

        private void OnDestroy()
        {
            if (healthBar != null) healthBar.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (hurtTimer > 0f)
            {
                hurtTimer -= Time.deltaTime;
                float tintAmount = Mathf.Clamp01(hurtTimer / hurtPeriod);
                foreach (Renderer r in GetRenderers())
                {
                    r.material.SetFloat("_OverrideAmount", tintAmount);
                }
            }
            if (stunTimer > 0f)
            {
                stunTimer -= Time.deltaTime;
            }
        }

        private void LateUpdate()
        {
            if (healthBar != null)
            {
                healthBar.SetWorldPos(transform.position + transform.up * 5f);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (Transform j in joints)
            {
                Gizmos.DrawSphere(j.position, 0.1f);
            }
        }

        public void Move(Vector3 inputMove, bool inputFire, bool inputAim, bool inputCrouch, Vector3 lookAt)
        {
            Vector3 vel = Vector3.zero;
            bool isAiming = weapon != null && (inputFire || inputAim);
            bool onGround = FloorCast() != null;
            if (stunTimer > 0f)
            {
                PlayAnim("Stun", stunPeriod);
            }
            else if (onGround)
            {
                string animAim = isAiming ? "Aim" : "";
                if (inputMove.x == 0f && inputMove.z == 0f)
                {
                    PlayAnim((inputCrouch ? "Crouch" : "Idle") + animAim);
                    animator.speed = 1f;
                }
                else
                {
                    PlayAnim((inputCrouch ? "CrouchRun" : "Run") + animAim);
                    if (inputMove.magnitude >= 1f) inputMove = inputMove.normalized;
                    animator.speed = inputMove.magnitude;
                    vel = (transform.forward * inputMove.z + transform.right * inputMove.x) * (inputCrouch || inputMove.z <= 0f ? speedCrouch : speedStanding);
                }
                if (inputMove.y > 0f) body.velocity = new Vector3(body.velocity.x, jumpSpeed, body.velocity.z);
                if (weapon != null && inputFire) weapon.TryFire(lookAt, isPlayer);
            }
            else
            {
                animator.speed = 1f;
                PlayAnim("Jump");
                vel = body.velocity;
            }
            body.velocity = new Vector3(vel.x, body.velocity.y, vel.z);
            body.angularVelocity = Vector3.zero;
            if (inputMove.x != 0f || inputMove.z != 0f || inputFire || inputAim)
            {
                transform.LookAt(lookAt);
                transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
            }
        }

        private Transform FloorCast()
        {
            bool cast = Physics.BoxCast(transform.position + new Vector3(0f, 0.3f, 0f), new Vector3(0.5f, 0.1f, 0.5f), Vector3.down, out RaycastHit hitInfo, transform.rotation, 0.21f, levelGeoLayer, QueryTriggerInteraction.Ignore);
            return cast ? hitInfo.transform : null;
        }

        private void PlayAnim(string anim, float crossFade = 0.1f)
        {
            if (lastAnim == anim) return;
            lastAnim = anim;
            animator.CrossFade(anim, crossFade);
        }

        public bool IsCrouching()
        {
            return lastAnim.Contains("Crouch");
        }

        public Renderer[] GetRenderers()
        {
            if (renderers == null) renderers = GetComponentsInChildren<Renderer>();
            return renderers;
        }

        public void GetHit(BodyPart bodyPart)
        {
            hurtTimer = hurtPeriod;
            if (!isPlayer) stunTimer = stunPeriod;
            switch (bodyPart.part)
            {
                case BodyPart.Part.Default:
                    hp--;
                    break;
                case BodyPart.Part.Limb:
                    hp -= 0.5f;
                    break;
                case BodyPart.Part.Head:
                    hp -= 3;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            if (hp <= 0)
            {
                DeadHandler.inst.Show(this);
                gameObject.SetActive(false);
                if (healthBar != null) healthBar.gameObject.SetActive(false);
            }
            else if (healthBar != null)
            {
                healthBar.SetHealth(Mathf.Clamp01(hp / hpMax));
            }
        }

        public Vector3 GetPosToBeAimed()
        {
            return transform.position + transform.up * 3f;
        }
    }
}
