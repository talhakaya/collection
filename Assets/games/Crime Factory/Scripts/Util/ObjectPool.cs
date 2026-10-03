using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.CrimeFactory
{
	public class ObjectPool : MonoBehaviour
	{
	    public enum PoolType { LaserEnemy, FirePlayer, Bomb, Explosion, AudioPlayer, FireEnemy, Dead, Money, Dust };
	    public static ObjectPool laserEnemyPool;
	    public static ObjectPool firePlayerPool;
	    public static ObjectPool fireEnemyPool;
	    public static ObjectPool bombPool;
	    public static ObjectPool audioPlayerPool;
	    public static ObjectPool explosionPool;
	    public static ObjectPool deadPool;
	    public static ObjectPool moneyPool;
	    public static ObjectPool dustPool;
	    public PoolType poolType;
	    public GameObject objectPrefab;
	    public int NoOfObjects = 100;
	    public List<GameObject> objects;
	    public List<PhysBox> physBoxes;
	    private int objectCount;

	    void Start()
	    {
	        if (poolType == PoolType.LaserEnemy)
	        {
	            laserEnemyPool = this;
	        }
	        else if (poolType == PoolType.FirePlayer) {
	            firePlayerPool = this;
	        }
	        else if (poolType == PoolType.Bomb) {
	            bombPool = this;
	        }
	        else if (poolType == PoolType.Explosion) {
	            explosionPool = this;
	        }
	        else if (poolType == PoolType.AudioPlayer) {
	            audioPlayerPool = this;
	        }
	        else if (poolType == PoolType.FireEnemy) {
	            fireEnemyPool = this;
	        }
	        else if (poolType == PoolType.Dead) {
	            deadPool = this;
	        }
	        else if (poolType == PoolType.Money) {
	            moneyPool = this;
	        }
	        else if (poolType == PoolType.Dust) {
	            dustPool = this;
	        }
	        objects = new List<GameObject>();
	        physBoxes = new List<PhysBox>();
	        for (int i = 0; i < NoOfObjects; i++)
	        {
	            GameObject go = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
	            go.transform.parent = transform;
	            objects.Add(go);
	            go.SetActive(false);
	            physBoxes.Add(go.GetComponent<PhysBox>());
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

	    public PhysBox getPhys(Vector3 position) {
	        GameObject go = getObject(position);
	        PhysBox phys = go.GetComponent<PhysBox>();
	        phys.rect.position = position;
	        return phys;
	    }

	    public void ResetObjs() {
	        objectCount = 0;
	        for (int i = 0; i < NoOfObjects; i++) {
	            objects[i].SetActive(false);
	        }
	    }
	}
}
