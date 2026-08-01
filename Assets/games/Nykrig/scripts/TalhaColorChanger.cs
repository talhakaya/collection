using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class TalhaColorChanger : MonoBehaviour {

		public Color[] colors;
		public Color filter = new Color(1f, 1f, 1f);
	    public float colorRatio = 1;
	    public float period = 1f;
		public int[] colorIDs;
		private int i = 0;
		private float ratio = 0f;
	    public float timerOffset;
		private TintScript tint;
		
		void Start ()
		{
			tint = GetComponent<TintScript> ();
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
				tint.changingColor = colors[0];
			}
		}
		
		void Update ()
		{
			if (colors.Length != 0)
			{
				ratio = 1f - ((Game.time + timerOffset) % period) / period;
				i = Mathf.FloorToInt((Game.time % (period * colors.Length)) / period);
				if (i == colors.Length - 1)
				{
					tint.changingColor = new Color(colors[i].r * ratio + colors[0].r * (1f - ratio), colors[i].g * ratio + colors[0].g * (1f - ratio), colors[i].b * ratio + colors[0].b * (1f - ratio), colors[i].a * ratio + colors[0].a * (1f - ratio));
				}
				else
				{
					tint.changingColor = new Color(colors[i].r * ratio + colors[i + 1].r * (1f - ratio), colors[i].g * ratio + colors[i + 1].g * (1f - ratio), colors[i].b * ratio + colors[i + 1].b * (1f - ratio), colors[i].a * ratio + colors[i + 1].a * (1f - ratio));
				}
				tint.changingColor = new Color(filter.r * tint.changingColor.r, filter.g * tint.changingColor.g, filter.b * tint.changingColor.b, tint.changingColor.a);

	            if (colorRatio != 1f)
	            {
	                tint.changingColor = new Color(1f - (1f - tint.changingColor.r) * colorRatio, 1f - (1f - tint.changingColor.g) * colorRatio, 1f - (1f - tint.changingColor.b) * colorRatio, 1f - (1f - tint.changingColor.a) * colorRatio);
	            }
			}
		}
	}

}
