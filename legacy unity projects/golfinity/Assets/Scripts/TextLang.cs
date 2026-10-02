using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextLang : MonoBehaviour
{
    private Text text;
    public string textEN;
    public string textTR;

	void Start ()
    {
        text = GetComponent<Text>();
        if (Game.lang == Lang.EN)
        {
            text.text = textEN;
        }
        else
        {
            text.text = textTR;
        }
	}
	
	void Update ()
    {
        if (Game.lang == Lang.EN)
        {
            text.text = textEN;
        }
        else
        {
            text.text = textTR;
        }
	}
}
