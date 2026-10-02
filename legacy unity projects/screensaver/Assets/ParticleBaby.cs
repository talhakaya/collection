using UnityEngine;
using System.Collections;

public class ParticleBaby : MonoBehaviour
{
    private TalhaColorChanger tint;
    private float period;
    private float timer;
    private const float MinPeriod = 0.1f;
    private const float MaxPeriod = 0.4f;
    private Vector3 vec;
    private Vector3 firstPos;

	void Start ()
    {
        firstPos = transform.position;
        tint = GetComponent<TalhaColorChanger>();
        reset();
	}
	
	void Update ()
    {
        timer += Game.dt;
        if (timer >= period)
        {
            reset();
        }
        else
        {
            transform.localScale += Vector3.one * Game.dt * period;
            transform.position += vec * Game.dt;
        }
	}

    void reset()
    {
        transform.position = firstPos;
        transform.localScale = Vector3.zero;
        period = Random.Range(MinPeriod, MaxPeriod);
        vec = Geometry.createVector3(Random.value * 360f, 2f + Random.value * 10f) - 10f * Vector3.forward / period;
        timer = 0f;
        tint.period = Random.Range(MinPeriod, MaxPeriod) * 6f;
    }
}
