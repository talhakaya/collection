using UnityEngine;
using System.Collections.Generic;

namespace Games.Nykrig
{
	public class Level : MonoBehaviour {
	    public int[] musicChannels;
	    public float minX = 0;
	    public float maxX = 0;
	    public float minY = 0;
	    public float maxY = 0;
	    public bool generateOuterWalls;

	    void OnValidate() {
	        if (generateOuterWalls) {
	            //foreach (Transform child in transform) {
	            //    if (child.name == "Wall(Clone)") {
	            //        DestroyImmediate(child.gameObject);
	            //    }
	            //}
	            GameObject wallPrefab = Resources.Load("Wall") as GameObject;
	            GameObject w0 = Instantiate(wallPrefab, transform) as GameObject;
	            GameObject w1 = Instantiate(wallPrefab, transform) as GameObject;
	            GameObject w2 = Instantiate(wallPrefab, transform) as GameObject;
	            GameObject w3 = Instantiate(wallPrefab, transform) as GameObject;
	            float levelMinX = (minX - 1) * Game.levelUnitX;
	            float levelMaxX = (maxX + 1) * Game.levelUnitX;
	            float levelMinY = (minY - 1) * Game.levelUnitY;
	            float levelMaxY = (maxY + 1) * Game.levelUnitY;
	            w0.transform.localPosition = new Vector3(levelMinX + 0.5f, (levelMinY + levelMaxY) * 0.5f, 0f);
	            w1.transform.localPosition = new Vector3(levelMaxX - 0.5f, (levelMinY + levelMaxY) * 0.5f, 0f);
	            w2.transform.localPosition = new Vector3((levelMinX + levelMaxX) * 0.5f, levelMinY + 0.5f, 0f);
	            w3.transform.localPosition = new Vector3((levelMinX + levelMaxX) * 0.5f, levelMaxY - 0.5f, 0f);
	            w0.transform.localScale = new Vector3(1f, (levelMaxY - levelMinY), 1f);
	            w1.transform.localScale = new Vector3(1f, (levelMaxY - levelMinY), 1f);
	            w2.transform.localScale = new Vector3((levelMaxX - levelMinX), 1f, 1f);
	            w3.transform.localScale = new Vector3((levelMaxX - levelMinX), 1f, 1f);
	        }
	    }

	    public void Spawn() {
	        foreach (Transform child in transform) {
	            if (child.GetComponent<Spawn>() != null) {
	                child.GetComponent<Spawn>().Create();
	            }
	        }
	    }
	}

}
