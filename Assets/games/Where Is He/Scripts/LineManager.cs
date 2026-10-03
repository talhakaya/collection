using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.WhereIsHe
{
	public class LineManager : MonoBehaviour
	{
	    public enum Shape
	    {
	        Line,
	        Circle
	    }
	    public Shape shape;
	    private LineRenderer line;
	    public bool obey;
	    public int vertexCount = 100;
	    public int rgbSplitCount = 10;
	    public float width = 7f;
	    public float height = 0.1f;
	    public float rgbSplit = 1f;
	    public float rgbSplitOffset = 1f;
	    public static int SvertexCount = 100;
	    public static int SrgbSplitCount = 10;
	    public static float Swidth = 7f;
	    public static float Sheight = 0.2f;
	    public static float SrgbSplit = 0f;
	    private const float sineConst = Mathf.PI * 2f * 1f;
	    private List<LineRenderer> rgbSplitArray;
	    public Color filterColor;

		// In the collection: the shader is now named "Legacy Shaders/Particles/Additive" (the old
		// name finds nothing and threw). Its default grey tint of 0.5 is doubled by the shader to
		// mean "unchanged"; the project is in Linear colour space, where that came to 0.43 and
		// dimmed every line, so the tint is given as the value that still doubles to 1.
		static Material additive()
		{
		    Material material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
		    float grey = Mathf.LinearToGammaSpace(0.5f);
		    material.SetColor("_TintColor", new Color(grey, grey, grey, 0.5f));
		    return material;
		}

		void Start ()
	    {
	        if (obey)
	        {
	            vertexCount = SvertexCount;
	            rgbSplitCount = SrgbSplitCount;
	        }
	        line = GetComponent<LineRenderer>();
	        line.SetVertexCount(vertexCount);
	        line.material = additive();
	        Color c = new Color(filterColor.r, filterColor.g, filterColor.b, 0.3f * filterColor.a);
	        line.SetColors(c, c);
	        rgbSplitArray = new List<LineRenderer>();
	        for (int i = 0; i < rgbSplitCount; i++)
	        {
	            GameObject go = new GameObject("rgb" + i);
	            LineRenderer l = go.AddComponent<LineRenderer>();
	            rgbSplitArray.Add(l);
	            l.SetVertexCount(vertexCount);
	            l.material = additive();
	            l.SetWidth(0.3f, 0.3f);
	            go.transform.parent = transform;
	            go.transform.localEulerAngles = Vector3.zero;
	            go.transform.localPosition = Vector3.zero;
	            go.transform.localScale = Vector3.one;
	        }
		}

		void Update ()
	    {
	        if (obey)
	        {
	            width = Swidth;
	            height = Sheight;
	            rgbSplit = SrgbSplit + rgbSplitOffset;
	            if (rgbSplitOffset > 0f)
	            {
	                rgbSplitOffset -= Game.dt;
	                if (rgbSplitOffset < 0f)
	                {
	                    rgbSplitOffset = 0f;
	                }
	            }
	        }
	        Color c = new Color(filterColor.r, filterColor.g, filterColor.b, 0.3f * filterColor.a);
	        line.SetColors(c, c);
	        line.SetWidth(transform.lossyScale.y * 0.3f, transform.lossyScale.y * 0.3f);
	        if (shape == Shape.Circle)
	        {
	            drawCircle(line);
	        }
	        else if (shape == Shape.Line)
	        {
	            drawLine(line);
	        }
	        for (int i = 0; i < rgbSplitArray.Count; i++)
	        {
	            float f = ((Game.time + 3f * i / rgbSplitArray.Count) % 6f);
	            if (f > 3f)
	            {
	                f = 6f - f;
	            }
	            float r = 0f;
	            float g = 0f;
	            float b = 0f;
	            if (f < 0.5f)
	            {
	                r = f * 2f;
	            }
	            if (f < 1f)
	            {
	                r = (1f - f) * 2f;
	            }
	            else if (f < 1.5f)
	            {
	                g = (f - 1f) * 2f;
	            }
	            else if (f < 2f)
	            {
	                g = (2f - f) * 2f;
	            }
	            else if (f < 2.5f)
	            {
	                b = (f - 2f) * 2f;
	            }
	            else if (f < 3f)
	            {
	                b = (3f - f) * 2f;
	            }
	            Color c2 = new Color(r * filterColor.r, g * filterColor.g, b * filterColor.b, (0.1f + f * 0.1f) * filterColor.a);
	            rgbSplitArray[i].SetWidth(transform.lossyScale.y * (0.1f + (6f - f) * 0.1f), transform.lossyScale.y * (0.1f + (6f - f) * 0.1f));
	            rgbSplitArray[i].transform.localPosition = Geometry.createVector3(i * 360f / rgbSplitArray.Count, rgbSplit / transform.localScale.x);
	            rgbSplitArray[i].SetColors(c2, c2);

	            if (shape == Shape.Circle)
	            {
	                drawCircle(rgbSplitArray[i]);
	            }
	            else if (shape == Shape.Line)
	            {
	                drawLine(rgbSplitArray[i]);
	            }
	        }
		}

	    void drawLine(LineRenderer l, float timeOffset = 1f, float timeSpeed = 10f)
	    {
	        for (int i = 0; i < vertexCount; i++)
	        {
	            Vector3 v = new Vector3(-width + 2 * width * i / vertexCount, height * Mathf.Sin(Game.time + timeOffset + timeSpeed * (sineConst * i / vertexCount)), 0f);
	            l.SetPosition(i, l.transform.TransformPoint(v));
	        }
	    }

	    void drawCircle(LineRenderer l, float timeOffset = 1f, float timeSpeed = 10f)
	    {
	        for (int i = 0; i < vertexCount; i++)
	        {
	            Vector3 v = Geometry.createVector3(360f * i / (vertexCount - 1), width * 0.5f + height * Mathf.Sin(Game.time + timeOffset + timeSpeed * (sineConst * i / vertexCount)));
	            l.SetPosition(i, l.transform.TransformPoint(v));
	        }
	    }
	}
}
