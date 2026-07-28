using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class Spline : MonoBehaviour
	{
	    public const float yRange = 5f;
	    public const float distPerPoint = 12f;
	    public const int resolution = 5;
	    public bool callOnValidate;
	    public List<Vector3> points;
	    public LineRenderer lineRenderer;

	#if UNITY_EDITOR
	    private void OnValidate()
	    {
	        if (callOnValidate)
	        {
	            callOnValidate = false;
	            const int numPoints = LevelGenerator.numLevelsPerColor + 1;
	            points.Clear();
	            points.Add(new Vector3(0f, 0f, 0f));
	            for (int i = 1; i < numPoints; i++)
	            {
	                points.Add(new Vector3(distPerPoint * i, Random.Range(-2f * yRange, 2f * yRange), 0f));
	            }
	            points.Add(new Vector3(distPerPoint * numPoints, 0f, 0f));
	            int len = points.Count - 1;
	            lineRenderer.positionCount = len * resolution + 1;
	            for (int i = 0; i < len; i++)
	            {
	                Vector3 p0 = points[Mathf.Max(0, i - 1)];
	                Vector3 p1 = points[i];
	                Vector3 p2 = points[i + 1];
	                Vector3 p3 = points[Mathf.Min(len, i + 2)];
	                for (int j = 0; j < resolution; j++)
	                {
	                    lineRenderer.SetPosition(i * resolution + j, GetSplineValue(p0, p1, p2, p3, j * 1f / resolution));
	                }
	            }
	            lineRenderer.SetPosition(len * resolution, new Vector3(distPerPoint * numPoints, 0f, 0f));
	        }
	    }
	#endif

	    private Vector3 GetSplineValue(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	    {
	        Vector3 r = new Vector3();
	        r.x = 0.5f * (2 * p1.x + (-p0.x + p2.x) * t + (2 * p0.x - 5 * p1.x + 4 * p2.x - p3.x) * t * t + (-p0.x + 3 * p1.x - 3 * p2.x + p3.x) * t * t * t);
	        r.y = 0.5f * (2 * p1.y + (-p0.y + p2.y) * t + (2 * p0.y - 5 * p1.y + 4 * p2.y - p3.y) * t * t + (-p0.y + 3 * p1.y - 3 * p2.y + p3.y) * t * t * t);
	        r.z = 0f;
	        return r;
	    }
	}

}
