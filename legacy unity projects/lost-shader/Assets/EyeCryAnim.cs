using UnityEngine;
using System.Collections;

public class EyeCryAnim : MonoBehaviour
{
    public GameObject[] tears;
    public SpriteRenderer otherEye;
    public SpriteRenderer eye;
    private Sprite firstSprite;
    private float timer;
    private int counter;

	void Start ()
    {
        firstSprite = eye.sprite;
        for (int i = 0; i < tears.Length; i++)
        {
            tears[i].SetActive(i < counter);
        }
	}
	
	void Update ()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            timer = 0.4f;
            counter++;
            for (int i = 0; i < tears.Length; i++)
            {
                tears[i].SetActive(i < counter);
            }
        }
        timer -= Game.dt;

	    if (timer <= 0f)
        {
            eye.sprite = firstSprite;
            otherEye.sprite = firstSprite;
            GetComponent<TalhaAnimation>().enabled = false;
            otherEye.GetComponent<TalhaAnimation>().enabled = false;
        }
        else
        {
            GetComponent<TalhaAnimation>().enabled = true;
            otherEye.GetComponent<TalhaAnimation>().enabled = true;
        }
	}
}
