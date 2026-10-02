using UnityEngine;
using System.Collections;

public class end : MonoBehaviour {

    private float period;
    private float rotationSpeed;

    void Start()
    {
        transform.localScale = Vector3.zero;
        period = GetComponent<TalhaAnimation>().period;
        Game.time = 0f;
        Screen.showCursor = false;
    }

    void Update()
    {
        Game.roadSpeed = 10 + Game.time / 3.5f;
        if (Game.time % period < 1f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            transform.localScale = Vector3.zero;
            transform.rotation = Quaternion.identity;
            if (Random.value < 0.5f)
            {
                rotationSpeed = Random.Range(10f, 20f);
            }
            else
            {
                rotationSpeed = Random.Range(-20f, -10f);
            }
        }
        else if (Game.time % period < 1.5f)
        {
            transform.localScale = Vector3.one * (Game.time % period - 1f) * 6f;
            transform.Rotate(Vector3.forward * rotationSpeed * Game.dt);
        }
        else if (Game.time % period < 4.5f)
        {
            transform.localScale = Vector3.one * 3f;
        }
        else if (Game.time % period < 4.75f)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 2f * (4.75f - (Game.time % period)));
            transform.localScale = Vector3.one * (3f + Mathf.Pow(Game.time % period - 4.5f, 2f) * 100f);
        }
        else
        {
            transform.localScale = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Game.nextLevel();
        }
    }
}
