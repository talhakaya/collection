using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    public Vector3 playerBeginPos;
    private List<GameObject> enemies;
    private List<Door> doors;
    public bool finished;

	void Start ()
    {
        doors = new List<Door>();
        enemies = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Enemy>() != null)
            {
                enemies.Add(child.gameObject);
            }
            else if (child.GetComponent<Door>() != null)
            {
                doors.Add(child.GetComponent<Door>());
            }
        }
	}
	
	void Update ()
    {
        if (!finished)
        {
            bool noEnemyLeft = true;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].activeSelf)
                {
                    noEnemyLeft = false;
                    break;
                }
            }
            if (noEnemyLeft)
            {
                finished = true;
                for (int i = 0; i < doors.Count; i++)
                {
                    doors[i].open();
                }
                if (enemies.Count > 0)
                {
                    RoomCleared.create();
                }
            }
        }
	}

    public void reset()
    {
        PlayerScript.instance.transform.position = playerBeginPos;
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SetActive(true);
        }
    }
}
