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
	                    PlayerPrefs.SetInt("gold", Game.gold);
	                }
	                break;
	            case 1:
	                if (int.TryParse(inputStrokes.text, out var strokes))
	                {
	                    Game.noOfStrokes = strokes;
	                    PlayerPrefs.SetInt("noOfStrokes", Game.noOfStrokes);
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
	                    for (int i = 0, len = Game.stars.Count; i < len; i++)
	                    {
	                        PlayerPrefs.SetString($"stars_{i}", Game.stars[i]);
	                    }
	                    PlayerPrefs.SetString($"stars_{Game.stars.Count}", "");
	                }
	                break;

	        }
	        PlayerPrefs.Save();
	    }

	    public void OnClickRemoveAds()
	    {
	        Game.removedAds = true;
	        PlayerPrefs.SetInt("Game.removedAds", 1);
	        PlayerPrefs.Save();
	    }

	    public void OnClickBack()
	    {
	        Hide();
	    }
	}

}
