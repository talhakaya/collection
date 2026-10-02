using UnityEngine;
using System.Collections;

public class Spoon : MonoBehaviour
{
    public bool isFull;
    public int eatCount;
    public int stateChange = 2;
    public float maxSpeed;
    public float acceleration;
    private float speed;
    private Rigidbody2D body;
    public Transform linePos0;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        body = GetComponent<Rigidbody2D>();
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        lineRenderer.SetPosition(0, transform.position + Vector3.forward + Vector3.right * 3f);
        lineRenderer.SetPosition(1, linePos0.position);
        Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetMouseButton(0))
        {
            dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        }
        if (dir.x != 0f || dir.y != 0f)
        {
            speed += Game.dt * acceleration;
            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
        }
        else
        {
            speed = 0f;
        }
        body.velocity = Geometry.normalizeVector2(dir, speed * Game.timeSpeed);
    }

    public void getCereal()
    {
        isFull = true;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void eatCereal()
    {
        if (isFull)
        {
            eatCount++;
            isFull = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            if (eatCount >= stateChange)
            {
                changeState();
            }
        }
    }

    public MiniGame game;
    public void changeState()
    {
        game.end();
    }
}
