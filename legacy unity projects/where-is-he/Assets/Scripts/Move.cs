using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{
    public Vector3 moveVec;
    public float animSpeed;

    void Start()
    {
        if (GetComponent<Animator>() != null)
        {
            GetComponent<Animator>().speed = animSpeed;
        }
    }
	
	void Update ()
    {
        transform.position += moveVec * Game.dt;
	}
}
