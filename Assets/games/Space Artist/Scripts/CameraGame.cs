using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class CameraGame : MonoBehaviour
	{
	    public static CameraGame instance;
	    private TextureResolutionSetter pixelGlitcher;
	    public static float pixelGlitch;

		void Start ()
	    {
	        instance = this;
	        pixelGlitcher = GetComponent<TextureResolutionSetter>();
		}

	    void Update()
	    {
	        if (pixelGlitch > 0f)
	        {
	            pixelGlitch -= Game.dt;
	        }
	        pixelGlitcher.pixelGlitch = (pixelGlitch > 0f);
	    }
	}

}
