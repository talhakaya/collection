using UnityEngine;
using System.Collections;

public class TalhaColorChanger : MonoBehaviour {

	public Color[] colors;
	public Color filter = new Color(1f, 1f, 1f);
	public int[] colorIDs;
	private float period = 1f;
	private int i = 0;
	private float ratio = 0f;
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
			ratio = 1f - (Game.time % period) / period;
			i = Mathf.FloorToInt((Game.time % (period * colors.Length)) / period);
			if (i == colors.Length - 1)
			{
				tint.changingColor = new Color(colors[i].r * ratio + colors[0].r * (1f - ratio), colors[i].g * ratio + colors[0].g * (1f - ratio), colors[i].b * ratio + colors[0].b * (1f - ratio), tint.selfColor.a);
			}
			else
			{
				tint.changingColor = new Color(colors[i].r * ratio + colors[i + 1].r * (1f - ratio), colors[i].g * ratio + colors[i + 1].g * (1f - ratio), colors[i].b * ratio + colors[i + 1].b * (1f - ratio), tint.selfColor.a);
			}
			tint.changingColor = new Color(filter.r * tint.changingColor.r, filter.g * tint.changingColor.g, filter.b * tint.changingColor.b, tint.changingColor.a);
		}
	}
}
