using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour
{
    public static Music instance;
    public AudioClip iwill;
    public float iwillLength;
    public float songTimer;
    public string[] texts;

	void Start ()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (LevelPass.no == Game.LastLevel)
        {
            Destroy(instance.gameObject);
            instance = this;
            GetComponent<AudioSource>().clip = iwill;
            GetComponent<AudioSource>().Play();
            GetComponent<AudioSource>().loop = false;
        }
        else
        {
            Destroy(gameObject);
        }
	}
	
	void Update ()
    {
        if (LevelPass.no == Game.LastLevel)
        {
            songTimer += Time.deltaTime;

            if (songTimer > iwillLength)
            {
                Application.Quit();
            }
            else
            {
                int i = Mathf.RoundToInt(texts.Length * songTimer / iwillLength);
                if (i >= texts.Length)
                {
                    i = texts.Length - 1;
                }
                FadeInUI.globalText.text.text = texts[i];
                FadeInUI.globalText.alpha = 1.1f;
                if (songTimer < 1f)
                {
                    FadeInUI.globalImage.image.color = new Color(0f, 0f, 0f, 1f - songTimer);
                }
                else
                {
                    FadeInUI.globalImage.image.color = new Color(0f, 0f, 0f, (songTimer - 1f) / (iwillLength - 1f));
                }
            }
        }
	}
}
