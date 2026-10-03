using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.DevilMayCry
{
    public class Actor : MonoBehaviour
    {
        public enum State
        {
            None,
            Idle,
            Walk,
            Jump,
            Attack0,
            Attack1,
            Dodge,
            Squirt0,
            Squirt1,
            Hit,
            Die
        }
        public State state;
        public float defaultDamage;
        public float hp;
        public bool isDead { get { return hp <= 0f; } }

        [HideInInspector] public Animator animator;
        [HideInInspector] public float dodgeTime;

        private List<float> oldHitIds = new List<float>();
        protected Rigidbody2D body;
        protected BoxCollider2D boxCollider;
        protected SpriteRenderer spriteRenderer;

        private const float damageTintPeriod = 0.2f;
        private float damageTintTime;

        private const float glowPeriod = 0.5f;
        private float glowTime;

        protected virtual void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
            SetState(State.Idle);
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void Update()
        {
            if (damageTintTime > 0f)
            {
                damageTintTime = Mathf.Clamp(damageTintTime - Time.deltaTime, 0f, damageTintPeriod);
                spriteRenderer.materials[0].SetFloat("_TintAmount", damageTintTime / damageTintPeriod);
            }
            if (glowTime > 0f)
            {
                glowTime = Mathf.Clamp(glowTime - Time.deltaTime, 0f, glowPeriod);
                spriteRenderer.materials[0].SetFloat("_GlowAmount", glowTime / glowPeriod);
            }
        }

        public virtual void GetHitBy(Actor other, float id, Vector3 hitPosition)
        {
            if (isDead || other.isDead || dodgeTime > 0f) return;
            if (oldHitIds.Contains(id)) return;

            oldHitIds.Add(id);

            damageTintTime = damageTintPeriod;
            float damage = other.GetDamage();
            hp -= damage;
            if (isDead)
            {
                Main.inst.OnDie(this);
                SetState(State.Die);
            }
            else
            {
                Main.inst.OnHit(this);
                SetState(State.Hit);
            }

            GameObject hitParticle = Pool.HitParticle.Get();
            hitParticle.transform.position = hitPosition;
            float scale = UnityEngine.Random.Range(3f, 7f);
            hitParticle.transform.localScale = new Vector3(Mathf.Sign(other.transform.localScale.x) * scale, scale, 1f);
            GameObject splashParticle = Pool.SplashParticle.Get();
            splashParticle.transform.position = hitPosition;
            scale = UnityEngine.Random.Range(3f, 7f);
            splashParticle.transform.localScale = new Vector3(scale, scale, 1f);
        }

        protected void SetState(State state)
        {
            this.state = state;
            animator.CrossFade($"{state}", 0.1f, -1, 0f);
        }

        public virtual float GetDamage()
        {
            return defaultDamage;
        }

        public void Glow()
        {
            glowTime = glowPeriod;
        }
    }
}
