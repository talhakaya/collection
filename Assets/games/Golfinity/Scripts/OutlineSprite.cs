using UnityEngine;
using System.Collections;

namespace Games.Golfinity
{
	public class OutlineSprite : MonoBehaviour
	{
	    public static bool isOn;
	    protected GameObject go;
	    protected int noOfSprites = 6;
	    protected GameObject[] gos;
	    protected bool inited;
	    protected const float outlineWidth = 0.1f;

	    private void Start()
	    {
	        Init();
	        GameEvents.OnOutlineOnOff += OnOutlineOnOff;
	    }

	    private void OnDestroy()
	    {
	        GameEvents.OnOutlineOnOff -= OnOutlineOnOff;
	    }

	    public void Init(bool force = false)
	    {
	        if (!inited || force)
	        {
	            inited = true;
	            if (isOn)
	            {
	                gos = new GameObject[noOfSprites];
	                for (int i = 0; i < noOfSprites; i++)
	                {
	                    go = new GameObject("Outline");
	                    go.transform.SetParent(transform);
	                    go.transform.localScale = Vector3.one;
	                    go.transform.position = transform.position + Vector3.forward * 5f + Geometry.createVector3(i * 360f / noOfSprites, outlineWidth);
	                    go.transform.localEulerAngles = Vector3.zero;
	                    SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
	                    sr.sprite = GetComponent<SpriteRenderer>().sprite;
	                    sr.color = Color.black;
	                    gos[i] = go;
	                }
	            }
	        }
	    }

	    void OnOutlineOnOff()
	    {
	        if (isOn)
	        {
	            if (gos == null)
	            {
	                Init(true);
	            }
	        }
	        else
	        {
	            if (gos != null)
	            {
	                for (int i = 0; i < noOfSprites; i++)
	                {
	                    Destroy(gos[i]);
	                }
	                gos = null;
	            }
	        }
		}
	}

}
