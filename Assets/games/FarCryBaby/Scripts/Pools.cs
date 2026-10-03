using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.FarCryBaby
{
    public enum PoolType
    {
        Bullet,
        OutlineCube,
        DeadCube,
        HealthBar,
        MapIcon
    }

    public class Pools : MonoBehaviour
    {

        [Serializable]
        public class PoolData
        {
            public PoolType poolType;
            public GameObject prefab;
            public int numInstances;
            public Transform parent;
        }

        public PoolData[] prefabs;

        private Pool[] pools;

        private static Pools inst;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (pools != null) return;
            inst = this;
            int len = Enum.GetValues(typeof(PoolType)).Length;
            pools = new Pool[len];
            foreach (PoolData data in prefabs)
            {
                pools[(int)data.poolType] = new Pool(data.prefab, data.numInstances, data.parent != null ? data.parent : transform);
            }
        }

        public static GameObject Get(PoolType poolType)
        {
            return inst.pools[(int)poolType].Get();
        }

        public static int Size(PoolType poolType)
        {
            return inst.pools[(int)poolType].numInstances;
        }

        public static void ReturnAll(PoolType poolType)
        {
            foreach (GameObject go in inst.pools[(int)poolType].instances) go.SetActive(false);
        }
    }

    public class Pool
    {
        public GameObject prefab;
        public int numInstances;

        private int index;
        public GameObject[] instances;
        private Transform parent;

        public Pool(GameObject prefab, int numInstances, Transform parent = null)
        {
            this.prefab = prefab;
            this.numInstances = numInstances;
            this.parent = parent;
            instances = new GameObject[numInstances];
            for (int i = 0; i < numInstances; i++)
            {
                GameObject go = GameObject.Instantiate(prefab, this.parent);
                go.SetActive(false);
                instances[i] = go;
            }
        }

        public GameObject Get()
        {
            GameObject res = instances[index];
            res.SetActive(true);
            index++;
            if (index >= numInstances) index = 0;
            return res;
        }
    }
}
