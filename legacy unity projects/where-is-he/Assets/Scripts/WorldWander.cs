using UnityEngine;
using System.Collections;

public class WorldWander : MonoBehaviour
{
    public static Transform currentRoom;
    public Transform player;
    public int levelX;
    public int levelY;
    private const float LevelWidth = 32f;
    private const float LevelHeight = 18f;
    public static Vector3 camPos;
    public GameObject[] enableWhenFinished;
    private bool finished;

	void Start ()
    {
        camPos = Camera.main.transform.position;
	}
	
	void Update ()
    {
        int lx = Mathf.FloorToInt((player.position.x + LevelWidth * 0.5f) / LevelWidth);
        int ly = Mathf.FloorToInt((player.position.y + LevelHeight * 0.5f) / LevelHeight);
        if (lx != levelX || ly != levelY)
        {
            levelX = lx;
            levelY = ly;
            foreach (Transform child in transform)
            {
                if (child.name == "" + levelX + "-" + levelY)
                {
                    child.gameObject.SetActive(true);
                    currentRoom = child;
                    if (currentRoom.GetComponent<Room>() != null)
                    {
                        currentRoom.GetComponent<Room>().playerBeginPos = player.position;
                    }
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
            Camera.main.transform.position = new Vector3(levelX * LevelWidth, levelY * LevelHeight, Camera.main.transform.position.z);
            camPos = Camera.main.transform.position;


            if (!finished)
            {
                if (levelY < -4)
                {
                    finished = true;
                    for (int i = 0; i < enableWhenFinished.Length; i++)
                    {
                        enableWhenFinished[i].SetActive(true);
                    }
                }
            }
        }
	}
}
