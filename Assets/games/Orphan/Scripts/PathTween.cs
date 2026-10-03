using UnityEngine;

namespace Games.Orphan
{
	// In the collection: stands in for the two iTween calls the game made (MoveTo along a
	// path with easeInOutCubic, and Stop). iTween itself was written for Unity 3 and no
	// longer compiles (GUIText, GUITexture), so it was not brought in. Like iTween, the
	// path starts from where the object is, runs through the points as a Catmull-Rom
	// spline, and a new MoveTo replaces a running one.
	public class PathTween : MonoBehaviour
	{
		private Vector3[] points;
		private float duration;
		private float elapsed;

		public static void MoveTo(GameObject go, float time, Vector3[] path)
		{
			PathTween tween = go.GetComponent<PathTween>();
			if (tween == null) tween = go.AddComponent<PathTween>();

			// control points: a mirrored one before the start and one after the end
			Vector3[] p = new Vector3[path.Length + 3];
			p[1] = go.transform.position;
			for (int i = 0; i < path.Length; i++) p[i + 2] = path[i];
			p[0] = p[1] + (p[1] - p[2]);
			p[p.Length - 1] = p[p.Length - 2] + (p[p.Length - 2] - p[p.Length - 3]);

			tween.points = p;
			tween.duration = Mathf.Max(0.0001f, time);
			tween.elapsed = 0f;
			tween.enabled = true;
		}

		public static void Stop(GameObject go)
		{
			PathTween tween = go.GetComponent<PathTween>();
			if (tween != null) tween.enabled = false;
		}

		void Update()
		{
			if (points == null) return;
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / duration);
			transform.position = Interpolate(EaseInOutCubic(t));
			if (t >= 1f) enabled = false;
		}

		static float EaseInOutCubic(float t)
		{
			t *= 2f;
			if (t < 1f) return 0.5f * t * t * t;
			t -= 2f;
			return 0.5f * (t * t * t + 2f);
		}

		Vector3 Interpolate(float t)
		{
			int sections = points.Length - 3;
			int current = Mathf.Min(Mathf.FloorToInt(t * sections), sections - 1);
			float u = t * sections - current;
			Vector3 a = points[current];
			Vector3 b = points[current + 1];
			Vector3 c = points[current + 2];
			Vector3 d = points[current + 3];
			return 0.5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
		}
	}
}
