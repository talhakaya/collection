using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public enum PoolType { Terrain, Circle, AudioPlayer };
    public static ObjectPool terrainPool;
    public static ObjectPool circlePool;
    public static ObjectPool audioPlayerPool;
    public PoolType poolType;
    public GameObject objectPrefab;
    public int NoOfObjects = 100;
    private List<GameObject> objects;
    private int objectCount;

    void Start()
    {
        if (poolType == PoolType.Terrain)
        {
            terrainPool = this;
        }
        else if (poolType == PoolType.Circle)
        {
            circlePool = this;
        }
        else if (poolType == PoolType.AudioPlayer)
        {
            audioPlayerPool = this;
        }
        if (!Game.instance.isMenu && ((!Game.circleHoleEffectOn && poolType == PoolType.Circle) || (!Game.terrainEffectOn && poolType == PoolType.Terrain)))
        {
            Destroy(gameObject);
        }
        else
        {
            objects = new List<GameObject>();
            for (int i = 0; i < NoOfObjects; i++)
            {
                GameObject go = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                if (go.GetComponent<OutlineSprite>() != null)
                {
                    go.GetComponent<OutlineSprite>().Init();
                }
                go.transform.parent = transform;
                objects.Add(go);
                go.SetActive(false);
            }
            objectCount = 0;
        }
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

    public GameObject get(Vector3 position)
    {
        return getObject(position);
    }
}
