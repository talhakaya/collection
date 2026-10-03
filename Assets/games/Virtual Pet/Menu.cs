using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Games.VirtualPet
{
	public class Menu : MonoBehaviour
	{
	    public Button continueButton;
	    public Button resetButton;
	    private int resetButtonCounter;
	    public GameObject[] game;

		void Start ()
	    {
		    if (PlayerPrefs.GetInt("VirtualPet.hasSave", 0) == 0)
	        {
	            continueButton.GetComponentInChildren<Text>().text = "start";
	            resetButton.GetComponentInChildren<Text>().text = "reset";
	            resetButton.interactable = false;
	        }
	        else
	        {
	            continueButton.GetComponentInChildren<Text>().text = "continue";
	            resetButton.GetComponentInChildren<Text>().text = "reset";
	            resetButtonCounter = 0;
	        }
		}

		void Update ()
	    {

		}

	    public void pressContinue()
	    {
	        for (int i = 0; i < game.Length; i++)
	        {
	            game[i].SetActive(true);
	        }
	        gameObject.SetActive(false);
	        PlayerPrefs.SetInt("VirtualPet.hasSave", 1);
	    }

	    public void pressReset()
	    {
	        if (resetButtonCounter == 0)
	        {
	            resetButton.GetComponentInChildren<Text>().text = "really?";
	        }
	        else if (resetButtonCounter == 1)
	        {
	            resetButton.GetComponentInChildren<Text>().text = "wanna reset?";
	        }
	        else if (resetButtonCounter == 2)
	        {
	            resetButton.GetComponentInChildren<Text>().text = "for real?";
	        }
	        else if (resetButtonCounter == 3)
	        {
	            resetButton.GetComponentInChildren<Text>().text = "not kidding?";
	        }
	        else if (resetButtonCounter == 4)
	        {
	            resetButton.GetComponentInChildren<Text>().text = "you don't love her?";
	        }
	        else if (resetButtonCounter == 5)
	        {
	            resetButton.GetComponentInChildren<Text>().text = "she loves you";
	        }
	        else if (resetButtonCounter == 6)
	        {
	            resetButton.GetComponentInChildren<Text>().text = "you shouldn't do this";
	        }
	        else if (resetButtonCounter == 7)
	        {
	            resetButton.GetComponentInChildren<Text>().text = "are you sure?";
	        }
	        else if (resetButtonCounter == 8)
	        {
	            resetButton.GetComponentInChildren<Text>().text = "ok";
	        }
	        else if (resetButtonCounter == 9)
	        {
	            continueButton.GetComponentInChildren<Text>().text = "start";
	            resetButton.GetComponentInChildren<Text>().text = "reset";
	            resetButton.interactable = false;
	            // In the collection: was PlayerPrefs.DeleteAll(); only this game's keys go.
	            PlayerPrefs.DeleteKey("VirtualPet.hasSave");
	            PlayerPrefs.DeleteKey("VirtualPet.playerName");
	            PlayerPrefs.DeleteKey("VirtualPet.petName");
	        }
	        resetButtonCounter++;
	    }
	}
}
