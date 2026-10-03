using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.ToEveryoneIllNeverMeet
{
	public class FacingPlayer : MonoBehaviour
	{
	    public string[] sentences;
	    private bool spoken;
	    public static List<int> sortedSentences;

		void Start ()
	    {

		}

		void Update ()
	    {
	        if (Game.instance != null)
	        {
	            transform.LookAt(Game.instance.transform);
	            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
	        }
		}

	    void OnTriggerEnter(Collider other)
	    {
	        if (other.tag == "Player" && !spoken)
	        {
	            spoken = true;
	            string threeSentences = "";
	            threeSentences = getDifferentSentences();
	            FadeInUI.globalText.text.text = threeSentences;
	            FadeInUI.globalText.alpha = 6f;
	        }
	    }

	    public string getDifferentSentences()
	    {
	        if (sortedSentences == null)
	        {
	            sortedSentences = new List<int>();
	            for (int i = 0; i < (Game.LastLevel - 1) * 3; i++)
	            {
	                sortedSentences.Add(0);
	            }
	            for (int i = 0; i < (Game.LastLevel - 1) * 3; i++)
	            {
	                bool found = false;
	                while (!found)
	                {
	                    sortedSentences[i] = Random.Range(0, sentences.Length);
	                    found = true;
	                    for (int j = 0; j < i; j++)
	                    {
	                        if (sortedSentences[j] == sortedSentences[i])
	                        {
	                            found = false;
	                            break;
	                        }
	                    }
	                }
	            }
	        }

	        return sentences[sortedSentences[(LevelPass.no - 1) * 3]] + "\n\n" + sentences[sortedSentences[(LevelPass.no - 1) * 3 + 1]] + "\n\n" + sentences[sortedSentences[(LevelPass.no - 1) * 3 + 2]];
	    }
	}
}
