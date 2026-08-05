using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Games.Abused
{
	public class SoulText : MonoBehaviour {

	    public Text text;
	    private int lastPickedSoul;

		void Start ()
	    {
	        text.color = new Color(1f, 1f, 1f, 0f);
		}
		
		void Update ()
	    {
	        text.text = "" + SoulPoint.pickedSoul + " / " + SoulPoint.totalSoul;

	        if (text.color.a > 0f)
	        {
	            text.color = new Color(1f, 1f, 1f, text.color.a - Game.dt / 8f);
	        }

	        if (lastPickedSoul != SoulPoint.pickedSoul/**/ && SoulPoint.pickedSoul != 0)
	        {
	            text.color = new Color(1f, 1f, 1f, 1f);
	            if (SoulPoint.pickedSoul == SoulPoint.totalSoul)
	            {
	                GetComponent<AudioSource>().pitch = 0.5f;
	                GetComponent<AudioSource>().Play();
	            }
	        }

	        if (SoulPoint.pickedSoul == SoulPoint.totalSoul)
	        {
	            if (Camera.main.orthographicSize > 1f)
	            {
	                Camera.main.orthographicSize = Mathf.Max(0.01f, Camera.main.orthographicSize - 7f * Game.dt / 8f);
                }
                RenderGrayScale.instance.greyScaleRatio += Game.dt;
                if (text.color.a <= 0f)
	            {
	                Application.Quit();
	            }
	            else
	            {
	                text.text = "";
	                for (int i = 0; i < 6; i++)
	                {
	                    if (Random.value < 0.8f)
	                    {
	                        text.text += Random.Range(0, 10);
	                    }
	                    else
	                    {
	                        if (Random.value < 0.5f)
	                        {
	                            text.text += "A";
	                        }
	                        else
	                        {
	                            if (Random.value < 0.5f)
	                            {
	                                text.text += "B";
	                            }
	                            else
	                            {
	                                text.text += "C";
	                            }
	                        }
	                    }
	                }
	                text.text += "/" + SoulPoint.totalSoul;
	            }
	        }

	        lastPickedSoul = SoulPoint.pickedSoul;
		}
	}

}
