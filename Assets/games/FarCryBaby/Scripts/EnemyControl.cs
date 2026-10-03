using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.FarCryBaby
{
    public class EnemyControl : MonoBehaviour
    {
        public enum State
        {
            None,
            Passive,
            Suspicious,
            Aggressive
        }

        public Actor actor;
        public State state;
        public LayerMask playerLayer;

        private Vector3 passiveWalkDir;
        private float passiveWalkTime;
        private float passiveWaitTime;

        private Vector3 suspiciousPos;
        public float suspiciousTimer;
        private const float suspiciousMaxTime = 5f;

        private float aggressiveTimer;
        private float aggressiveInputVer;
        private bool aggressiveInputFire;

        private void OnEnable()
        {
            Debug.Assert(MapView.inst != null);
            if (MapView.inst.enemies == null) MapView.inst.enemies = new List<Transform>();
            MapView.inst.enemies.Add(transform);
        }

        private void OnDisable()
        {
            if (MapView.inst.enemies == null) MapView.inst.enemies = new List<Transform>();
            MapView.inst.enemies.Remove(transform);
        }

        private void Update()
        {
            float inputHor = 0f;
            float inputVer = 0f;
            bool inputFire = false;
            bool inputAim = false;
            bool inputCrouch = false;
            bool inputJump = false;
            Vector3 lookAt = transform.position;
            float distToPlayer = (PlayerControl.inst.transform.position - transform.position).magnitude;
            bool canSeePlayer = CanSeePlayer();

            switch (state)
            {
                case State.Passive:
                    if (passiveWalkTime <= 0f && passiveWaitTime <= 0f)
                    {
                        passiveWaitTime = UnityEngine.Random.Range(0f, 3f);
                        passiveWalkTime = passiveWaitTime + UnityEngine.Random.Range(3f, 10f);
                        passiveWalkDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized;
                    }
                    if (passiveWalkTime > 0f) passiveWalkTime -= Time.deltaTime;
                    else if (passiveWaitTime > 0f) passiveWaitTime -= Time.deltaTime;
                    inputVer = passiveWalkTime > 0f ? 0.3f : 0f;
                    lookAt += passiveWalkDir;

                    if ((distToPlayer < 60f && canSeePlayer) || actor.hurtTimer > 0f)
                    {
                        state = State.Suspicious;
                        suspiciousPos = PlayerControl.inst.actor.GetPosToBeAimed();
                        suspiciousTimer = suspiciousMaxTime * 0.5f;
                    }
                    break;
                case State.Suspicious:
                    inputVer = 1f;
                    lookAt = suspiciousPos;

                    if (canSeePlayer) suspiciousTimer += 2f * Time.deltaTime;
                    else suspiciousTimer -= Time.deltaTime;

                    if (suspiciousTimer < 0f)
                    {
                        state = State.Passive;
                        passiveWalkTime = 0f;
                        passiveWaitTime = 0f;
                    }
                    else if (distToPlayer < 20f || suspiciousTimer >= suspiciousMaxTime)
                    {
                        state = State.Aggressive;
                    }
                    break;
                case State.Aggressive:
                    lookAt = PlayerControl.inst.actor.GetPosToBeAimed();
                    if (aggressiveTimer <= 0f)
                    {
                        if (distToPlayer > 40f || (distToPlayer > 10 && !canSeePlayer))
                        {
                            aggressiveInputVer = 1f;
                            aggressiveInputFire = false;
                        }
                        else
                        {
                            aggressiveInputVer = 0f;
                            aggressiveInputFire = true;
                        }
                        aggressiveTimer = 1f;
                    }
                    aggressiveTimer -= Time.deltaTime;
                    inputVer = aggressiveInputVer;
                    inputFire = aggressiveInputFire;
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            actor.Move(new Vector3(inputHor, inputJump ? 1f : 0f, inputVer), inputFire, inputAim, inputCrouch, lookAt);
        }

        public bool CanSeePlayer()
        {
            if (PlayerControl.inst.IsHidden()) return false;
            Vector3 dirTowardsPlayer = (PlayerControl.inst.actor.GetPosToBeAimed() - transform.position).normalized;
            Ray ray = new Ray(transform.position, dirTowardsPlayer);
            bool cast = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, playerLayer, QueryTriggerInteraction.Ignore);
            if (!cast || hitInfo.transform != PlayerControl.inst.transform) return false;
            float playerLookAngle = Mathf.Acos(Vector3.Dot(dirTowardsPlayer, transform.forward));
            return playerLookAngle < Mathf.PI * 0.25f;
        }
    }
}
