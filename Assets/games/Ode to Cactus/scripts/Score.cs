using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Games.OdeToCactus
{
	public class Score : MonoBehaviour {

	    public static int score;
	    private Text text;
	    public Color[] colors;
	    public Color filter = new Color(1f, 1f, 1f);
	    public float colorRatio = 1;
	    public float period = 1f;
	    public int[] colorIDs;
	    private int i = 0;
	    private float ratio = 0f;

		void Start ()
	    {
	        text = GetComponent<Text>();
	        if (colors.Length == 0 && colorIDs.Length > 0)
	        {
	            colors = new Color[colorIDs.Length];
	            for (int i = 0; i < colorIDs.Length; i++)
	            {
	                colors[i] = Game.colors[colorIDs[i]];
	            }
	        }

	        if (colors.Length != 0)
	        {
	            text.color = colors[0];
	        }
		}

		void Update ()
	    {
	        text.text = "Score: " + score;
	        if (colors.Length != 0)
	        {
	            ratio = 1f - (Game.time % period) / period;
	            i = Mathf.FloorToInt((Game.time % (period * colors.Length)) / period);
	            if (i == colors.Length - 1)
	            {
	                text.color = new Color(colors[i].r * ratio + colors[0].r * (1f - ratio), colors[i].g * ratio + colors[0].g * (1f - ratio), colors[i].b * ratio + colors[0].b * (1f - ratio), text.color.a);
	            }
	            else
	            {
	                text.color = new Color(colors[i].r * ratio + colors[i + 1].r * (1f - ratio), colors[i].g * ratio + colors[i + 1].g * (1f - ratio), colors[i].b * ratio + colors[i + 1].b * (1f - ratio), text.color.a);
	            }
	            text.color = new Color(filter.r * text.color.r, filter.g * text.color.g, filter.b * text.color.b, text.color.a);

	            if (colorRatio != 1f)
	            {
	                text.color = new Color(1f - (1f - text.color.r) * colorRatio, 1f - (1f - text.color.g) * colorRatio, 1f - (1f - text.color.b) * colorRatio, 1f - (1f - text.color.a) * colorRatio);
	            }
	        }
		}
	}
}
