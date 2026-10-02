using UnityEngine;
using System.Collections;

public class State : MonoBehaviour
{
    public GameObject[] states;
    public static State instance;
    public int currentState;

	void Awake ()
    {
        instance = this;
	}

    void Start()
    {
        changeState();
    }
	
	void Update ()
    {
	
	}

    public static void next()
    {
        instance.currentState++;
        instance.changeState();
        instance.GetComponent<AudioSource>().Play();
    }

    void changeState()
    {
        for (int i = 0; i < states.Length; i++)
        {
            states[i].SetActive(i == currentState);
        }
    }
}
