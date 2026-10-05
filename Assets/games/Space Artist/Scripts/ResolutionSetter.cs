using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class ResolutionSetter : MonoBehaviour
	{
	    public const float aspectRatio = 4f / 3f;
	    public const float udelta = 0.01f;
	    private Resolution maxResolution;

		// In the collection: this made the window 4:3 itself, every frame, and windowed. The
		// collection does that for the game now (its GameList entry asks for 4:3, which gives
		// it black bars at the sides), so nothing here runs.
		void Start ()
	    {
	        enabled = false;
	        if (!enabled) return;

	        maxResolution = Screen.resolutions[0];
	        for (int i = 1; i < Screen.resolutions.Length; i++)
	        {
	            if (Screen.resolutions[i].width > maxResolution.width || Screen.resolutions[i].height > maxResolution.height)
	            {
	                maxResolution = Screen.resolutions[i];
	            }
	        }
	        SetResolution(maxResolution.width - 100, maxResolution.height - 100);
		}
		
		void Update ()
	    {
	        SetResolution(Screen.width, Screen.height);
		}

	    void SetResolution(int width, int height)
	    {
	        float ratio = width / height;
	        if (ratio < aspectRatio - udelta || ratio > aspectRatio + udelta)
	        {
	            bool useWidth = (height * aspectRatio > width);
	            if (useWidth)
	            {
	                Screen.SetResolution(width, Mathf.RoundToInt(width / aspectRatio), false);
	            }
	            else
	            {
	                Screen.SetResolution(Mathf.RoundToInt(height * aspectRatio), height, false);
	            }
	        }
	    }
	}

}
