using UnityEngine;
using System.Collections;

namespace Games.BloodSpace
{
	public class Highscore : MonoBehaviour {

		private TextBlood textBlood;

		void Start ()
		{
			textBlood = GetComponent<TextBlood> ();
			textBlood.text = ": " + Collection.Saving.SaveManager.Slot.bloodSpace.highscore;
		}

		void Update ()
		{
			if (Game.score > Collection.Saving.SaveManager.Slot.bloodSpace.highscore)
			{
				Collection.Saving.SaveManager.Slot.bloodSpace.highscore = Game.score; Collection.Saving.SaveManager.MarkDirty();
				textBlood.text = ": " + Game.score;
			}
		}
	}
}
