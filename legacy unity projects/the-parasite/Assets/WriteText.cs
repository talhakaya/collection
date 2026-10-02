using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WriteText : MonoBehaviour {

    public static string[] texts = new string[] { "The Parasite", "By Talha Kaya", "Play with mouse, select answers" };
    public static int appearId = -1;
    public int whichText;
    public bool sometimesAppear;
    private Text t;

	void Start ()
    {
        t = GetComponent<Text>();
	}
	
	void Update ()
    {
        if (!sometimesAppear || Game.answerId == whichText || Game.answerId == -1)
        {
            t.text = texts[whichText];
        }
        else
        {
            t.text = "";
        }
	}
}
