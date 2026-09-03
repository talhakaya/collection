using System.Collections.Generic;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// Owns the scene being played and the ghosts of the songs already beaten. Ported from
	/// SceneManager.as.
	///
	/// Ghosts are full Scenes - same class, same spike and particle machinery - shrunk to
	/// 0.2 and laid along the bottom, fed the input recorded on the run that set the high
	/// score. Once a ghost's recording runs out it falls through to the live input, so the
	/// old runs finish by mirroring the player.
	///
	/// replayInputs is never saved to disk (see SaveManager), so ghosts only ever replay
	/// runs from the current session; that is the original's behaviour.
	/// </summary>
	public class SceneManager : FlashObject
	{
		public static List<float>[] replayInputs = NewReplayInputs();
		public static int[] scores = new int[6];
		public static int WHICHSONG = 1;

		public List<int> replayInputPointers;
		public List<Scene> passiveScenes;
		public List<bool> passiveSceneInputs;
		public bool musicStarted = false;
		public Scene activeScene;

		private static List<float>[] NewReplayInputs()
		{
			List<float>[] inputs = new List<float>[6];
			for (int i = 0; i < inputs.Length; i++)
			{
				inputs[i] = new List<float>();
			}

			return inputs;
		}

		/// Statics outlive a play-mode restart when domain reload is off.
		public static void ResetStatics()
		{
			replayInputs = NewReplayInputs();
			scores = new int[6];
			WHICHSONG = 1;
		}

		public static SceneManager New()
		{
			SceneManager manager = NewNode("SceneManager").AddComponent<SceneManager>();

			GameManager.time = -2000;
			// Sets rhythm and numberOfSections for the song, and both are read while the
			// Scenes below are built - so this has to come first.
			SoundManager.changeMusic("" + (GameManager.id + 1));

			manager.activeScene = Scene.New(GameManager.id + 1, false);
			manager.addChild(manager.activeScene);

			manager.passiveScenes = new List<Scene>();
			manager.passiveSceneInputs = new List<bool>();
			manager.replayInputPointers = new List<int>();

			for (int i = 0; i < GameManager.id; i++)
			{
				manager.passiveScenes.Add(Scene.New(i + 1, true));
				manager.passiveScenes[i].scaleX = manager.passiveScenes[i].scaleY = 0.2f;
				manager.passiveScenes[i].x = Main.stageWidth * i / (float)GameManager.id;
				manager.passiveScenes[i].y = Main.stageHeight * (1f - manager.passiveScenes[i].scaleY);
				manager.addChild(manager.passiveScenes[i]);
				manager.passiveSceneInputs.Add(false);
				manager.replayInputPointers.Add(0);
			}

			return manager;
		}

		public void Tick()
		{
			activeScene.inputHandler(GameManager.getKeyDown());

			// The music starts when the two-second lead-in reaches zero, and from then on it
			// is the clock: SoundManager hands GameManager.time back off the audio position.
			if (!musicStarted && GameManager.time >= 0)
			{
				musicStarted = true;
				SoundManager.skipToMusic(GameManager.time);
				SoundManager.playMusic();
			}

			for (int i = 0; i < passiveScenes.Count; i++)
			{
				// Recorded inputs are stored in beats, and each entry is a toggle rather than
				// a state - press, release, press.
				if (replayInputs[i].Count > replayInputPointers[i]
					&& GameManager.time > replayInputs[i][replayInputPointers[i]] * GameManager.rhythm)
				{
					passiveSceneInputs[i] = !passiveSceneInputs[i];
					replayInputPointers[i] = replayInputPointers[i] + 1;
				}

				if (replayInputs[i].Count > replayInputPointers[i])
				{
					passiveScenes[i].inputHandler(passiveSceneInputs[i]);
				}
				else
				{
					passiveScenes[i].inputHandler(GameManager.getKeyDown());
				}
			}

			sceneGraphicHandler();
		}

		public void sceneGraphicHandler()
		{
			activeScene.updateGraphics();
			for (int i = 0; i < passiveScenes.Count; i++)
			{
				passiveScenes[i].updateGraphics();
			}
		}
	}
}
