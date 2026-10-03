using System.Collections.Generic;
using static Games.GarbagePeople.Globals;

namespace Games.GarbagePeople
{
	/// <summary>
	/// The second level: smash five signs, five radios and five cars on grey ground. Ported
	/// from city.js (the parts all levels share are in Level).
	/// </summary>
	public class City : Level
	{
		private List<Sprite> signs;
		private int signCount;
		private int signGoal;
		private List<Sprite> radios;
		private int radioCount;
		private int radioGoal;
		private List<Sprite> cars;
		private int carCount;
		private int carGoal;

		public City() : base("city")
		{
		}

		protected override string musicKey => "City";

		protected override void createWorld()
		{
			signCount = 0;
			carCount = 0;
			radioCount = 0;
			signGoal = 5;
			carGoal = 5;
			radioGoal = 5;
			add.image(width / 2, height / 2, "white").setScale(width / 16, height / 16).setTint(0x838383);

			addDetails();
			signs = new List<Sprite>();
			for (int i = 0; i < 16; i++)
			{
				Sprite sign = physics.add.sprite(PhaserMath.FloatBetween(0, width), PhaserMath.FloatBetween(0, height), "sign");
				sign.hp = 2;
				signs.Add(sign);
			}

			cars = new List<Sprite>();
			for (int i = 0; i < 16; i++)
			{
				Sprite car = physics.add.sprite(PhaserMath.FloatBetween(0, width), PhaserMath.FloatBetween(0, height), "car");
				car.hp = 5;
				cars.Add(car);
			}

			radios = new List<Sprite>();
			for (int i = 0; i < 16; i++)
			{
				Sprite radio = physics.add.sprite(PhaserMath.FloatBetween(0, width), PhaserMath.FloatBetween(0, height), "radio");
				radio.hp = 3;
				radios.Add(radio);
			}
		}

		protected override void hitThings()
		{
			for (int i = 0; i < signs.Count; i++)
			{
				Sprite thing = signs[i];
				if (isClose(player, thing, 150))
				{
					if (hit(signs, i, "junk")) signCount++;
					else thing.setTint(0xff0000);
				}
			}

			for (int i = 0; i < radios.Count; i++)
			{
				Sprite thing = radios[i];
				if (isClose(player, thing, 150))
				{
					if (hit(radios, i, "junk")) radioCount++;
					else if (thing.hp > 1) thing.setTint(0xff8888);
					else thing.setTint(0xff0000);
				}
			}

			for (int i = 0; i < cars.Count; i++)
			{
				Sprite thing = cars[i];
				if (isClose(player, thing, 150))
				{
					if (hit(cars, i, "junk")) carCount++;
					else if (thing.hp > 3) thing.setTint(0xffcccc);
					else if (thing.hp > 2) thing.setTint(0xff8888);
					else if (thing.hp > 1) thing.setTint(0xff4444);
					else thing.setTint(0xff0000);
				}
			}
		}

		protected override void updateMissions()
		{
			if (signCount > 0 || radioCount > 0 || carCount > 0)
			{
				text.setText("Cash: " + moneyAmount + "$\n\n" + "Missions:\nSigns destroyed: " + signCount + " / " + signGoal + "\nRadios destroyed: " + radioCount + " / " + radioGoal + "\nCars destroyed: " + carCount + " / " + carGoal);
				if (signCount >= signGoal && radioCount >= radioGoal && carCount >= carGoal)
				{
					win(cityEnd);
				}
			}
			else
			{
				text.setText("Cash: " + moneyAmount + "$\n\n" + "Missions:\nDestroy " + signGoal + " signs.\nDestroy " + radioGoal + " radios.\nDestroy " + carGoal + " cars.");
			}
		}

		private void cityEnd()
		{
			dialogueLines = new[]
			{
				"My little destroyer! My garbage person!",
				"Did I just destroy people's cars?",
				"Yes, that happened.",
				"Well I wish that didn't happen!",
				"Well, it already happened. Move on.",
				"At least the cash is good I guess.",
				"My garbage person likes cash.",
				"Please don't call me that...",
				"That is who you are! Be proud!",
				"Why?",
				"Because. There are a lot of garbage people\non earth that do not know that they are garbage.\nYou know you are garbage.\nThat makes you a good person.",
				"Really?",
				"Yeah I mean that is my theory at the moment.",
				"Good to know I guess.",
				"Ready for one last mission for today?",
				"Please don't be private property...",
				"We are going to destroy houses.",
				"Of course...",
				"Let's go!",
			};
			dialogueNextScene = "road";
			scene.start("store");
		}
	}
}
