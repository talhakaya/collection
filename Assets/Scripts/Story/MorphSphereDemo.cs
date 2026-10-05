using UnityEngine;

namespace Collection.Story
{
	/// <summary>
	/// Shows the Morph Sphere working: turns it slowly and takes each of its six sides
	/// from sphere to cube and back, one after the other, so that every mix of flat and
	/// round sides comes up.
	/// </summary>
	[RequireComponent(typeof(MorphSphere))]
	public class MorphSphereDemo : MonoBehaviour
	{
		[Tooltip("Seconds for one side to go from sphere to cube and back.")]
		public float period = 6f;

		[Tooltip("How far each side runs behind the one before it, as a part of the period.")]
		[Range(0f, 1f)] public float stagger = 0.19f;

		[Tooltip("Degrees a second.")]
		public Vector3 spin = new Vector3(7f, 23f, 0f);

		private MorphSphere sphere;

		private void Awake()
		{
			sphere = GetComponent<MorphSphere>();
		}

		private void Update()
		{
			transform.Rotate(spin * Time.deltaTime, Space.World);

			for (int side = 0; side < 6; side++)
			{
				// A wave that rests a while at both ends, so the plain cube side and the
				// plain sphere side are both seen.
				float wave = Mathf.Cos((Time.time / period - side * stagger) * Mathf.PI * 2f);
				sphere[side] = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(-0.7f, 0.7f, wave));
			}
		}
	}
}
