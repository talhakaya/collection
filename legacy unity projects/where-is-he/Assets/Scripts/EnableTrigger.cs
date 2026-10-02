using UnityEngine;
using System.Collections;

public class EnableTrigger : MonoBehaviour {

    public GameObject[] enableWhenTriggered;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            for (int i = 0; i < enableWhenTriggered.Length; i++)
            {
                enableWhenTriggered[i].SetActive(true);
            }
            Destroy(gameObject);
        }
    }
}
