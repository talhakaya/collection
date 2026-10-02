using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
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
