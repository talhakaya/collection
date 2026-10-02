using System.Collections.Generic;
using static Games.GarbagePeople.Globals;

namespace Games.GarbagePeople
{
	/// <summary>
	/// The first level: smash five rocks and five trees in a yellow field. Ported from
	/// forest.js (the parts all levels share are in Level).
	/// </summary>
	public class Forest : Level
	{
		private List<Sprite> rocks;
		private List<Sprite> trees;
		private int treeCount;
		private int rockCount;
		private int treeGoal;
		private int rockGoal;

		public Forest() : base("forest")
		{
		}

		protected override string musicKey => "Forest";

		protected override void createWorld()
		{
			treeCount = 0;
			rockCount = 0;
			treeGoal = 5;
			rockGoal = 5;
			add.image(width / 2, height / 2, "yellow").setScale(width / 16, height / 16);

			addDetails();
			trees = new List<Sprite>();
			for (int i = 0; i < 16; i++)
			{
				Sprite tree = physics.add.sprite(PhaserMath.FloatBetween(0, width), PhaserMath.FloatBetween(0, height), "tree" + PhaserMath.Between(0, 2));
				tree.hp = 3;
				trees.Add(tree);
			}

			rocks = new List<Sprite>();
			for (int i = 0; i < 16; i++)
			{
				Sprite rock = physics.add.sprite(PhaserMath.FloatBetween(0, width), PhaserMath.FloatBetween(0, height), "rock" + PhaserMath.Between(0, 2));
				rock.hp = 3;
				rocks.Add(rock);
			}
		}

		protected override void hitThings()
		{
			for (int i = 0; i < rocks.Count; i++)
			{
				Sprite thing = rocks[i];
				if (isClose(player, thing, 150))
				{
					if (hit(rocks, i, "dirt")) rockCount++;
					else if (thing.hp > 1) thing.setTint(0xff8888);
					else thing.setTint(0xff0000);
				}
			}

			for (int i = 0; i < trees.Count; i++)
			{
				Sprite thing = trees[i];
				if (isClose(player, thing, 150))
				{
					if (hit(trees, i, "trunk")) treeCount++;
					else if (thing.hp > 1) thing.setTint(0xff8888);
					else thing.setTint(0xff0000);
				}
			}
		}

		protected override void updateMissions()
		{
			if (rockCount > 0 || treeCount > 0)
			{
				text.setText("Cash: " + moneyAmount + "$\n\n" + "Missions:\nRocks destroyed: " + rockCount + " / " + rockGoal + "\nTrees destroyed: " + treeCount + " / " + treeGoal);
				if (rockCount >= rockGoal && treeCount >= treeGoal)
				{
					win(forestEnd);
				}
			}
			else
			{
				text.setText("Cash: " + moneyAmount + "$\n\n" + "Missions:\nDestroy " + rockGoal + " rocks.\nDestroy " + treeGoal + " trees.");
			}
		}

		private void forestEnd()
		{
			dialogueLines = new[]
			{
				"You did a great job!",
				"I destroyed nature!",
				"Yes, that happened.",
				"Why did I do that?",
				"You had no option. I told you to do so.",
				"Why did I do what you told me?",
				"Because I am a creepy giant and you are scared.",
				"Oh okay.",
				"Also you are making crazy cash, dude.",
				"I do like that.",
				"Don't worry about the trees.\nWe will destroy some garbage now.",
				"That sounds better.",
				"...",
				"Hey, can I get dressed now?",
				"NO!",
				"Why?",
				"Garbage people don't get dressed. They only destroy.",
				"Don't call me a garbage person! Sounds bad!",
				"Nothing bad about being a garbage person.",
				"Yeah?",
				"Yeah! Be proud of your garbageness.",
				"Why?",
				"I will explain later.",
				"...",
				"Let's destroy!",
			};
			dialogueNextScene = "city";
			scene.start("store");
		}
	}
}
