using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Games.ToEveryoneIllNeverMeet
{
	public class FadeInUI : MonoBehaviour
	{
	    public Image image;
	    public Text text;
	    public float alpha = 1.1f;
	    public float extraTime;
	    public static FadeInUI globalText;
	    public static FadeInUI globalImage;

		void Start ()
	    {
	        image = GetComponent<Image>();
	        text = GetComponent<Text>();
	        if (text != null)
	        {
	            if (text.color.r > 0f)
	            {
	                globalText = this;
	                if (LevelPass.no == 0)
	                {
	                    text.text = "to everyone\nI'll never meet\n\nby Talha Kaya";
	                    extraTime = 5f;
	                }
	                else
	                {
	                    text.text = "";
	                }
	            }
	        }
	        if (image != null)
	        {
	            globalImage = this;
	            if (LevelPass.no == 0)
	            {
	                extraTime = 3f;
	            }
	        }
	        alpha += extraTime;
		}

		void Update ()
	    {
	        if (LevelPass.no != Game.LastLevel)
	        {
	            if (Game.fadeOut)
	            {
	                if (alpha < 1f)
	                {
	                    alpha += Time.deltaTime;
	                    if (image != null)
	                    {
	                        image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Min(1f, alpha));
	                    }
	                    if (text != null && text.color.r > 0f)
	                    {
	                        text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Min(1f, alpha));
	                    }
	                }
	                else if (image != null)
	                {
	                    LevelPass.no++;
	                    Game.travelling = true; // In the collection: the next level, not a fresh start
	                    SceneManager.LoadScene(SceneManager.GetActiveScene().path);
	                }
	            }
	            else if (alpha > 0f)
	            {
	                alpha -= Time.deltaTime;
	                if (image != null)
	                {
	                    image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Min(1f, alpha));
	                }
	                if (text != null && text.color.r > 0f)
	                {
	                    text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Min(1f, alpha));
	                }
	            }
	        }

	        if (text != null)
	        {
	            if (text.color.r == 0f)
	            {
	                text.text = globalText.text.text;
	                text.color = new Color(0f, 0f, 0f, globalText.text.color.a);
	            }
	        }
		}
	}
}
