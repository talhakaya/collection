using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float lineHeightChange = 0.1f;
    public float lineRGBChange = 0.1f;
    public float lifeTime = 10f;
    private TintScript tint;
    private Rigidbody2D body;
    public float fastSpeedMinimum = 3f;
    private bool stopped;
    private int frameCounter;
    public Vector3 vel;  

	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        tint = GetComponent<TintScript>();
        body.gravityScale = 0f;
        GetComponent<Collider2D>().enabled = false;
        frameCounter = 0;
	}
	
	void Update ()
    {
        lifeTime -= Game.dt;
        frameCounter++;
        if (frameCounter == 2)
        {
            GetComponent<Collider2D>().enabled = true;
        }
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
        else
        {
            if (lifeTime < 1f)
            {
                tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, lifeTime);
            }
            if ((WorldWander.camPos.x + 16f < transform.position.x || WorldWander.camPos.x - 16f > transform.position.x || WorldWander.camPos.y + 9f < transform.position.y || WorldWander.camPos.y - 9f > transform.position.y))
            {
                Destroy(gameObject);
            }
        }

        if (!stopped)
        {
            transform.position += vel * Game.dt;
        }
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        Explosive e = collision.gameObject.GetComponent<Explosive>();
        if (e != null)
        {
            if (!stopped)
            {
                e.explode();
                kill();
            }
        }
        else if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Bullet")
        {
            if (!stopped)
            {
                LineManager.SrgbSplit += lineRGBChange;
                LineManager.Sheight += lineHeightChange;
                Game.screenShakeSmall();
            }
        }
        if (collision.gameObject.tag != "Player")
        {
            stopped = true;
            body.gravityScale = 1f;
            GetComponent<Collider2D>().enabled = false;
        }
    }

    void kill()
    {
        LineManager.SrgbSplit += lineRGBChange;
        LineManager.Sheight += lineHeightChange;
        Game.screenShakeSmall();
        Destroy(gameObject);
    }
}
