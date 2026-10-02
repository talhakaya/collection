using UnityEngine;
using System.Collections;

public class Bush : MonoBehaviour {

	void Start ()
    {
        transform.localScale *= Random.Range(0.1f, 4f);
        transform.parent.position = Game.getDifferentPositions(1)[0];
	}
	
	void Update ()
    {
        if (Game.instance != null)
        {
            transform.parent.LookAt(Game.instance.transform);
            transform.parent.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        }
	}
}
