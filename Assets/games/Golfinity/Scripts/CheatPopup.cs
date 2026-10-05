using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Games.Golfinity
{
	public class CheatPopup : Popup
	{
	    public TMP_InputField inputGold;
	    public TMP_InputField inputStrokes;
	    public TMP_InputField inputHoles;

	    public void OnClickSave(int index)
	    {
	        switch (index)
	        {
	            case 0:
	                if (int.TryParse(inputGold.text, out var gold))
	                {
	                    Game.gold = gold;
	                    Collection.Saving.SaveManager.Slot.golfinity.gold = Game.gold; Collection.Saving.SaveManager.MarkDirty();
	                }
	                break;
	            case 1:
	                if (int.TryParse(inputStrokes.text, out var strokes))
	                {
	                    Game.noOfStrokes = strokes;
	                    Collection.Saving.SaveManager.Slot.golfinity.noOfStrokes = Game.noOfStrokes; Collection.Saving.SaveManager.MarkDirty();
	                }
	                break;
	            case 2:
	                if (int.TryParse(inputHoles.text, out var holes))
	                {
	                    int starsIndex = holes / Game.STAR_LENGTH;
	                    int charIndex = holes % Game.STAR_LENGTH;
	                    Game.stars = new List<string>();
	                    for (int i = 0; i < starsIndex; i++)
	                    {
	                        Game.stars.Add(new string('1', Game.STAR_LENGTH));
	                    }
	                    Game.stars.Add(new string('1', charIndex));
	                    Game.SaveStars();
	                }
	                break;

	        }
	    }

	    public void OnClickRemoveAds()
	    {
	        Game.removedAds = true;
	    }

	    public void OnClickBack()
	    {
	        Hide();
	    }
	}

}
