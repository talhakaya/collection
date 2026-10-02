using UnityEngine;
using System.Collections;

public class Villain : MonoBehaviour {

    public GameObject[] disableWhenEnter;
	
	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.Return))
        {
            for (int i = 0; i < disableWhenEnter.Length; i++)
            {
                disableWhenEnter[i].SetActive(false);
            }
        }
	}
}
