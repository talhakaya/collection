using UnityEngine;
using UnityEngine.Video;
using System.Collections;

namespace Games.IncrediblePenis
{
	// In the collection: rewritten around VideoPlayer. MovieTexture, which this was written
	// for, no longer exists. The video is drawn into the renderer's main texture as before,
	// its sound goes through the AudioSource when playSound is set, and a video that is not
	// looping moves on to the next scene when it ends.
	[RequireComponent (typeof(AudioSource))]
	public class VideoPlay : MonoBehaviour {

		public VideoClip movTexture;
		public bool skipScene;
		public bool playSound;

		private VideoPlayer player;
		private bool ended;

		void Start()
		{
			player = gameObject.AddComponent<VideoPlayer>();
			player.playOnAwake = false;
			player.source = VideoSource.VideoClip;
			player.clip = movTexture;
			player.renderMode = VideoRenderMode.MaterialOverride;
			player.targetMaterialRenderer = GetComponent<Renderer>();
			player.targetMaterialProperty = GetComponent<Renderer>().material.HasProperty("_BaseMap") ? "_BaseMap" : "_MainTex";
			player.isLooping = !skipScene;
			player.skipOnDrop = true;
			if (playSound)
			{
				player.audioOutputMode = VideoAudioOutputMode.AudioSource;
				player.SetTargetAudioSource(0, GetComponent<AudioSource>());
			}
			else
			{
				player.audioOutputMode = VideoAudioOutputMode.None;
			}
			player.loopPointReached += onEnd;
			player.Play();
		}

		void onEnd(VideoPlayer source)
		{
			ended = true;
		}

		void Update()
		{
			if (skipScene && ended)
			{
				ended = false;
				Levels.next();
			}
		}
	}

	// In the collection: the game walked through its build order with
	// Application.LoadLevel(loadedLevel + 1) and quit after the last one. The order is kept
	// here and the scenes are loaded by path; after the last one it is back to the
	// collection's menu.
	public static class Levels
	{
		public static readonly string[] order = { "video0", "scene0", "video1", "scene1", "video3", "video2", "scene2", "video4", "scene3", "video5" };

		public static void next()
		{
			string current = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			int i = System.Array.IndexOf(order, current);
			if (i < 0 || i == order.Length - 1)
			{
				Collection.Controls.GlobalInputManager.ReturnToMainMenu();
			}
			else
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/games/Incredible Penis/Scenes/" + order[i + 1] + ".unity");
			}
		}
	}
}
