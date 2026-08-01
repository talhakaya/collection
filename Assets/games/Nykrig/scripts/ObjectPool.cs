using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.Nykrig
{
	public class ObjectPool : MonoBehaviour
	{
	    public enum PoolType { Other, Fire, Light, Audio, EnemyScale, EnemySimple, EnemyPortal, EnemySimpleSmall, LightPermanent, EnemyPlus, EnemyX, NightPill, NextLevel, Boss };
	    public static ObjectPool firePool;
	    public static ObjectPool lightPool;
	    public static ObjectPool audioPool;
	    public static ObjectPool enemyScalePool;
	    public static ObjectPool enemySimplePool;
	    public static ObjectPool enemyPortalPool;
	    public static ObjectPool enemySimpleSmallPool;
	    public static ObjectPool enemyPlusPool;
	    public static ObjectPool enemyXPool;
	    public static ObjectPool lightPermanentPool;
	    public static ObjectPool nightPillPool;
	    public static ObjectPool nextLevelPool;
	    public static ObjectPool bossPool;
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
	        else if (poolType == PoolType.EnemyScale) {
	            enemyScalePool = this;
	        }
	        else if (poolType == PoolType.EnemySimple) {
	            enemySimplePool = this;
	        }
	        else if (poolType == PoolType.EnemyPortal) {
	            enemyPortalPool = this;
	        }
	        else if (poolType == PoolType.EnemySimpleSmall) {
	            enemySimpleSmallPool = this;
	        }
	        else if (poolType == PoolType.LightPermanent) {
	            lightPermanentPool = this;
	        }
	        else if (poolType == PoolType.EnemyPlus) {
	            enemyPlusPool = this;
	        }
	        else if (poolType == PoolType.EnemyX) {
	            enemyXPool = this;
	        }
	        else if (poolType == PoolType.NightPill) {
	            nightPillPool = this;
	        }
	        else if (poolType == PoolType.NextLevel) {
	            nextLevelPool = this;
	        }
	        else if (poolType == PoolType.Boss) {
	            bossPool = this;
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

	    public GameObject get(Vector3 position)
	    {
	        return getObject(position);
	    }

	    public void Reset() {
	        for (int i = 0; i < NoOfObjects; i++) {
	            objects[i].SetActive(false);
	        }
	    }

	    public static void ResetAll() {
	        firePool.Reset();
	        lightPool.Reset();
	        audioPool.Reset();
	        enemyScalePool.Reset();
	        enemySimplePool.Reset();
	        enemyPortalPool.Reset();
	        enemySimpleSmallPool.Reset();
	        enemyPlusPool.Reset();
	        enemyXPool.Reset();
	        lightPermanentPool.Reset();
	        nightPillPool.Reset();
	        nextLevelPool.Reset();
	        bossPool.Reset();
	    }
	}

}
