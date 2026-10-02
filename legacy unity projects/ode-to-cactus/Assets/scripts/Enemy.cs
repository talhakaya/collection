using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public float hp;
    private TintScript tint;
    public AudioClip aHit;

	// Use this for initialization
	void Start () {
        if (GetComponent<EnemyMain>() != null)
        {
            hp = 400;
        }
        else
        {
            hp = 100;
        }
        tint = GetComponent<TintScript>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (hp <= 0f)
        {
            if (GetComponent<EnemyMain>() != null)
            {
                Explosion.create(transform.position, Random.Range(8f, 12f));
                foreach (Transform child in transform)
                {
                    child.parent = null;
                }
                Score.score += 500;
            }
            else
            {
                Score.score += 100;
                Explosion.create(transform.position, Random.Range(4f, 6f));
            }
            
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.Contains("fire"))
        {
            AudioSource.PlayClipAtPoint(aHit, transform.position, Random.Range(0.1f, 0.2f));
            Explosion.create(other.transform.position, 1f);
            Destroy(other.gameObject);
            hp -= 3f;
            tint.changingColor = Color.white;
        }
    }
}
