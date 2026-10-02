using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    public class InputMan
    {
        public bool jump;
        public bool jumpDown;
        public bool jumpCachedDown;
        private bool[] jumpCache;
        public bool attack0;
        public bool attack0Down;
        public bool attack0CacheDown;
        private bool[] attack0Cache;
        public bool attack1;
        public bool attack1Down;
        public bool attack1CacheDown;
        private bool[] attack1Cache;
        public bool mode;
        public bool modeDown;
        public bool modeCacheDown;
        private bool[] modeCache;
        public bool dodge;
        public bool dodgeDown;
        public bool dodgeCacheDown;
        private bool[] dodgeCache;
        public float hor;
        public bool horDown;
        public bool horCacheDown;
        private float[] horCache;
        public float ver;
        public bool verDown;
        public bool verCacheDown;
        private float[] verCache;


        public void Init(PhysicsParams physics)
        {
            jumpCache = new bool[physics.inputCacheLength];
            attack0Cache = new bool[physics.inputCacheLength];
            attack1Cache = new bool[physics.inputCacheLength];
            horCache = new float[physics.inputCacheLength];
            verCache = new float[physics.inputCacheLength];
            modeCache = new bool[physics.inputCacheLength];
            dodgeCache = new bool[physics.inputCacheLength];
        }

        public void PreUpdate()
        {
            jump = Input.GetButton("Jump");
            jumpDown = GetInputDown(jumpCache, jump);
            jumpCachedDown = GetInputCachedDown(jumpCache);
            attack0 = Input.GetButton("Attack0");
            attack0Down = GetInputDown(attack0Cache, attack0);
            attack0CacheDown = GetInputCachedDown(attack0Cache);
            attack1 = Input.GetButton("Attack1");
            attack1Down = GetInputDown(attack1Cache, attack1);
            attack1CacheDown = GetInputCachedDown(attack1Cache);
            mode = Input.GetButton("Mode");
            modeDown = GetInputDown(modeCache, mode);
            modeCacheDown = GetInputCachedDown(modeCache);
            dodge = Input.GetButton("Dodge");
            dodgeDown = GetInputDown(dodgeCache, dodge);
            dodgeCacheDown = GetInputCachedDown(dodgeCache);
            hor = Input.GetAxisRaw("Horizontal");
            horDown = GetInputDown(horCache, hor);
            horCacheDown = GetInputCachedDown(horCache);
            ver = Input.GetAxisRaw("Vertical");
            verDown = GetInputDown(verCache, ver);
            verCacheDown = GetInputCachedDown(verCache);
        }

        public void LateUpdate()
        {
            SetInputCache(jumpCache, jump);
            SetInputCache(attack0Cache, attack0);
            SetInputCache(attack1Cache, attack1);
            SetInputCache(modeCache, mode);
            SetInputCache(dodgeCache, dodge);
            SetInputCache(horCache, hor);
            SetInputCache(verCache, ver);
        }

        bool GetInputDown(bool[] inputCache, bool current)
        {
            bool inputLastFrame = inputCache[inputCache.Length - 1];
            return current && !inputLastFrame;
        }

        bool GetInputCachedDown(bool[] inputCache)
        {
            bool oldDown = false;
            for (int i = 0, len = inputCache.Length; i < len; i++)
            {
                if (i > 0 && inputCache[i] && !inputCache[i - 1]) oldDown = true;
            }
            return oldDown;
        }

        void SetInputCache(bool[] inputCache, bool current)
        {
            for (int i = 0, len = inputCache.Length; i < len - 1; i++)
            {
                inputCache[i] = inputCache[i + 1];
            }
            inputCache[inputCache.Length - 1] = current;
        }

        bool GetInputDown(float[] inputCache, float current)
        {
            float inputLastFrame = inputCache[inputCache.Length - 1];
            return current != inputLastFrame;
        }

        bool GetInputCachedDown(float[] inputCache)
        {
            bool oldDown = false;
            for (int i = 0, len = inputCache.Length; i < len; i++)
            {
                if (i > 0 && inputCache[i] != 0f && inputCache[i - 1] == 0f) oldDown = true;
            }
            return oldDown;
        }

        void SetInputCache(float[] inputCache, float current)
        {
            for (int i = 0, len = inputCache.Length; i < len - 1; i++)
            {
                inputCache[i] = inputCache[i + 1];
            }
            inputCache[inputCache.Length - 1] = current;
        }
    }

    public enum Button
    {
        HOR,
        VER,
        JUMP,
        DODGE,
        MODE,
        ATTACK0,
        ATTACK1,
    }
}
