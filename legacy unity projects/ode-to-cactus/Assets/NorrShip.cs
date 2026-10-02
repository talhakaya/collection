using UnityEngine;
using System.Collections;

public class NorrShip : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        Game.nextLevel();
    }
}
