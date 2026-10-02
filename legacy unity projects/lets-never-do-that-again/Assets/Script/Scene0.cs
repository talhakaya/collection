using UnityEngine;
using System.Collections;

public class Scene0 : MonoBehaviour {
    public static int sceneCount;
    private static int maxCount = 4;
    private float timeCounter = 0f;
	void Start ()
    {
        if (sceneCount == 0)
        {
            Physics.gravity = new Vector2(0f, -9.81f);
            Textt.updateText("Let's Never Do That Again");
        }
        else if (sceneCount == 1)
        {
            Textt.updateText("by Talha Kaya");
        }
        else if (sceneCount == 2)
        {
            Textt.updateText("and David Bowie");
        }
        else if (sceneCount == 3)
        {
            Textt.updateText("W to jump");
        }
        else if (sceneCount == 4)
        {
            Textt.updateText("");
            Penguin.instance.rigidbody2D.velocity = new Vector2(Penguin.instance.rigidbody2D.velocity.x, 2f);
        }
	}

    void Update()
    {
        if (sceneCount == 0 || sceneCount == 1 || sceneCount == 2)
        {
            if (Penguin.instance.transform.position.x >= 6.4f)
            {
                nextLevel();
            }
        }
        else if (sceneCount == 3)
        {
            if (Input.GetAxisRaw("Vertical") > 0f && Physics2D.gravity != Vector2.zero)
            {
                Physics2D.gravity = Vector2.zero;
                Penguin.instance.rigidbody2D.velocity = new Vector2(Penguin.instance.rigidbody2D.velocity.x, 12f);
            }
            else if (Physics2D.gravity == Vector2.zero)
            {
                Penguin.instance.rigidbody2D.velocity = new Vector2(Penguin.instance.rigidbody2D.velocity.x, 12f);
                if (Penguin.instance.transform.position.y >= 5f)
                {
                    nextLevel();
                }
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log(Physics2D.gravity);
            }
        }
        else if (sceneCount == 4)
        {
            
            timeCounter += Game.dt;
            if (timeCounter < 22f)
            {

            }
            else if (timeCounter < 24f)
            {
                Physics2D.gravity = new Vector2(0f, -9.81f);
            }
            else
            {
                nextLevel();
            }

        }
    }

    void nextLevel()
    {
        sceneCount++;
        if (sceneCount > maxCount)
        {
            sceneCount = 0;
        }
        Application.LoadLevel(sceneCount);
    }
}
