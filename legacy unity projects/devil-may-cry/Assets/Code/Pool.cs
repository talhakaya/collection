using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2
{
    public class Pool : MonoBehaviour
    {
        public enum ObjectType
        {
            EMLParticle,
            HitParticle,
            SplashParticle,
            Bullet,
        }

        public static Pool EMLParticle;
        public static Pool HitParticle;
        public static Pool SplashParticle;
        public static Pool Bullet;

        [SerializeField] private ObjectType objectType;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int numInstances = 100;

        private int index;
        private GameObject[] instances;
        private static EMLParticle[] instancesEMLParticle;

        void Start()
        {
            instances = new GameObject[numInstances];
            for (int i = 0; i < numInstances; i++)
            {
                GameObject go = Instantiate(prefab, transform);
                go.SetActive(false);
                instances[i] = go;
            }
            switch (objectType)
            {
                case ObjectType.EMLParticle:
                    EMLParticle = this;
                    instancesEMLParticle = new EMLParticle[numInstances];
                    for (int i = 0; i < numInstances; i++)
                    {
                        instancesEMLParticle[i] = instances[i].GetComponent<EMLParticle>();
                    }
                    break;
                case ObjectType.HitParticle:
                    HitParticle = this;
                    break;
                case ObjectType.SplashParticle:
                    SplashParticle = this;
                    break;
                case ObjectType.Bullet:
                    Bullet = this;
                    break;
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

        public EMLParticle GetEMLParticle()
        {
            EMLParticle res = instancesEMLParticle[index];
            res.gameObject.SetActive(true);
            index++;
            if (index >= numInstances) index = 0;
            return res;
        }
    }
}
