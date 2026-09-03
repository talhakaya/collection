using UnityEngine;

namespace Games.SleepyTime
{
	/// <summary>
	/// The animated "how to play" diagram, shown as the tutorial dialogue's character.
	/// Ported from Tutorial.as.
	///
	/// A four-second loop off GameManager.time: a single spike approaching and being tapped,
	/// then a pair joined by a line being held. The spacebar and mouse artwork swap between
	/// pressed and unpressed on the same schedule, so the diagram demonstrates the input as
	/// well as the timing.
	///
	/// Note the constructor resets GameManager.time to 0 - the loop is read straight off the
	/// game clock, so it has to start from a known point.
	/// </summary>
	public class Tutorial : Citmap
	{
		public Citmap whiteLine;
		public Citmap spike3;
		public Citmap spike2;
		public Citmap spike1;
		public Citmap spaceBar2;
		public Citmap spaceBar;
		public Citmap mouse;
		public Citmap crosshair;

		public static Tutorial New()
		{
			Tutorial tutorial = NewNode("Tutorial").AddComponent<Tutorial>();
			tutorial.Init(SleepyAssets.GetSprite("img/scene1/line1.png"), 50, 2);
			tutorial.rotating = false;
			tutorial.bitmap.scaleX = 2f;

			tutorial.spike1 = Citmap.New(SleepyAssets.GetSprite("img/spike.png"), 4, 30);
			tutorial.spike1.alpha = 0f;
			tutorial.addChild(tutorial.spike1);

			tutorial.spike2 = Citmap.New(SleepyAssets.GetSprite("img/spike.png"), 4, 30);
			tutorial.spike2.alpha = 0f;
			tutorial.addChild(tutorial.spike2);

			tutorial.spike3 = Citmap.New(SleepyAssets.GetSprite("img/spike.png"), 4, 30);
			tutorial.spike3.alpha = 0f;
			tutorial.addChild(tutorial.spike3);

			tutorial.whiteLine = Citmap.New(SleepyAssets.GetSprite("img/line.png"), 50, 8);
			tutorial.whiteLine.y = -4f;
			tutorial.whiteLine.scaleY = 0.5f;
			tutorial.whiteLine.alpha = 0f;
			tutorial.addChild(tutorial.whiteLine);

			tutorial.spaceBar = Citmap.New(SleepyAssets.GetSprite("img/tutorialSpace.png"), 0, 0);
			tutorial.spaceBar.x = -40f;
			tutorial.spaceBar.y = 40f;
			tutorial.spaceBar.scaleX = tutorial.spaceBar.scaleY = 4f;
			tutorial.spaceBar.alpha = 0f;
			tutorial.addChild(tutorial.spaceBar);

			tutorial.spaceBar2 = Citmap.New(SleepyAssets.GetSprite("img/tutorialSpace2.png"), 0, 0);
			tutorial.spaceBar2.x = -40f;
			tutorial.spaceBar2.y = 40f;
			tutorial.spaceBar2.scaleX = tutorial.spaceBar2.scaleY = 4f;
			tutorial.spaceBar2.alpha = 0f;
			tutorial.addChild(tutorial.spaceBar2);

			tutorial.mouse = Citmap.New(SleepyAssets.GetSprite("img/tutorialMouse.png"), 0, 0);
			tutorial.mouse.y = 50f;
			tutorial.mouse.scaleX = tutorial.mouse.scaleY = 4f;
			tutorial.mouse.alpha = 0f;
			tutorial.addChild(tutorial.mouse);

			tutorial.crosshair = Citmap.New(SleepyAssets.GetSprite("img/crosshair1.png"), 25, 25);
			tutorial.addChild(tutorial.crosshair);

			GameManager.time = 0;
			return tutorial;
		}

		public override void Tick()
		{
			int t = GameManager.time % 4000;
			bool pressed = false;

			if (t < 1000)
			{
				pressed = false;
				spike1.alpha = 1f;
				spike2.alpha = 0f;
				spike3.alpha = 0f;
				whiteLine.alpha = 0f;
				spike1.x = 125f - t / 8f;
				spike1.scaleX = spike1.scaleY = 1f;
				spike2.scaleX = spike2.scaleY = 1f;
				spike3.scaleX = spike3.scaleY = 1f;
				whiteLine.scaleY = 0.5f;
			}
			else if (t < 1250)
			{
				pressed = true;
				spike1.alpha = 1f;
				spike2.alpha = 0f;
				spike3.alpha = 0f;
				whiteLine.alpha = 0f;
				spike1.x = 125f - t / 8f;
				spike1.scaleX = spike1.scaleY = 4f * ((1250f - t) / 250f);
			}
			else if (t < 2000)
			{
				pressed = false;
				spike1.alpha = 0f;
				spike2.alpha = 0f;
				spike3.alpha = 0f;
				whiteLine.alpha = 0f;
			}
			else if (t < 3000)
			{
				pressed = false;
				spike1.alpha = 0f;
				spike2.alpha = 1f;
				spike2.x = 375f - t / 8f;
				spike3.alpha = 1f;
				spike3.x = 437.5f - t / 8f;
				whiteLine.alpha = 1f;
				whiteLine.x = 375f - t / 8f + 31.25f;
				whiteLine.scaleX = 0.625f;
			}
			else if (t < 3250)
			{
				pressed = true;
				spike1.alpha = 0f;
				spike2.alpha = 1f;
				spike2.x = 375f - t / 8f;
				spike2.scaleX = spike2.scaleY = 4f * ((3250f - t) / 250f);
				spike3.alpha = 1f;
				spike3.x = 437.5f - t / 8f;
				whiteLine.alpha = 1f;
				whiteLine.x = 375f - t / 8f + 31.25f;
				whiteLine.scaleY = 2f * ((3500f - t) / 500f);
				whiteLine.scaleX = 0.625f;
			}
			else if (t < 3500)
			{
				pressed = true;
				spike1.alpha = 0f;
				spike2.alpha = 0f;
				spike3.alpha = 1f;
				spike3.x = 437.5f - t / 8f;
				whiteLine.alpha = 1f;
				whiteLine.x = 375f - t / 8f + 31.25f;
				whiteLine.scaleY = 2f * ((3500f - t) / 500f);
				whiteLine.scaleX = 0.625f;
			}
			else if (t < 3750)
			{
				pressed = false;
				spike1.alpha = 0f;
				spike2.alpha = 0f;
				spike3.alpha = 1f;
				spike3.x = 437.5f - t / 8f;
				spike3.scaleX = spike3.scaleY = 4f * ((3750f - t) / 250f);
				whiteLine.alpha = 0f;
			}
			else
			{
				pressed = false;
				spike1.alpha = 0f;
				spike2.alpha = 0f;
				spike3.alpha = 0f;
				whiteLine.alpha = 0f;
			}

			if (pressed)
			{
				mouse.alpha = 1f;
				spaceBar2.alpha = 1f;
				spaceBar.alpha = 0f;
				crosshair.scaleX = crosshair.scaleY = 2f;
			}
			else
			{
				mouse.alpha = 0f;
				spaceBar2.alpha = 0f;
				spaceBar.alpha = 1f;
				crosshair.scaleX = crosshair.scaleY = 1f;
			}
		}
	}
}
