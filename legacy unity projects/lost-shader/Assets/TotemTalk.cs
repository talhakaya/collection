using UnityEngine;
using System.Collections;

public class TotemTalk : MonoBehaviour
{
    private float timer;
    private float screenshakeTimer;
    private int counter;
    public GameObject[] texts;
    private bool screenshake;
    public GameObject disableKid;
    public GameObject enableKid;
    public GameObject destroyObject;
    public GameObject enableObject;

	void Start ()
    {
        timer = 5f;
	}
	
	void Update ()
    {
        
        timer -= Game.dt;

        if (timer <= 0f)
        {
            if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
            {
                timer = 0.5f;
                counter++;
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].SetActive(i < counter);
                }
                if (counter == texts.Length + 1)
                {
                    screenshake = true;
                }
            }
        }

        if (screenshake)
        {
            screenshakeTimer += Game.dt;
            if (screenshakeTimer > 20f)
            {
                //the end
                Destroy(destroyObject);
            }
            if (screenshakeTimer > 18f)
            {
                //intro kid but with color
                disableKid.SetActive(false);
                enableKid.SetActive(true);
            }
            else if (screenshakeTimer > 10f)
            {
                enableObject.SetActive(true);
                GetComponent<AudioSource>().Stop();
            }
            else
            {
                transform.position += new Vector3(Random.value - 0.5f, Random.value - 0.5f, 0f) * Game.dt;
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].transform.position += Vector3.down * (texts[i].transform.position.y + 10f) * Game.dt * 0.1f;
                }
            }
        }
	}
}
