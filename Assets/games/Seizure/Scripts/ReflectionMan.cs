using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Seizure
{
	public class ReflectionMan : MonoBehaviour
	{
	    public RenderTexture rt;
	    public List<Renderer> rends;
	    private int rendIndex;
	    public float scaleSpeed = 0.01f;

	    private float maxScale;

	    private void Start()
	    {
	        maxScale = 1f + 0.1f * rends.Count;
	    }

	    void FixedUpdate()
	    {
	        RenderTexture.active = rt;
	        Texture2D t = new Texture2D(rt.width, rt.height);
	        t.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
	        t.Apply();
	        rends[rendIndex].material.SetTexture("_BaseMap", t);
	        for (int i = 0, len = rends.Count; i < len; i++)
	        {
	            float scale = 1f + scaleSpeed * (len - i);
	            rends[(i + rendIndex) % len].transform.localScale = new Vector3(6.4f * scale, 3.6f * scale, 1f);
	            //float currentScale = rends[(i + rendIndex) % len].transform.localScale.x / 6.4f;
	            //if (currentScale > maxScale)
	            //{
	            //    currentScale -= maxScale - 1f;
	            //}
	            //else
	            //{
	            //    currentScale += scaleSpeed * Time.deltaTime * (len - i);
	            //}
	            //rends[(i + rendIndex) % len].transform.localScale = new Vector3(6.4f * currentScale, 3.6f * currentScale, 1f);
	            rends[(i + rendIndex) % len].transform.localPosition = new Vector3(0f, 0f, (len - i) * 0.01f);
	        }
	        rendIndex++;
	        if (rendIndex >= rends.Count) rendIndex = 0;
	    }
	}
}
