using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    public class Character : Actor
    {
        public Transform trans;
        public Transform squirtTrans;
        public Transform lineHor;
        public Transform lineVer;
        public Animator doubleJumpAnim;
        public AnimationCurve curveBgLineX;
        public AnimationCurve curveBgLineY;

        [HideInInspector] public float attackTime;
        [HideInInspector] public int numJumps;
        [HideInInspector] public int animMoveHor;
        [HideInInspector] public int animMoveVer;

        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private AttackPattern[] attackPatterns;
        [SerializeField] private bool onGroundOld;

        private AttackPattern currentAttackPattern;
        private AttackPattern.Attack currentAttack;
        private float bgLineTimer;
        private Vector2 dodgeDir;

        public void UpdateDt(float dt, InputMan input, PhysicsParams physicsParams)
        {
            if (isDead) return;

            bool onGround = OnGround();

            if (input.hor != 0f)
            {
                trans.localScale = new Vector3(Mathf.Sign(input.hor) * Mathf.Abs(trans.localScale.x), trans.localScale.y, trans.localScale.z);
            }

            //reset jumps
            if (onGround) numJumps = 0;

            Vector2 velocity = body.velocity;

            //cancel y velocity
            if (dodgeTime <= 0f && onGround)
            {
                velocity = new Vector2(velocity.x, 0f);
            }

            //animation
            if ((onGround && input.horDown) || (onGround && !onGroundOld))
            {
                SetState(input.hor == 0f ? State.Idle : State.Walk);
            }

            //gravity
            velocity += new Vector2(0f, physicsParams.gravityAcceleration * dt);

            //friction
            velocity = new Vector2(Mathf.Sign(velocity.x) * Mathf.Max(0f, Mathf.Abs(velocity.x) - physicsParams.horFriction * dt), velocity.y);

            //left right
            if (attackTime <= 0f && dodgeTime <= 0f)
                velocity += new Vector2(input.hor * physicsParams.horAcceleration * dt, 0f);

            //jump
            {
                bool isInitialJump = false;
                if (input.jumpDown && numJumps < physicsParams.maxNumJumps) isInitialJump = true;
                else if ((input.jumpDown || input.jumpCachedDown) && onGround) isInitialJump = true;

                if (isInitialJump)
                {
                    numJumps++;
                    velocity = new Vector2(velocity.x, physicsParams.jumpInitial);
                    if (!onGround)
                    {
                        doubleJumpAnim.transform.position = trans.position + new Vector3(0f, -0.1f, 0f);
                        doubleJumpAnim.SetTrigger("Jump");
                    }
                    SetState(State.Jump);
                }
                else if (input.jump)
                {
                    velocity += new Vector2(0f, physicsParams.jumpAcceleration * dt);
                }
                else if (attackTime > 0f)
                {
                    velocity = new Vector2(velocity.x, 0f);
                }
            }

            //max speed
            velocity = new Vector2(Mathf.Clamp(velocity.x, -physicsParams.velocityMax.x, physicsParams.velocityMax.x), Mathf.Clamp(velocity.y, -physicsParams.velocityMax.y, physicsParams.velocityMax.y));

            //dodge
            {
                if (dodgeTime <= -physicsParams.timeDodgeCooloff)
                {
                    if (input.dodgeDown)
                    {
                        velocity = Dodge(input, physicsParams);
                    }
                }
                else if (dodgeTime > 0f)
                {
                    velocity = dodgeDir * physicsParams.speedDodge * (dodgeTime / physicsParams.timeDodge);
                }
                dodgeTime -= dt;
            }

            //attack
            {
                if (attackTime <= 0f)
                {
                    if (input.attack0Down || input.attack0CacheDown)
                    {
                        Attack(Button.ATTACK0, input.mode);
                    }
                    else if (input.attack1Down || input.attack1CacheDown)
                    {
                        Attack(Button.ATTACK1, input.mode);
                    }
                    else if (input.attack0 && input.mode)
                    {
                        Attack(Button.ATTACK0, input.mode);
                    }
                    else if (attackTime < -physicsParams.timeCancelAttackPattern)
                    {
                        currentAttackPattern = null;
                        currentAttack = null;
                    }
                }
                attackTime -= dt;
            }

            //set bg lines
            float velocityXRatio = (Mathf.Abs(2f * velocity.x / 3f) + Mathf.Abs(1f * velocity.y / 3f)) / (2f * physicsParams.velocityMax.x / 3f + 1f * physicsParams.velocityMax.y / 3f);
            float velocityYRatio = (Mathf.Abs(1f * velocity.x / 3f) + Mathf.Abs(2f * velocity.y / 3f)) / (1f * physicsParams.velocityMax.x / 3f + 2f * physicsParams.velocityMax.y / 3f);
            float lineXRatio = velocityXRatio * 0.1f + Mathf.Clamp(0f, bgLineTimer, 0.9f);
            float lineYRatio = velocityYRatio * 0.1f + Mathf.Clamp(0f, bgLineTimer, 0.9f);
            lineHor.localScale = new Vector3(
                96f * curveBgLineX.Evaluate(lineXRatio),
                0.3f * curveBgLineY.Evaluate(lineXRatio),
                1f);
            lineVer.localScale = new Vector3(
                0.3f * curveBgLineY.Evaluate(lineYRatio),
                96f * curveBgLineX.Evaluate(lineYRatio),
                1f);
            bgLineTimer = Mathf.Clamp01(bgLineTimer - dt);

            body.velocity = velocity;

            onGroundOld = onGround;
        }

        void Attack(Button buttonPressed, bool mode)
        {
            AttackPattern selectedAttackPattern = null;
            AttackPattern.Attack selectedAttack = null;
            if (currentAttackPattern != null && currentAttackPattern.attacks[currentAttackPattern.attacks.Length - 1] == currentAttack)
            {
                currentAttackPattern = null;
                currentAttack = null;
            }

            if (currentAttackPattern == null)
            {
                foreach (var pattern in attackPatterns)
                {
                    if (pattern.attacks[0].button == buttonPressed && pattern.attacks[0].mode == mode)
                    {
                        selectedAttackPattern = pattern;
                        selectedAttack = pattern.attacks[0];
                    }
                }
            }
            else
            {
                int currentAttackIndex = -1;
                for (int i = 0, len = currentAttackPattern.attacks.Length; i < len; i++)
                {
                    if (currentAttackPattern.attacks[i] == currentAttack)
                    {
                        currentAttackIndex = i + 1;
                        if (i < len - 1)
                        {
                            if (currentAttackPattern.attacks[i + 1].button == buttonPressed && currentAttackPattern.attacks[i + 1].mode == mode)
                            {
                                selectedAttackPattern = currentAttackPattern;
                                selectedAttack = currentAttackPattern.attacks[i + 1];
                            }
                        }
                        break;
                    }
                }
                if (selectedAttackPattern == null)
                {
                    foreach (var pattern in attackPatterns)
                    {
                        if (pattern == currentAttackPattern) continue;
                        for (int i = 0, len = Mathf.Min(currentAttackPattern.attacks.Length, pattern.attacks.Length, currentAttackIndex + 1); i < len; i++)
                        {
                            if (i < currentAttackIndex)
                            {
                                if (currentAttackPattern.attacks[i].button != pattern.attacks[i].button || currentAttackPattern.attacks[i].mode != pattern.attacks[i].mode)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (buttonPressed == pattern.attacks[i].button && mode == pattern.attacks[i].mode)
                                {
                                    selectedAttackPattern = pattern;
                                    selectedAttack = pattern.attacks[i];
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (selectedAttackPattern == null || selectedAttack == null) return;
            currentAttackPattern = selectedAttackPattern;
            currentAttack = selectedAttack;
            attackTime = currentAttack.period;
            if (currentAttack.squirt > 0) Squirt(currentAttack.squirt);
            SetState(currentAttack.state);
            bgLineTimer += 0.4f;
        }

        void Squirt(int numEML)
        {
            Vector2 pos = squirtTrans.position;
            Vector2 dir = new Vector2(Mathf.Sign(trans.localScale.x), 0f);
            for (int i = 0; i < numEML; i++)
            {
                EMLParticle p = Pool.EMLParticle.GetEMLParticle();
                p.Init(pos, dir);
            }
        }

        Vector2 Dodge(InputMan input, PhysicsParams physicsParams)
        {
            dodgeTime = physicsParams.timeDodge;
            if (input.ver != 0f)
                dodgeDir = new Vector2(0f, input.ver);
            else if (input.hor != 0f)
                dodgeDir = new Vector2(input.hor, 0f);
            else
                dodgeDir = new Vector2(Mathf.Sign(trans.localScale.x), 0f);
            SetState(State.Dodge);
            return dodgeDir * physicsParams.speedDodge;
        }

        bool OnGround()
        {
            Bounds boxBounds = boxCollider.bounds;
            Vector2 bottomMiddle = new Vector2(boxBounds.center.x, boxBounds.center.y - boxBounds.extents.y);
            Vector2 xOffset = new Vector2(boxBounds.extents.x, 0f);
            Collider2D collider = Physics2D.OverlapArea(bottomMiddle - xOffset, bottomMiddle + xOffset + new Vector2(0f, 0.1f), groundLayerMask);
            return collider != null;
        }
    }
}
