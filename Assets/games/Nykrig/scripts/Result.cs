using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Collection.Controls;

namespace Games.Nykrig
{
	public class Result : MonoBehaviour {
	    public Text textScore0;
	    public Text textScore1;
	    public Text textHighscore0;
	    public Text textHighscore1;
	    public GameObject newHighScore;
	    private float timer;
	    private bool gamepad;
	    private float mouseAngle;
	    private float mouseDistance;
	    public CanvasGroup canvasGroup;
	    private bool fireButtonOld;
	    public GameObject menu;
	    public int[] musicChannels;

	    void OnEnable () {
	        textScore0.text = "SCORE\n" + Game.score;
	        textScore1.text = textScore0.text;
	        int highScore = Collection.Saving.SaveManager.Slot.nykrig.score;
	        if (Game.score > highScore) {
	            newHighScore.SetActive(true);
	            textHighscore0.text = "";
	            Collection.Saving.SaveManager.Slot.nykrig.score = Game.score; Collection.Saving.SaveManager.MarkDirty();
	        }
	        else {
	            newHighScore.SetActive(false);
	            textHighscore0.text = "HIGH SCORE\n" + highScore;
	        }
	        textHighscore1.text = textHighscore0.text;

	        timer = 0f;
	        canvasGroup.alpha = 0f;
	        fireButtonOld = true;
	        Music.instance.Set(musicChannels, true);
	    }
		
		void Update () {
	        timer += Game.dt;
	        canvasGroup.alpha = Mathf.Min(1f, timer);
	        bool fireButton = (TaloketoInputManager.GetButton("Fire") || TaloketoInputManager.GetAxisRaw("FireAxis") > 0.5f);
	        if (timer > 1f && fireButton && !fireButtonOld) {
	            gameObject.SetActive(false);
	            Game.instance.StartGame(Game.instance.endless, Game.instance.twoPlayers);
	        }
	        fireButtonOld = fireButton;
	    }
	}

}
