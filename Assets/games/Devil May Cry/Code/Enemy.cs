using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.DevilMayCry
{
    public class Enemy : Actor
    {
        public enum Mood
        {
            Passive,
            Aggressive
        }

        public Mood mood;

        [HideInInspector] public int animAttack;

        [SerializeField] private float statePassivePeriod;
        [SerializeField] private float moveSpeed;
        [SerializeField] private string animAttackString = "Attack";

        private float attackTriggerTimer;
        private float stateTimer;
        private float statePeriod;
        private bool isWalkDirRight;

        protected override void Awake()
        {
            base.Awake();
            animAttack = Animator.StringToHash(animAttackString);
            mood = Mood.Passive;
            SetState(State.Idle);
            attackTriggerTimer = 1000f;
        }

        public void UpdateDt(float dt, PhysicsParams physicsParams)
        {
            if (isDead) return;

            Vector2 velocity = body.linearVelocity;

            switch (mood)
            {
                case Mood.Passive:
                    {
                        if (moveSpeed == 0f) break;
                        if (stateTimer >= statePeriod)
                        {
                            stateTimer = 0f;
                            statePeriod = UnityEngine.Random.value * statePassivePeriod;
                            if (UnityEngine.Random.value < 0.5f)
                            {
                                SetState(State.Walk);
                                isWalkDirRight = UnityEngine.Random.value < 0.5f;
                                transform.localScale = new Vector3((isWalkDirRight ? 1f : -1f) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                            }
                            else
                            {
                                SetState(State.Idle);
                            }
                        }
                        else
                        {
                            stateTimer += dt;
                            if (state == State.Walk)
                            {
                                velocity = new Vector2((isWalkDirRight ? 1f : -1f) * moveSpeed, velocity.y);
                            }
                            else
                            {
                                velocity = new Vector2(0f, velocity.y);
                            }
                        }
                        break;
                    }
                case Mood.Aggressive:
                    {
                        attackTriggerTimer += dt;
                        if (state == State.Idle || state == State.Walk)
                        {
                            if (attackTriggerTimer < 0.2f)
                            {
                                SetState(State.Attack0);
                            }
                            else if (state == State.Idle)
                            {
                                SetState(State.Walk);
                            }
                            else
                            {
                                if (Main.inst.player != null)
                                {
                                    isWalkDirRight = Main.inst.player.trans.position.x > transform.position.x;
                                    transform.localScale = new Vector3((isWalkDirRight ? 1f : -1f) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                                    velocity = new Vector2((isWalkDirRight ? 1f : -1f) * moveSpeed, velocity.y);
                                }
                            }
                        }
                        else
                        {
                            velocity = new Vector2(0f, velocity.y);
                        }

                        break;
                    }
            }

            //gravity
            velocity += new Vector2(0f, physicsParams.gravityAcceleration * dt);

            //friction
            velocity = new Vector2(Mathf.Sign(velocity.x) * Mathf.Max(0f, Mathf.Abs(velocity.x) - physicsParams.horFriction * dt), velocity.y);

            //max speed
            velocity = new Vector2(Mathf.Clamp(velocity.x, -physicsParams.velocityMax.x, physicsParams.velocityMax.x), Mathf.Clamp(velocity.y, -physicsParams.velocityMax.y, physicsParams.velocityMax.y));

            body.linearVelocity = velocity;
        }

        public void OnAttackTrigger()
        {
            attackTriggerTimer = 0f;
            if (mood == Mood.Passive) mood = Mood.Aggressive;
        }

        public override void GetHitBy(Actor other, float id, Vector3 hitPosition)
        {
            base.GetHitBy(other, id, hitPosition);
            mood = Mood.Aggressive;
        }
    }
}
