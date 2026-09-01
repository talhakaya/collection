using System.Collections.Generic;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// One playthrough of one song: the scrolling timeline, the notes on it, the figure, the
	/// text, the particles and the scoring. Ported from Scene.as.
	///
	/// The same class runs the ghost replays. isReplay only changes where input comes from
	/// and how loud the sounds are (panning spreads the ghosts across the stereo field), so
	/// a passive scene is the same object at scale 0.2 fed a recorded input stream.
	///
	/// numberOfSections and lengthOfSection are static and shared, so every live scene lays
	/// itself out against the *current* song's values. That is the original's behaviour and
	/// it is what makes the ghosts scroll at the active song's speed rather than their own.
	/// </summary>
	public class Scene : FlashObject
	{
		public static float lengthOfSection;
		public static float PenisHeightMax;
		public static float LineYRatioToScene = 0.3f;
		public static int numberOfSections = 5;
		public static int finishingScoreTableConst = 3000;

		public List<Beat> trackArray;
		public int timeAfterTrackArrayFinishes;
		public TextTalha textScore;
		public TextTalha textRating;
		public TextTalha textLyric;
		public TextTalha textCombo;
		public List<Spike> spikes;
		public int shakingPenisCounter = 0;
		public int shakingHeadCounter = 0;
		public int shakingHandCounter = 0;
		public float shakeFactor = 2f;
		public int score = 0;
		public int pressingSpikeScoreCounter = 0;
		public int pressingSpikeScoreConstant = 100;
		public Spike pressingSpike;
		public int pointerForSpikeCreation = 0;
		public float penisHeight = 36f;
		public Citmap penis;
		public int particlePoolPointer = 0;
		public int particlePoolMaxLength = 100;
		public List<Particle> particlePool;
		public float panning;
		public bool oldKeyDown = false;
		public int lyricsPointer = 0;
		public List<Lyric> lyrics = new List<Lyric>();
		public List<FlashBitmap> lines;
		public float lineY;
		public bool isReplay;
		public List<float> inputSaver;
		public int id;
		public int headRotationTimeCurrent = 0;
		public float headRotationMax = 15f;
		public bool headRotatingToMax = false;
		public bool headRotating = true;
		public Citmap head;
		public Citmap hand;
		public int finishingScoreTableCounter = 0;
		public int finishingParticleCounter = 0;
		public bool finished = false;
		public float endOfSong;
		public bool destroyMePleaseMessageTaken = false;
		public bool destroyMePlease = false;
		public Citmap crosshair;
		public int comboCounter = 0;
		public uint color2;
		public uint color1;
		public List<Body> bodiesLevel6;

		public static Scene New(int id = 0, bool isReplay = false)
		{
			Scene scene = NewNode("Scene").AddComponent<Scene>();
			scene.id = id;
			scene.isReplay = isReplay;

			// Ghosts are spread across the stereo field by how far back in the game they are;
			// the active scene sits in the middle.
			scene.panning = !isReplay ? 0f : -1f + 2f * (id + 1) / (float)(GameManager.id + 1);

			scene.trackArray = SleepyCharts.getTrackArray(id);

			switch (id)
			{
				case 1:
					scene.color1 = 11158596;
					scene.color2 = 5579298;
					scene.timeAfterTrackArrayFinishes = 1000;
					break;
				default:
					scene.color1 = 11158596;
					scene.color2 = 5579298;
					break;
			}

			scene.spikes = new List<Spike>();
			scene.lines = new List<FlashBitmap>();
			lengthOfSection = (float)Main.stageWidth / numberOfSections;
			scene.lineY = Main.stageHeight * 0.3f;

			for (int i = 0; i < numberOfSections + 1; i++)
			{
				FlashBitmap line = FlashBitmap.New(SleepyAssets.GetSprite("img/scene" + id + "/line1.png"));
				scene.lines.Add(line);
				line.scaleX = GameManager.ScaleX * 8f / numberOfSections;
				line.x = i * lengthOfSection;
				line.y = scene.lineY;
				scene.addChild(line);
			}

			scene.endOfSong = scene.trackArray[scene.trackArray.Count - 1].timeInSong
				+ scene.trackArray[scene.trackArray.Count - 1].lengthOfPress;

			if (GameManager.id != 5)
			{
				scene.penis = Citmap.New(SleepyAssets.GetSprite("img/scene" + id + "/penis.png"), 8, 72);
				scene.hand = Citmap.New(SleepyAssets.GetSprite("img/scene" + id + "/hand.png"), 64, 90);
				scene.head = Citmap.New(SleepyAssets.GetSprite("img/scene" + id + "/head.png"), 45, 90);
				scene.penis.scaleX = scene.penis.scaleY = scene.hand.scaleX = scene.hand.scaleY =
					scene.head.scaleX = scene.head.scaleY = 4f;
				scene.head.x = 45f * scene.head.scaleX;
				scene.hand.x = Main.stageWidth;
				scene.penis.x = Main.stageWidth - 32f * scene.hand.scaleX;
				scene.hand.y = Main.stageHeight + 72f * scene.hand.scaleY + scene.shakeFactor;
				scene.addChild(scene.penis);
				scene.addChild(scene.head);
				scene.addChild(scene.hand);
			}
			else
			{
				// Song 6 shows five figures instead of one, borrowing the other songs'
				// artwork - which is why img/scene6 only holds the two line sprites.
				scene.bodiesLevel6 = new List<Body>();
				for (int i = 1; i < 6; i++)
				{
					scene.bodiesLevel6.Add(Body.New(i, scene.endOfSong));
					scene.addChild(scene.bodiesLevel6[i - 1]);
				}
			}

			scene.crosshair = Citmap.New(SleepyAssets.GetSprite("img/crosshair1.png"), 25, 25);
			scene.crosshair.y = scene.lineY + 3f;
			scene.crosshair.x = lengthOfSection;
			scene.addChild(scene.crosshair);

			scene.textScore = TextTalha.New("");
			scene.textScore.x = Main.stageWidth / 2f;
			scene.textScore.y = Main.stageHeight / 7f;
			scene.textScore.blinking = true;
			scene.addChild(scene.textScore);

			scene.textRating = TextTalha.New("");
			scene.textRating.alpha = 0f;
			scene.textRating.x = Main.stageWidth / 4f;
			scene.textRating.y = Main.stageHeight * 3f / 7f;
			scene.textRating.blinking = true;
			scene.addChild(scene.textRating);

			scene.textCombo = TextTalha.New("");
			scene.textCombo.x = Main.stageWidth * 3f / 4f;
			scene.textCombo.y = Main.stageHeight * 3f / 7f;
			scene.textCombo.blinking = true;
			scene.addChild(scene.textCombo);

			scene.textLyric = TextTalha.New("");
			scene.textLyric.x = Main.stageWidth / 2f;
			scene.textLyric.y = Main.stageHeight * 5f / 7f;
			scene.addChild(scene.textLyric);

			scene.particlePool = new List<Particle>();
			for (int i = 0; i < scene.particlePoolMaxLength; i++)
			{
				scene.particlePool.Add(Particle.New());
			}

			scene.inputSaver = new List<float>();
			scene.lyrics = SleepyLyrics.putInLyrics(id);
			return scene;
		}

		public void updateTexts()
		{
			textScore.text = "Score: " + score;
			textScore.Tick();
			// _temp_1.alpha in the decompiled source: the rating fades out over half a beat,
			// and textRating is the only thing here with an alpha to fade.
			textRating.alpha -= GameManager.dt / (float)GameManager.rhythm / 2f;
			textRating.Tick();

			if (comboCounter > 1)
			{
				textCombo.text = "Combo x" + comboCounter;
			}
			else
			{
				textCombo.text = "No Combo";
			}

			textCombo.Tick();
			textLyric.Tick();
		}

		public void updateParticles()
		{
			for (int i = 0; i < particlePoolMaxLength; i++)
			{
				if (particlePool[i].needsToBeKilled)
				{
					particlePool[i].kill();
					removeChild(particlePool[i]);
				}

				particlePool[i].Tick();
			}
		}

		public void updateLyrics()
		{
			if (lyrics.Count > lyricsPointer
				&& GameManager.time / (float)GameManager.rhythm >= lyrics[lyricsPointer].time)
			{
				textLyric.text = lyrics[lyricsPointer].text;
				textLyric.scaleUpCounter = 125;
				++lyricsPointer;
			}
		}

		/// <summary>
		/// The timeline scroll. The two line sprites alternate along the row and swap every
		/// frame, which is what makes the track shimmer - and the reason assets are cached
		/// rather than loaded per call.
		/// </summary>
		public void updateGraphicsTimeline()
		{
			int intoBeat = GameManager.time % GameManager.rhythm;
			float phase = intoBeat / (float)GameManager.rhythm;
			bool alternate = phase > 0.5f;

			for (int i = 0; i < numberOfSections + 1; i++)
			{
				alternate = !alternate;
				lines[i].bitmapData = alternate
					? SleepyAssets.GetSprite("img/scene" + id + "/line1.png")
					: SleepyAssets.GetSprite("img/scene" + id + "/line2.png");
				lines[i].x = i * lengthOfSection - phase * lengthOfSection;
				lines[i].y = lineY;
			}
		}

		public void updateGraphicsSpikes()
		{
			bool creating = true;
			while (creating && pointerForSpikeCreation < trackArray.Count)
			{
				if (trackArray[pointerForSpikeCreation].getRelativePosition() < GameManager.rhythm * numberOfSections)
				{
					Spike spike = Spike.New(pointerForSpikeCreation, trackArray[pointerForSpikeCreation].lengthOfPress, id);
					spikes.Add(spike);
					addChild(spike);
					++pointerForSpikeCreation;
				}
				else
				{
					creating = false;
				}
			}

			int index = 0;
			while (index < spikes.Count)
			{
				Beat beat = trackArray[spikes[index].idInTrack];

				if (!beat.isHit && beat.getRelativePosition() < -Beat.timingMax
					|| beat.lengthOfPress != 0 && !beat.isHitEnd && beat.getRelativePositionToEnd() < -Beat.timingMax)
				{
					comboCounter = 0;
				}

				if (beat.isHit && beat.getRelativePosition() < Beat.timingMax)
				{
					spikes[index].firstSpike.scaleX = spikes[index].firstSpike.scaleY =
						300f * (1f - Mathf.Abs(beat.getRelativePosition()) / (float)Beat.timingMax) / 100f;
					if (spikes[index].firstSpike.scaleX < 0f)
					{
						spikes[index].firstSpike.scaleX = spikes[index].firstSpike.scaleY = 0f;
					}
				}

				if (beat.lengthOfPress != 0 && beat.isHitEnd && beat.getRelativePositionToEnd() < Beat.timingMax)
				{
					spikes[index].secondSpike.scaleX = spikes[index].secondSpike.scaleY =
						300f * (1f - Mathf.Abs(beat.getRelativePositionToEnd()) / (float)Beat.timingMax) / 100f;
					if (spikes[index].secondSpike.scaleX < 0f)
					{
						spikes[index].secondSpike.scaleX = spikes[index].secondSpike.scaleY = 0f;
					}
				}

				if (spikes[index] == pressingSpike)
				{
					spikes[index].line.scaleY = 2f * (beat.getRelativePositionToEnd() / (float)beat.lengthOfPress);
					pressingSpikeScoreCounter += GameManager.dt;
					if (pressingSpikeScoreCounter >= pressingSpikeScoreConstant)
					{
						pressingSpikeScoreCounter = 0;
						addScore(1);
					}
				}
				else if (beat.isHit && beat.isHitEnd)
				{
					if (beat.lengthOfPress != 0)
					{
						spikes[index].line.scaleY = 0f;
					}
				}
				else if (beat.lengthOfPress != 0)
				{
					spikes[index].line.scaleY = 0.5f;
				}

				spikes[index].x = lengthOfSection + lengthOfSection * (beat.getRelativePosition() / (float)GameManager.rhythm);
				spikes[index].y = lineY;

				if (spikes[index].scaleX <= 0f
					|| beat.getRelativePosition() < -2 * GameManager.rhythm && beat.getRelativePositionToEnd() < -2 * GameManager.rhythm)
				{
					removeChild(spikes[index]);
					Destroy(spikes[index].gameObject);
					spikes.RemoveAt(index);
					index--;
				}

				index++;
			}
		}

		/// <summary>
		/// _temp_1 through _temp_6 in the decompiled source are the compound assignments the
		/// decompiler lost the targets of; the counters they sit under name them - hand, then
		/// head, then penis. Body.update repeats the same block with the same order.
		/// </summary>
		public void updateGraphicsBody()
		{
			penisHeight = Mathf.Max(0f, 36f * (endOfSong - GameManager.time) / endOfSong);
			penis.y = Main.stageHeight + penisHeight * penis.scaleY + shakeFactor;
			head.y = Main.stageHeight + (36f - penisHeight) * penis.scaleY / 4f + shakeFactor;
			head.x = 45f * head.scaleX;
			penis.x = Main.stageWidth - 32f * penis.scaleX;
			penis.bitmap.alpha = GameManager.blink(penis.bitmap.alpha, 0.4f, 0.6f);
			head.bitmap.alpha = GameManager.blink(head.bitmap.alpha, 0.4f, 0.6f);

			if (headRotating)
			{
				if (headRotatingToMax)
				{
					headRotationTimeCurrent += GameManager.dt;
					if (headRotationTimeCurrent >= GameManager.rhythm / 2f)
					{
						headRotatingToMax = false;
					}
				}
				else
				{
					headRotationTimeCurrent -= GameManager.dt;
					if (headRotationTimeCurrent <= -GameManager.rhythm / 2f)
					{
						headRotatingToMax = true;
					}
				}

				head.rotation = headRotationMax * (headRotationTimeCurrent / (float)GameManager.rhythm / 2f);
			}

			if (shakingHandCounter > 0)
			{
				shakingHandCounter -= GameManager.dt;
				hand.x += -shakeFactor + 2f * shakeFactor * Random.value;
				hand.y += -shakeFactor + 2f * shakeFactor * Random.value;
			}

			if (shakingHeadCounter > 0)
			{
				shakingHeadCounter -= GameManager.dt;
				head.x += -shakeFactor + 2f * shakeFactor * Random.value;
				head.y += -shakeFactor + 2f * shakeFactor * Random.value;
			}

			if (shakingPenisCounter > 0)
			{
				shakingPenisCounter -= GameManager.dt;
				penis.x += -shakeFactor + 2f * shakeFactor * Random.value;
				penis.y += -shakeFactor + 2f * shakeFactor * Random.value;
			}
		}

		public void updateGraphicsBodiesLevel6()
		{
			float sweep = 2f * (GameManager.time % (GameManager.rhythm * 6)) / (GameManager.rhythm * 6f) - 1f;

			for (int i = 0; i < bodiesLevel6.Count; i++)
			{
				bodiesLevel6[i].x = Main.stageWidth / 2f + i * sweep * Main.stageWidth / 16f + (sweep + 2f) * i * 30f - 60f;
				bodiesLevel6[i].y = Main.stageHeight;
				bodiesLevel6[i].scaleX = bodiesLevel6[i].scaleY = Mathf.Abs(1f - Mathf.Abs(sweep));
				bodiesLevel6[i].Tick();
			}
		}

		public void updateGraphics()
		{
			updateGraphicsTimeline();
			updateGraphicsSpikes();
			updateLyrics();
			updateTexts();
			updateParticles();

			if (GameManager.id != 5)
			{
				updateGraphicsBody();
			}
			else
			{
				updateGraphicsBodiesLevel6();
			}

			checkIfFinished();
		}

		public Particle particle(float x, float y)
		{
			if (particlePoolPointer >= particlePoolMaxLength)
			{
				particlePoolPointer = 0;
			}

			particlePool[particlePoolPointer].reset(x, y);
			addChild(particlePool[particlePoolPointer]);
			++particlePoolPointer;
			return particlePool[particlePoolPointer - 1];
		}

		public void inputHandler(bool keyDown)
		{
			if (!oldKeyDown && keyDown)
			{
				SoundManager.playSound("explosion", !isReplay, panning);
				if (!isReplay)
				{
					inputSaver.Add(GameManager.time / (float)GameManager.rhythm);
				}

				if (GameManager.id != 5)
				{
					shakingPenisCounter = 500;
					shakingHeadCounter = 500;
					Actuate.stop(hand);
					Actuate.tween(hand, 1f,
						y: Main.stageHeight + penisHeight * hand.scaleY,
						x: Main.stageWidth + 3f * hand.scaleX * (-1f + 2f * Random.value));
				}

				crosshair.scaleX = crosshair.scaleY = 2f;

				if (pressingSpike == null)
				{
					for (int i = 0; i < spikes.Count; i++)
					{
						Beat beat = trackArray[spikes[i].idInTrack];
						// Spikes are in time order, so the first one still in the future ends
						// the search.
						if (beat.getRelativePosition() > Beat.timingMax)
						{
							break;
						}

						if (beat.hit())
						{
							addToCombo();
							changeRating(FlashMath.round(Mathf.Abs(beat.getRelativePosition())));
							if (beat.lengthOfPress != 0)
							{
								pressingSpike = spikes[i];
							}
						}
						else if (beat.getRelativePosition() < 0 && beat.getRelativePositionToEnd() > 0)
						{
							// Late to the start of a hold, but still inside it: you get to
							// hold the rest of it, without the score for hitting it.
							pressingSpike = spikes[i];
						}
					}
				}
			}
			else if (oldKeyDown && !keyDown)
			{
				SoundManager.playSound("explosion", !isReplay, panning);
				if (!isReplay)
				{
					inputSaver.Add(GameManager.time / (float)GameManager.rhythm);
				}

				if (GameManager.id != 5)
				{
					Actuate.stop(hand);
					Actuate.tween(hand, 1f,
						y: Main.stageHeight + 72f * hand.scaleY,
						x: Main.stageWidth + 3f * hand.scaleX * (-1f + 2f * Random.value));
				}

				crosshair.scaleX = crosshair.scaleY = 1f;

				if (pressingSpike != null)
				{
					Beat beat = trackArray[pressingSpike.idInTrack];
					if (beat.getRelativePositionToEnd() <= Beat.timingMax)
					{
						if (beat.hitEnd())
						{
							addToCombo();
							changeRating(FlashMath.round(Mathf.Abs(beat.getRelativePositionToEnd())));
						}
					}
				}

				pressingSpike = null;
			}
			else if (oldKeyDown && keyDown)
			{
				if (pressingSpike != null)
				{
					if (trackArray[pressingSpike.idInTrack].getRelativePositionToEnd() < -Beat.timingMax)
					{
						pressingSpike = null;
					}
				}
			}

			oldKeyDown = keyDown;
		}

		public void checkIfFinished()
		{
			if (!finished)
			{
				if (GameManager.time > endOfSong + timeAfterTrackArrayFinishes)
				{
					finished = true;
					if (GameManager.id != 5)
					{
						shakingHandCounter = 5000;
						shakingHeadCounter = 5000;
						shakingPenisCounter = 5000;
						for (int i = 0; i < 40; i++)
						{
							particle(penis.x, penis.y - 56f * penis.scaleY);
						}
					}
					else
					{
						for (int i = 0; i < bodiesLevel6.Count; i++)
						{
							bodiesLevel6[i].shakingHandCounter = 5000;
							bodiesLevel6[i].shakingHeadCounter = 5000;
							bodiesLevel6[i].shakingPenisCounter = 5000;
							for (int j = 0; j < 20; j++)
							{
								particle(
									bodiesLevel6[i].x + bodiesLevel6[i].penis.x * bodiesLevel6[i].scaleX,
									bodiesLevel6[i].y + (bodiesLevel6[i].penis.y - 56f * bodiesLevel6[i].penis.scaleY) * bodiesLevel6[i].scaleY);
							}
						}
					}
				}
			}
			else
			{
				finishingParticleCounter += GameManager.dt;
				if (finishingParticleCounter > 25)
				{
					if (GameManager.id != 5)
					{
						particle(penis.x, penis.y - 56f * penis.scaleY);
					}
					else
					{
						for (int i = 0; i < bodiesLevel6.Count; i++)
						{
							particle(
								bodiesLevel6[i].x + bodiesLevel6[i].penis.x * bodiesLevel6[i].scaleX,
								bodiesLevel6[i].y + (bodiesLevel6[i].penis.y - 56f * bodiesLevel6[i].penis.scaleY) * bodiesLevel6[i].scaleY);
						}
					}

					finishingParticleCounter = 0;
				}

				finishingScoreTableCounter += GameManager.dt;
				if (finishingScoreTableCounter > 3000)
				{
					destroyMePlease = true;
					// scores[id - 1], because Scene.id is 1-based. ScoreTable reads
					// scores[GameManager.id] instead, which is 0-based - the two genuinely
					// disagree in the original. Don't "fix" one into the other.
					if (score > SceneManager.scores[id - 1])
					{
						SceneManager.scores[id - 1] = score;
						SceneManager.replayInputs[id - 1] = inputSaver;
						SaveManager.save();
					}
				}
			}
		}

		/// <summary>
		/// Scores twice on purpose: the rating's own value, and then a flat 10 on every hit
		/// regardless of accuracy. ScoreTable.myScores and ratioOfStars were tuned against
		/// the doubled totals, so removing it would silently re-tune every star threshold in
		/// the game.
		/// </summary>
		public void changeRating(int offBy)
		{
			if (offBy <= Beat.timingMax / 10)
			{
				textRating.text = "Perfect!";
				addScore(10);
				SoundManager.playSound("perfect", !isReplay, panning);
			}
			else if (offBy <= Beat.timingMax * 3 / 10)
			{
				textRating.text = "Great!";
				addScore(5);
				SoundManager.playSound("great", !isReplay, panning);
			}
			else if (offBy <= Beat.timingMax * 5 / 10)
			{
				textRating.text = "Good";
				addScore(2);
				SoundManager.playSound("good", !isReplay, panning);
			}
			else if (offBy <= Beat.timingMax * 7 / 10)
			{
				textRating.text = "OK";
				addScore(1);
				SoundManager.playSound("ok", !isReplay, panning);
			}
			else
			{
				textRating.text = "Sad :(";
				SoundManager.playSound("sad", !isReplay, panning);
			}

			addScore(10);
		}

		public void addToCombo()
		{
			++comboCounter;
			if (comboCounter > 1)
			{
				textCombo.scaleUpCounter = FlashMath.round(Mathf.Min(1250f, 750f + (comboCounter - 2) * 1000f / 20f));
			}
		}

		public void addScore(int amount)
		{
			score += FlashMath.round(Mathf.Max(amount, amount * comboCounter / 10f));
			textScore.scaleUpCounter = 1000;
			textRating.alpha = 1f;
			textRating.scaleUpCounter = 750;

			for (int i = -amount; i < amount + 1; i++)
			{
				particle(
					textScore.x + i * 100f / amount - 4f + 8f * Random.value,
					textScore.y - 4f + 8f * Random.value);
			}

			for (int i = 0; i < amount; i++)
			{
				particle(crosshair.x - 4f + 8f * Random.value, crosshair.y - 4f + 8f * Random.value);
			}
		}
	}
}
