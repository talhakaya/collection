using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Games.LetsNeverDoThatAgain
{
	public class Textt : MonoBehaviour {

	    public static Textt instance;
	    public string text;
	    public Text textObject;
	    private float period = 1f;

	    void Awake()
	    {
	        instance = this;
	    }

		void Start ()
	    {
	        textObject.text = text;
		}

		void Update ()
	    {
	        textObject.text = text;
	        if (period > 0f)
	        {
	            period -= Game.dt;
	            textObject.transform.rotation = Quaternion.identity;
	            textObject.transform.Rotate(Vector3.forward * Random.Range(-10f, 10f) * period);
	            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 1f - period);
	        }
	        else if (period > -2f)
	        {
	            period -= Game.dt;
	            textObject.transform.rotation = Quaternion.identity;
	            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 1f);
	        }
	        else if (period > -2.5f)
	        {
	            period -= Game.dt;
	            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 2 * (period + 2.5f));
	        }
	        else
	        {
	            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 0f);
	        }
		}

	    public static void updateText(string text)
	    {
	        instance.period = 1f;
	        instance.text = text;
	        Game.instance.aText.pitch = Random.Range(0.9f, 1.1f);
	        Game.instance.aText.volume = 0.5f;
	        Game.instance.aText.Play();
	    }
	}
}
