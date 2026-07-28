using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.Golfinity
{
	public class Pool : MonoBehaviour
	{
	    public enum PoolType { Terrain, Circle, AudioPlayer, CoinIcon };
	    public static Pool terrainPool;
	    public static Pool circlePool;
	    public static Pool audioPlayerPool;
	    public static Pool coinIconPool;
	    public static Pool mapLevelUiPool;
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
	        else if (poolType == PoolType.CoinIcon)
	        {
	            coinIconPool = this;
	        }
	        objects = new List<GameObject>();
	        for (int i = 0; i < NoOfObjects; i++)
	        {
	            Vector3 scale = objectPrefab.transform.localScale;
	            GameObject go = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
	            if (go.GetComponent<OutlineSprite>() != null)
	            {
	                go.GetComponent<OutlineSprite>().Init();
	            }
	            go.transform.SetParent(transform);
	            go.transform.localScale = scale;
	            objects.Add(go);
	            go.SetActive(false);
	        }
	        objectCount = 0;
	    }

	    private GameObject getObject(Vector3 position)
	    {
	        if (objects[objectCount] == null)
	        {
	            Vector3 scale = objectPrefab.transform.localScale;
	            GameObject newgo = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
	            newgo.transform.SetParent(transform);
	            newgo.transform.localScale = scale;
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

}
