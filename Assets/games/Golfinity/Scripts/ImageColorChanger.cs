using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Games.Golfinity
{
	public class ImageColorChanger : MonoBehaviour {

	    public Color[] colors;
	    public Color filter = new Color(1f, 1f, 1f);
	    public float colorRatio = 1;
	    public float period = 1f;
	    public int[] colorIDs;
	    private int i = 0;
	    private float ratio = 0f;
	    private Image image;

	    void Start() {
	        image = GetComponent<Image>();
	        if (colors.Length == 0 && colorIDs.Length > 0) {
	            colors = new Color[colorIDs.Length];
	            for (int i = 0; i < colorIDs.Length; i++) {
	                colors[i] = Game.instance.level.terrainColors[colorIDs[i]];
	            }
	        }

	        if (colors.Length != 0) {
	            image.color = colors[0];
	        }
	    }

	    void Update() {
	        if (colors.Length != 0) {
	            ratio = 1f - (Game.time % period) / period;
	            i = Mathf.FloorToInt((Game.time % (period * colors.Length)) / period);
	            if (i == colors.Length - 1) {
	                image.color = new Color(colors[i].r * ratio + colors[0].r * (1f - ratio), colors[i].g * ratio + colors[0].g * (1f - ratio), colors[i].b * ratio + colors[0].b * (1f - ratio), image.color.a);
	            }
	            else {
	                image.color = new Color(colors[i].r * ratio + colors[i + 1].r * (1f - ratio), colors[i].g * ratio + colors[i + 1].g * (1f - ratio), colors[i].b * ratio + colors[i + 1].b * (1f - ratio), image.color.a);
	            }
	            image.color = new Color(filter.r * image.color.r, filter.g * image.color.g, filter.b * image.color.b, image.color.a);

	            if (colorRatio != 1f) {
	                image.color = new Color(1f - (1f - image.color.r) * colorRatio, 1f - (1f - image.color.g) * colorRatio, 1f - (1f - image.color.b) * colorRatio, 1f - (1f - image.color.a) * colorRatio);
	            }
	        }
	    }
	}

}
