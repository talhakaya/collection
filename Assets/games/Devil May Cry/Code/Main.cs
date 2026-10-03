using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.DevilMayCry
{
    public class Main : MonoBehaviour
    {
        public static Main inst;

        public Character player;
        public List<Enemy> enemies;
        public PhysicsParams physicsParams;
        public InputMan input;
        public AnimationCurve dieTimeCurve;
        private float timeScaleTime;
        private float timeScalePeriod;

        private float bgLineTimer;

        // In the collection: the old project's 2D layer collision matrix (which layers pass
        // through each other) is applied while the game runs, and time is put back to normal
        // speed when it is left (dying slows it down).
        static readonly int[] ignoredLayerPairs = { 0, 11, 0, 12, 0, 13, 0, 14, 1, 11, 1, 12, 1, 13, 1, 14, 2, 11, 2, 12, 2, 13, 2, 14, 4, 11, 4, 12, 4, 13, 4, 14, 5, 11, 5, 12, 5, 13, 5, 14, 8, 11, 8, 12, 8, 13, 8, 14, 9, 9, 9, 10, 9, 11, 9, 13, 10, 10, 10, 12, 10, 14, 11, 11, 11, 12, 11, 13, 11, 14, 12, 12, 12, 13, 12, 14, 13, 13, 13, 14, 14, 14 };

        void Awake()
        {
            Collection.Controls.PortHelpers.PhysicsWithinGame(gameObject, true, ignoredLayerPairs);
        }

        void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        void Start()
        {
            timeScaleTime = 100f;
            inst = this;
            input = new InputMan();
            input.Init(physicsParams);
        }

        private void FixedUpdate()
        {
            input.PreUpdate();

            player.UpdateDt(Time.fixedDeltaTime, input, physicsParams);

            foreach (Enemy e in enemies)
                e.UpdateDt(Time.fixedDeltaTime, physicsParams);

            input.LateUpdate();
        }

        private void Update()
        {
            if (timeScaleTime < timeScalePeriod)
            {
                timeScaleTime += Time.unscaledDeltaTime;
                if (timeScaleTime >= timeScalePeriod)
                    Time.timeScale = 1f;
                else
                    Time.timeScale = dieTimeCurve.Evaluate(timeScaleTime / timeScalePeriod);
            }
        }

        public void OnDie(Actor actor)
        {
            timeScaleTime = 0f;
            if (actor is Character)
            {
                timeScalePeriod = 5f;
            }
            else if (actor is Enemy)
            {
                timeScalePeriod = 0.5f;
            }
        }

        public void OnHit(Actor actor)
        {
            if (actor is Character)
            {
                timeScalePeriod = 1f;
            }
        }
    }
}
