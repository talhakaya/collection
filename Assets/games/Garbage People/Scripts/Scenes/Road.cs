using System.Collections.Generic;
using static Games.GarbagePeople.Globals;

namespace Games.GarbagePeople
{
	/// <summary>
	/// The last level: smash fifteen buildings, ten hits each, on dark ground. Ported from
	/// road.js (the parts all levels share are in Level). Afterwards the last dialogue, and
	/// the title screen thanks you for playing.
	/// </summary>
	public class Road : Level
	{
		private List<Sprite> buildings;
		private int buildingCount;
		private int buildingGoal;

		public Road() : base("road")
		{
		}

		protected override string musicKey => "Skyscraper";

		protected override void createWorld()
		{
			buildingCount = 0;
			buildingGoal = 15;
			add.image(width / 2, height / 2, "white").setScale(width / 16, height / 16).setTint(0x525252);

			addDetails();
			buildings = new List<Sprite>();
			for (int i = 0; i < 32; i++)
			{
				Sprite building = physics.add.sprite(PhaserMath.FloatBetween(0, width), PhaserMath.FloatBetween(0, height), "building" + PhaserMath.Between(0, 3));
				building.hp = 10;
				buildings.Add(building);
			}
		}

		protected override void hitThings()
		{
			for (int i = 0; i < buildings.Count; i++)
			{
				Sprite thing = buildings[i];
				if (isClose(player, thing, 300))
				{
					if (hit(buildings, i, "mess")) buildingCount++;
					else if (thing.hp > 8) thing.setTint(0xffeeee);
					else if (thing.hp > 7) thing.setTint(0xffdddd);
					else if (thing.hp > 6) thing.setTint(0xffcccc);
					else if (thing.hp > 5) thing.setTint(0xffaaaa);
					else if (thing.hp > 4) thing.setTint(0xff9999);
					else if (thing.hp > 3) thing.setTint(0xff6666);
					else if (thing.hp > 2) thing.setTint(0xff4444);
					else if (thing.hp > 1) thing.setTint(0xff2222);
					else thing.setTint(0xff0000);
				}
			}
		}

		protected override void updateMissions()
		{
			if (buildingCount > 0)
			{
				text.setText("Cash: " + moneyAmount + "$\n\n" + "Missions:\nBuildings destroyed: " + buildingCount + " / " + buildingGoal);
				if (buildingCount >= buildingGoal)
				{
					win(roadEnd);
				}
			}
			else
			{
				text.setText("Cash: " + moneyAmount + "$\n\n" + "Missions:\nDestroy " + buildingGoal + " buildings.");
			}
		}

		private void roadEnd()
		{
			isGameFinished = true;
			dialogueLines = new[]
			{
				"You must be so tired!",
				"You said we were going to destroy some garbage.",
				"Yes.",
				"We have destroyed everything BUT garbage!",
				"Yes, that happened.",
				"I shouldn't have trusted you in the first place.",
				"If you didn't do what I said\nI would have crushed you with my hand.",
				"Okay okay, I understand the power dynamics here.",
				"But aren't you proud?\nWe have achieved so much!",
				"Not sure I would call it an achievement.",
				"You have made some cash.",
				"Indeed I did.",
				"You have made a friend.",
				"Friend?",
				"Of course! I am your friend.",
				"Okay.",
				"Okay!?",
				"Just please don't kill me.",
				"No promises.",
			};
			dialogueNextScene = "loading";
			scene.start("dialogue");
		}
	}
}
