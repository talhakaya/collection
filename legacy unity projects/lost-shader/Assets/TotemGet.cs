using UnityEngine;
using System.Collections;

public class TotemGet : MonoBehaviour
{
    public GameObject destroyObject;
    public GameObject enableObject;
    private float enabledTimer;
	// Use this for initialization
	void Start () {
        enableObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        enabledTimer += Game.dt;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        trigger(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        trigger(other);
    }

    void trigger(Collider2D other)
    {
        if (other.tag == "Player" && enabledTimer >= 30f)//other.transform.lossyScale.x >= 0.3f && 
        {
            enableObject.SetActive(true);
            Camera.main.transform.position = new Vector3(0f, 0f, -10f);
            Destroy(destroyObject);
        }
    }
}
