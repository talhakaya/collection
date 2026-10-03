using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Games.TheParasite
{
	public class TextShadow : MonoBehaviour {

	    private Vector3 firstPos;
	    private Vector3 offset;
	    private float timeCounter;
	    private float period;
	    private Text t;

	    void Start()
	    {
	        t = GetComponent<Text>();
	        firstPos = transform.localPosition;
	        Reset();
	    }

		void Update ()
	    {
	        timeCounter += Game.dt;
	        if (timeCounter >= period)
	        {
	            Reset();
	        }
	        else
	        {
	            transform.localPosition = firstPos + timeCounter * offset / period;
	            t.color = new Color(t.color.r, t.color.g, t.color.g, (period - timeCounter) / period);
	        }
		}

	    void Reset()
	    {
	        timeCounter = 0;
	        period = Random.Range(1f, 5f);
	        offset = Geometry.createVector3(Random.value * 360f, Random.value * 50f);
	        t.color = new Color(t.color.r, t.color.g, t.color.g, 1f);
	        transform.localPosition = firstPos;
	    }
	}
}
