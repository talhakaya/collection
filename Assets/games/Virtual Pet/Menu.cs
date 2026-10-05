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
		    if (!Collection.Saving.SaveManager.Slot.virtualPet.hasSave)
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
	        Collection.Saving.SaveManager.Slot.virtualPet.hasSave = true; Collection.Saving.SaveManager.MarkDirty();
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
	            Collection.Saving.SaveManager.Slot.virtualPet.hasSave = false; Collection.Saving.SaveManager.MarkDirty();
	            Collection.Saving.SaveManager.Slot.virtualPet.playerName = ""; Collection.Saving.SaveManager.MarkDirty();
	            Collection.Saving.SaveManager.Slot.virtualPet.petName = ""; Collection.Saving.SaveManager.MarkDirty();
	        }
	        resetButtonCounter++;
	    }
	}
}
