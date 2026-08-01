using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Collection.Controls;

namespace Games.Nykrig
{
	public class Menu : MonoBehaviour {
	    public GameObject[] enableObjects;
	    public Text[] texts;
	    private bool fireButtonOld;
	    public Text textHighscore0;
	    public Text textHighscore1;
	    public int[] musicChannels;
	    private bool shouldSayNykrig;

	    void OnEnable() {
		    for (int i = 0, len = enableObjects.Length; i < len; i++) {
	            enableObjects[i].SetActive(false);
	        }
	        int score = PlayerPrefs.GetInt("score", 0);
	        if (score == 0) {
	            textHighscore0.text = "";
	        }
	        else {
	            textHighscore0.text = "ENDLESS HIGH SCORE\n" + score;
	        }
	        textHighscore1.text = textHighscore0.text;
	        Music.instance.Set(musicChannels, true);
	        shouldSayNykrig = true;
	    }

	    void Update() {
	        if (shouldSayNykrig) {
	            shouldSayNykrig = false;
	            TalhaAudioSource.PlayInstance(AudioResources.instance.nykrig);
	        }

	        bool fireButton = TaloketoInputManager.GetButton("Fire") || TaloketoInputManager.GetAxisRaw("FireAxis") > 0.5f;

	        if (enableObjects[0].activeSelf) {
	            for (int i = 0, len = texts.Length; i < len; i++) {
	                float deltaAngle = Mathf.Abs(Game.mouseAngle - Geometry.angleOfVector2(texts[i].rectTransform.anchoredPosition));
	                if (Game.mouseDistanceClamped < 2f) {
	                    texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 0f);
	                }
	                else if (deltaAngle < 90f) {
	                    texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, (Game.mouseDistanceClamped - 2f) * (90f - deltaAngle) / 90f);
	                }
	                else {
	                    texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, Game.mouseDistanceClamped * 0.05f);
	                }
	            }

	            if (!fireButtonOld && fireButton) {
	                if (Game.mouseDistanceClamped >= 2f) {
	                    float textAlpha = 0f;
	                    int textIndex = -1;
	                    for (int i = 0, len = texts.Length; i < len; i++) {
	                        if (texts[i].color.a > textAlpha) {
	                            textAlpha = texts[i].color.a;
	                            textIndex = i;
	                        }
	                    }

	                    if (textIndex == 0) {
	                        Game.instance.StartGame(false, false);
	                        gameObject.SetActive(false);
	                    }
	                    else if (textIndex == 1) {
	                        Game.instance.StartGame(false, true);
	                        gameObject.SetActive(false);
	                    }
	                    else if (textIndex == 2) {
	                        Game.instance.StartGame(true, false);
	                        gameObject.SetActive(false);
	                    }
	                    else if (textIndex == 3) {
	                        Game.instance.StartGame(true, true);
	                        gameObject.SetActive(false);
	                    }
	                }
	                else {
	                    OnEnable();
	                }
	            }
	        }
	        else {
	            if (!fireButtonOld && fireButton) {
	                for (int i = 0, len = enableObjects.Length; i < len; i++) {
	                    enableObjects[i].SetActive(true);
	                    //if (enableObjects[i].GetComponent<Image>() != null) {
	                    //    Image image = enableObjects[i].GetComponent<Image>();
	                    //    image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
	                    //}
	                    if (enableObjects[i].GetComponent<Text>() != null) {
	                        Text text = enableObjects[i].GetComponent<Text>();
	                        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);

	                    }
	                }
	            }
	        }
	        fireButtonOld = fireButton;
	    }
	}

}
