using System.Collections.Generic;

namespace Games.SleepyTime
{
	/// <summary>
	/// The song select. Ported from Menu.as.
	///
	/// Songs unlock one at a time: the first loop counts how many are reachable by walking
	/// forward until it finds one with no stars, and that count is what the buttons are
	/// spaced across - so the row widens as the game is beaten. The second loop lays them out
	/// and stops at the same place.
	///
	/// Note both loops break *after* the unstarred song, so the next locked one is always
	/// shown and playable. That is what lets a new save reach song 1 at all.
	/// </summary>
	public class Menu : FlashObject
	{
		public TextTalha textTitle;
		public TextTalha textDevelopedBy;
		public List<ScoreStar> scoreStars = new List<ScoreStar>();
		public Citmap mainIcon;
		public Citmap kayabrosLogo;
		public bool destroyMePlease = false;

		public static Menu New()
		{
			Menu menu = NewNode("Menu").AddComponent<Menu>();

			menu.mainIcon = Citmap.New(SleepyAssets.GetSprite("img/sleepy time.png"), 50, 38);
			menu.mainIcon.scaleX = menu.mainIcon.scaleY = 2f;
			menu.mainIcon.x = Main.stageWidth / 2f;
			menu.mainIcon.y = Main.stageHeight / 4f;
			menu.addChild(menu.mainIcon);

			menu.kayabrosLogo = Citmap.New(SleepyAssets.GetSprite("img/kayabros.png"), 160, 173);
			menu.kayabrosLogo.scaleX = menu.kayabrosLogo.scaleY = 0.25f;
			menu.kayabrosLogo.x = Main.stageWidth * 9f / 32f;
			menu.kayabrosLogo.y = Main.stageHeight * 7f / 8f;
			menu.addChild(menu.kayabrosLogo);

			menu.textDevelopedBy = TextTalha.New("Made By");
			menu.textDevelopedBy.x = Main.stageWidth / 7f;
			menu.textDevelopedBy.y = Main.stageHeight * 7f / 8f;
			menu.addChild(menu.textDevelopedBy);

			menu.textTitle = TextTalha.New("Sleepy Time");
			menu.textTitle.x = Main.stageWidth / 2f;
			menu.textTitle.y = Main.stageHeight / 8f;
			menu.addChild(menu.textTitle);

			Button twitter = Button.New("Twitter");
			twitter.x = Main.stageWidth / 2f;
			twitter.y = Main.stageHeight * 7f / 8f;
			menu.addChild(twitter);

			Button soundtrack = Button.New("Soundtrack");
			soundtrack.x = Main.stageWidth * 3f / 4f;
			soundtrack.y = Main.stageHeight * 7f / 8f;
			menu.addChild(soundtrack);

			Button tutorial = Button.New("Tutorial");
			tutorial.x = Main.stageWidth * 13f / 16f;
			tutorial.y = Main.stageHeight * 4f / 8f;
			menu.addChild(tutorial);

			Button fullscreen = Button.New("Fullscreen");
			fullscreen.x = Main.stageWidth * 3f / 16f;
			fullscreen.y = Main.stageHeight * 4f / 8f;
			menu.addChild(fullscreen);

			int unlocked = 0;
			for (int i = 0; i < SceneManager.scores.Length; i++)
			{
				unlocked++;
				if (ScoreTable.howManyStars(i) == 0)
				{
					break;
				}
			}

			for (int i = 0; i < SceneManager.scores.Length; i++)
			{
				int stars = ScoreTable.howManyStars(i);

				Button song = Button.New("Song " + (i + 1));
				song.x = (i + 0.5f) * Main.stageWidth / unlocked;
				song.y = Main.stageHeight * 5f / 8f;
				menu.addChild(song);

				if (SceneManager.scores[i] != 0)
				{
					int remaining = stars;
					for (int slot = 0; slot < 5; slot++)
					{
						ScoreStar star = ScoreStar.New(remaining > 0);
						star.maxScale = 0.2f;
						star.x = song.x + 20f * (slot - 2);
						star.y = song.y + 30f;
						menu.addChild(star);
						menu.scoreStars.Add(star);
						remaining--;
					}
				}

				if (stars == 0)
				{
					break;
				}
			}

			return menu;
		}

		public void Tick()
		{
			mainIcon.Tick();
			kayabrosLogo.Tick();
			// Pinned every frame, so the title never settles out of its punched-up scale.
			textTitle.scaleUpCounter = 2000;
			textTitle.Tick();
			textDevelopedBy.Tick();

			for (int i = 0; i < scoreStars.Count; i++)
			{
				scoreStars[i].Tick();
			}
		}
	}
}
