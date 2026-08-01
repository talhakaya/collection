using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.SpaceArtist
{
	public class LevelManager : MonoBehaviour
	{
	    public static LevelManager instance;
	    public GameObject[] levels;
	    public static List<GameObject> getObjects;

		void Start ()
	    {
	        instance = this;
		}
		
		public void updateLevel ()
	    {
	        PlayerScript.instance.Reset();
		    
	        if (Game.level == 26)
	        {
	            for (int i = 0; i < levels.Length; i++)
	            {
	                levels[i].SetActive(true);
	                foreach (Transform child in levels[i].transform)
	                {
	                    if (child.tag == "Planet")
	                    {
	                        child.gameObject.SetActive(true);
	                    }
	                    else
	                    {
	                        child.gameObject.SetActive(false);
	                    }
	                }
	                float x = -2 + (i % 5);
	                float y = +3 - (i / 5);
	                levels[i].transform.localPosition = new Vector3(16f * x, 12f * y, 0f);
	            }
	        }
	        else
	        {
	            for (int i = 0; i < levels.Length; i++)
	            {
	                levels[i].SetActive(i == Game.level);
	                if (i == Game.level)
	                {
	                    getObjects = new List<GameObject>();
	                    foreach (Transform child in levels[i].transform)
	                    {
	                        if (child.tag == "GetObject" && child.gameObject.activeSelf)
	                        {
	                            getObjects.Add(child.gameObject);
	                        }
	                    }
	                }
	            }
	        }
		}

	    public void resetLevel()
	    {
	        foreach (Transform child in levels[Game.level].transform)
	        {
	            child.gameObject.SetActive(true);
	        }
	        PlayerScript.instance.Reset();
	    }
	}

}
