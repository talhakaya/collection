using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Collection.Controls;

namespace Games.SpaceArtist
{
	public class Boss : MonoBehaviour
	{
	    public static Boss instance;
	    public Text text;
	    public string[] texts;
	    public int[] noOfPlanes;
	    public enum State { Disabled, WritingText, Waiting }
	    public State state;
	    private float timer;

		void Start ()
	    {
		    instance = this;
	        timer = 0f;
	        next();
		}
		
		void Update ()
	    {
		    if (state == State.Disabled)
	        {
	            if (text.color.a == 0f)
	            {
	                text.text = "";

	                if (Game.level < 26)
	                {
	                    bool noMoreGetObject = true;
	                    for (int i = 0; i < LevelManager.getObjects.Count; i++)
	                    {
	                        if (LevelManager.getObjects[i].gameObject.activeSelf)
	                        {
	                            noMoreGetObject = false;
	                            break;
	                        }
	                    }
	                    if (noMoreGetObject)
	                    {
	                        PlaneManager.set(false, noOfPlanes[Game.level]);
	                        next();
	                    }
	                }
	            }
	        }
	        else if (state == State.WritingText)
	        {
	            timer += Game.dt;
	            int noOfLetters = Mathf.RoundToInt(timer / 0.015f);
	            if (noOfLetters >= texts[Game.level].Length)
	            {
	                text.text = texts[Game.level];
	                state = State.Waiting;
	            }
	            else
	            {
	                text.text = texts[Game.level].Substring(0, noOfLetters) + "<color=#00000000>" + texts[Game.level].Substring(noOfLetters, texts[Game.level].Length - noOfLetters) + "</color>";
	            }
	        }
	        else if (state == State.Waiting)
	        {
	            if (TaloketoInputManager.GetMouseButtonDown(0))
	            {
	                state = State.Disabled;
	                Game.level++;
	                PlaneManager.set(true, noOfPlanes[Game.level]);
	                LevelManager.instance.updateLevel();
	            }
	        }
		}
	    
	    public void next()
	    {
	        state = State.WritingText;
	        timer = 0f;
	        GetComponent<TextureResolutionSetter>().pixelGlitch = (Game.level >= 12);
	    }
	}

}
