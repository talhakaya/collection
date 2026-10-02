using UnityEngine;
using System.Collections;

public class TriggerBox : MonoBehaviour {

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "Penguin" && other.transform.position.x > transform.position.x)
        {
            collider2D.isTrigger = false;
        }
    }
}
