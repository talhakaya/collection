using UnityEngine;

namespace Collection.Story
{
	/// <summary>
	/// Shows the Morph Sphere working: turns it slowly and takes each of its six sides
	/// from sphere to cube and back, one after the other, so that every mix of flat and
	/// round sides comes up. If the object has a ScreenFace, its face pulls one of its
	/// emotes every few seconds, picked at random.
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

		[Tooltip("Untick to stop this moving the six sides, and move the Morph Sphere's sliders by hand while it plays.")]
		public bool animateSides = true;

		[Tooltip("Seconds the face idles between emotes: the least and the most. The wait starts when the face is back to idle.")]
		public Vector2 emoteEvery = new Vector2(2f, 5f);

		private MorphSphere sphere;
		private ScreenFace face;
		private float nextEmote;

		private void Awake()
		{
			sphere = GetComponent<MorphSphere>();
			face = GetComponent<ScreenFace>();
			nextEmote = Time.time + Random.Range(emoteEvery.x, emoteEvery.y);
		}

		private void UpdateFace()
		{
			if (face == null) return;

			// The wait is counted from when the face is idle again, so a long emote does not
			// run straight into the next.
			if (face.Showing != ScreenFace.IdleName)
			{
				nextEmote = Time.time + Random.Range(emoteEvery.x, emoteEvery.y);
				return;
			}

			if (Time.time < nextEmote) return;

			int withVideo = 0;
			foreach (ScreenFace.Emote emote in face.emotes)
			{
				if (emote.clip != null) withVideo++;
			}

			if (withVideo == 0) return;

			int pick = Random.Range(0, withVideo);
			foreach (ScreenFace.Emote emote in face.emotes)
			{
				if (emote.clip == null) continue;
				if (pick-- == 0)
				{
					face.Show(emote.name);
					break;
				}
			}

			nextEmote = Time.time + Random.Range(emoteEvery.x, emoteEvery.y);
		}

		private void Update()
		{
			transform.Rotate(spin * Time.deltaTime, Space.World);
			UpdateFace();

			if (!animateSides) return;

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
