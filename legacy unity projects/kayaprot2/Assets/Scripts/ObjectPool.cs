using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public enum PoolType { Other, Fire, Light, Audio, Explosion, Smoke, Blood, Noise };
    public static ObjectPool firePool;
    public static ObjectPool lightPool;
    public static ObjectPool audioPool;
    public static ObjectPool explosionPool;
    public static ObjectPool smokePool;
    public static ObjectPool bloodPool;
    public static ObjectPool noisePool;
    public PoolType poolType;
    public GameObject objectPrefab;
    public int NoOfObjects = 100;
    private List<GameObject> objects;
    private int objectCount;

    void Start()
    {
        if (poolType == PoolType.Fire)
        {
            firePool = this;
        }
        else if (poolType == PoolType.Light) {
            lightPool = this;
        }
        else if (poolType == PoolType.Audio) {
            audioPool = this;
        }
        else if (poolType == PoolType.Explosion) {
            explosionPool = this;
        }
        else if (poolType == PoolType.Smoke) {
            smokePool = this;
        }
        else if (poolType == PoolType.Blood) {
            bloodPool = this;
        }
        else if (poolType == PoolType.Noise) {
            noisePool = this;
        }
        else {
            throw new System.NotImplementedException();
        }
        objects = new List<GameObject>();
        for (int i = 0; i < NoOfObjects; i++)
        {
            GameObject go = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.parent = transform;
            objects.Add(go);
            go.SetActive(false);
        }
        objectCount = 0;
    }

    private GameObject getObject(Vector3 position)
    {
        if (objects[objectCount] == null)
        {
            GameObject newgo = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            newgo.transform.parent = transform;
            objects[objectCount] = newgo;
        }
        GameObject go = objects[objectCount];
        objectCount++;
        if (objectCount >= NoOfObjects)
        {
            objectCount = 0;
        }
        go.transform.position = position;
        go.SetActive(true);
        return go;
    }

    public GameObject get(Vector3 position) {
        return getObject(position);
    }

    public GameObject get(Vector3 position, Vector3 scale) {
        GameObject go = getObject(position);
        go.transform.localScale = scale;
        return go;
    }

    public void Create(Vector3 pos, int particleCount = 7) {
        for (int i = 0, len = particleCount; i < len; i++) {
            get(pos);
        }
    }
}
