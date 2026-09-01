using System.Collections.Generic;
using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// The screen after a song: the score counts up, then the stars land and the buttons
	/// appear. Ported from ScoreTable.as.
	///
	/// myScores are the author's own scores for the six songs, and ratioOfStars the fractions
	/// of them each star is worth. They were tuned against the doubled totals that
	/// Scene.changeRating produces - see the note there - so neither table means anything on
	/// its own.
	///
	/// Holding the button while the count-up runs multiplies its speed by ten. That and
	/// confirming a button never overlap: before starsCreated there is nothing to confirm,
	/// and after it the count-up has already finished, so the speed-up is inert.
	/// </summary>
	public class ScoreTable : FlashObject
	{
		public static float ButtonsXBetween;
		public static float StarsXBetween;
		public static int TextScoreStartConst = 1000;
		public static int TextScoreEndConst = 3000;
		public static int TextScoreCountingConst = 5000;
		public static int TextScoreSize = 1500;
		public static float TextScoreY = 0.3f;
		public static float StarsY = 0.55f;
		public static float ButtonsY = 0.75f;

		public static int[] myScores = { 15738, 60864, 45189, 80776, 150000, 150000 };
		public static float[] ratioOfStars = { 0.09f, 0.16f, 0.25f, 0.36f, 0.49f };

		public int textScoreStartCounter = 0;
		public int textScoreScore = -5000;
		public int textScoreEndCounter = 0;
		public int textScoreCountingCounter = 0;
		public TextTalha textScore;
		public int starsGained;
		public bool starsCreated = false;
		public List<ScoreStar> stars = new List<ScoreStar>();
		public int speedUpCounting = 1;
		public int score;
		public bool destroyMePleaseMessageTaken = false;
		public bool destroyMePlease = false;
		public GriddyBackground background;

		public static ScoreTable New(int score = 0)
		{
			ScoreTable table = NewNode("ScoreTable").AddComponent<ScoreTable>();
			table.score = score;

			table.background = GriddyBackground.New();
			table.addChild(table.background);

			table.textScore = TextTalha.New("Score: ");
			table.textScore.scaleUpCounter = 1500;
			table.textScore.x = Main.stageWidth / 2f;
			table.textScore.y = Main.stageHeight * 0.3f;
			table.addChild(table.textScore);
			return table;
		}

		/// <summary>
		/// Stars earned for a song, for the menu. Indexes SceneManager.scores and myScores
		/// with the same id, unlike calculateStarsGained below.
		/// </summary>
		public static int howManyStars(int id)
		{
			int count = 0;
			for (int i = 0; i < ratioOfStars.Length; i++)
			{
				if (SceneManager.scores[id] <= myScores[id] * ratioOfStars[i])
				{
					break;
				}

				count++;
			}

			return count;
		}

		public void updateTextScore()
		{
			if (GameManager.getKeyDown())
			{
				speedUpCounting = 300;
			}
			else
			{
				speedUpCounting = 30;
			}

			if (textScoreStartCounter < 1000)
			{
				textScoreStartCounter += speedUpCounting * GameManager.dt;
				textScore.scaleUpCounter = 1500;
			}
			else if (textScoreCountingCounter < 5000)
			{
				if (textScoreScore == score)
				{
					if (!starsCreated)
					{
						starsCreated = true;
						createStars();
						createButtons();
					}

					textScore.scaleUpCounter = 1500;
					if (textScoreEndCounter < 3000)
					{
						textScoreEndCounter += GameManager.dt;
					}
				}
				else
				{
					textScoreCountingCounter += speedUpCounting * GameManager.dt;
				}
			}
			else
			{
				textScoreCountingCounter = 0;
				if (textScoreScore < score)
				{
					textScoreScore += 5000;
					textScore.scaleUpCounter = 1500;
					if (textScoreScore >= score)
					{
						textScoreScore = score;
					}
				}

				textScore.text = "Score: " + textScoreScore;
			}

			textScore.Tick();
			for (int i = 0; i < stars.Count; i++)
			{
				stars[i].Tick();
			}
		}

		public void Tick()
		{
			background.Tick();
			updateTextScore();
		}

		public void createStars()
		{
			calculateStarsGained();
			int remaining = starsGained;
			for (int i = 0; i < 5; i++)
			{
				stars.Add(ScoreStar.New(remaining > 0));
				remaining--;
				stars[i].x = Main.stageWidth / 2f + (i - 2) * 110f;
				stars[i].y = Main.stageHeight * 0.55f;
				addChild(stars[i]);
			}
		}

		public void createButtons()
		{
			Button retry = Button.New("Retry");
			retry.y = Main.stageHeight * 0.75f;
			retry.x = Main.stageWidth / 2f;
			addChild(retry);

			// Retry sits centred and alone when the song was failed; earning a star is what
			// unlocks moving on, and shoves Retry aside to make room.
			if (starsGained > 0)
			{
				retry.x -= 300f / 2f;

				Button next = Button.New(GameManager.id == 5 ? "Finish" : "Next Level");
				next.y = Main.stageHeight * 0.75f;
				next.x = Main.stageWidth / 2f + 300f / 2f;
				addChild(next);
			}
		}

		/// <summary>
		/// Indexes SceneManager.scores with GameManager.id, which is 0-based - while
		/// Scene.checkIfFinished writes the score at scores[id - 1] using Scene's 1-based id.
		/// The two genuinely disagree in the original. Left as written: don't "fix" one into
		/// the other without deciding to.
		/// </summary>
		public void calculateStarsGained()
		{
			starsGained = 0;
			float best = Mathf.Max(score, SceneManager.scores[GameManager.id]);
			for (int i = 0; i < ratioOfStars.Length; i++)
			{
				if (best <= myScores[GameManager.id] * ratioOfStars[i])
				{
					break;
				}

				++starsGained;
			}
		}
	}
}
