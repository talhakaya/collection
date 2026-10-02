using UnityEngine;
using System.Collections;

public class Plate : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Spoon")
        {
            other.GetComponent<Spoon>().getCereal();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Spoon")
        {
            other.GetComponent<Spoon>().getCereal();
        }
    }
}
