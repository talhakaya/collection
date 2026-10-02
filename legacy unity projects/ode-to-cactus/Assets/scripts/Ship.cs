using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {

    public GameObject fire;
    public float speed;
    public float firePeriod;
    public float fireCounter;
    private float slowTime;
    public GameObject enemy;
    public float enemyPeriod;
    public float enemyPeriodMultASec;
    private float enemyMultCounter;
    private float enemyCounter;
    public bool alive;
    private float deadTimeCounter;

	void Start ()
    {
        alive = true;
	}
	
	void Update ()
    {
        if (alive)
        {
            slowTime -= Game.dt;
            if (slowTime > 0f)
            {
                rigidbody2D.AddForce(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * speed * Game.dt);
            }
            else
            {
                rigidbody2D.AddForce(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * speed * Game.dt);
            }
            fireCounter += Game.dt;
            if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space))
            {
                if (fireCounter >= firePeriod)
                {
                    fireCounter -= firePeriod;
                    for (int i = 0; i < 4; i++)
                    {
                        Instantiate(fire, transform.position + Vector3.forward, Quaternion.identity);
                    }

                    slowTime = firePeriod;
                }
            }


            //enemy creation
            enemyCounter += Game.dt;
            if (enemyCounter > enemyPeriod)
            {
                enemyCounter -= enemyPeriod;
                enemyPeriod *= enemyPeriodMultASec;
                Instantiate(enemy, new Vector3(Random.Range(-7f, 7f), 10f, Random.Range(-0.1f, 0.1f)), Quaternion.identity);
            }

            enemyMultCounter += Game.dt;
            if (enemyMultCounter >= 1f)
            {
                enemyMultCounter--;
                enemyPeriod = enemyPeriod * enemyPeriodMultASec;
            }
        }
        else
        {
            deadTimeCounter += Game.dt;

            if (deadTimeCounter >= 5f || Input.GetKeyDown(KeyCode.Return))
            {
                Game.nextLevel();
            }
        }
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name.Contains("enemy"))
        {
            Explosion.create(transform.position, 20f);
            alive = false;
            transform.localScale = Vector3.zero;
        }
    }
}
