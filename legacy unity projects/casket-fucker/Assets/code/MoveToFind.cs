using UnityEngine;
using System.Collections;

public class MoveToFind : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float moveTime;
    public GameObject goal;
    private float audioTime;
    private AudioSource audioSource;

	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
	}
	
	void Update ()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * moveSpeed * Game.dt;
	    if (audioSource != null)
        {
            if (audioTime > 0)
            {
                audioSource.enabled = true;
                audioTime -= Game.dt;
            }
            else
            {
                audioSource.enabled = false;
            }
        }
	}

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == goal)
        {
            audioTime = Game.dt * 10f;
            moveTime -= Game.dt;
            if (moveTime < 0)
            {
                State.next();
            }
        }
    }
}
