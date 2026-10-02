using UnityEngine;
using System.Collections;

public class LevelPass : MonoBehaviour
{
    public static int no = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Game.fadeOut = true;
        }
    }
}
