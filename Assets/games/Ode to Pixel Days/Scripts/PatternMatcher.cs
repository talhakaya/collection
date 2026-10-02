using System.Collections.Generic;

namespace Games.OdeToPixelDays
{
	/// <summary>
	/// The boss fight's memory game. Ported from PatternMatcher.as.
	///
	/// It shows a pattern on the four lights - one light the first time, six by the end -
	/// then waits for the player to repeat it on the levers. The level reads the flags it
	/// raises: right answer (drop crates on the boss, send monsters), wrong answer (the boss
	/// is let out), time to answer.
	///
	/// Invisible; a sprite only so that the level updates it.
	/// </summary>
	public class PatternMatcher : FlxSprite
	{
		private static string Sfxdogru = "PatternMatcher_Sfxdogru";
		private static string Sfxyanlis = "PatternMatcher_Sfxyanlis";

		private bool pattern1Complete;
		private bool pattern2Complete;
		private bool pattern3Complete;
		private bool pattern4Complete;
		private bool pattern5Complete;
		private bool pattern6Complete;
		private const double TIME = 1.5;
		private FlxTimer timer1;
		private FlxTimer timer2;
		private FlxTimer timer3;
		private FlxTimer timer4;
		private FlxTimer timer5;
		private FlxTimer timer6;
		private FlxTimer timer7;
		private FlxTimer timer8;
		private Lightbulb lightbulb1;
		private Lightbulb lightbulb2;
		private Lightbulb lightbulb3;
		private Lightbulb lightbulb4;
		public bool asking;
		public bool answering;
		public bool timer7set;
		private List<int> patternAsked;
		private List<int> answer;
		private bool rightAnswerGiven;
		public bool kutuFirlat;
		public bool canavarGonder;
		public bool yanlisBildin;
		public bool dogruBildin;
		public bool cevapver;
		private MonsterHans boss;

		public PatternMatcher(bool _1, bool _2, bool _3, bool _4, bool _5, bool _6, Lightbulb _bulb1, Lightbulb _bulb2, Lightbulb _bulb3, Lightbulb _bulb4, MonsterHans _b)
		{
			alpha = 0;
			pattern1Complete = _1;
			pattern2Complete = _2;
			pattern3Complete = _3;
			pattern4Complete = _4;
			pattern5Complete = _5;
			pattern6Complete = _6;
			lightbulb1 = _bulb1;
			lightbulb2 = _bulb2;
			lightbulb3 = _bulb3;
			lightbulb4 = _bulb4;
			boss = _b;
			timer1 = new FlxTimer();
			timer2 = new FlxTimer();
			timer3 = new FlxTimer();
			timer4 = new FlxTimer();
			timer5 = new FlxTimer();
			timer6 = new FlxTimer();
			timer7 = new FlxTimer();
			timer8 = new FlxTimer();
			timer7set = false;
			asking = false;
			answering = false;
			yanlisBildin = false;
			dogruBildin = false;
			canavarGonder = false;
			cevapver = false;
			answer = new List<int>();
			patternAsked = new List<int>();
		}

		public override void update()
		{
			base.update();
			if (!asking && answering)
			{
				if (lightbulb1.justOpened)
				{
					answer.Add(1);
				}
				else if (lightbulb2.justOpened)
				{
					answer.Add(2);
				}
				else if (lightbulb3.justOpened)
				{
					answer.Add(3);
				}
				else if (lightbulb4.justOpened)
				{
					answer.Add(4);
				}

				if (answer.Count > 0 && answer.Count <= patternAsked.Count)
				{
					timer8.start(TIME, 1, checkAnswer);
					if (answer.Count == patternAsked.Count)
					{
						answering = false;
					}
				}
			}
		}

		private void checkAnswer(FlxTimer _t)
		{
			int i = 0;
			rightAnswerGiven = true;
			for (i = 0; i < answer.Count; i++)
			{
				if (answer[i] != patternAsked[i])
				{
					rightAnswerGiven = false;
				}
			}

			if (rightAnswerGiven)
			{
				if (answer.Count == patternAsked.Count)
				{
					FlxG.play(Sfxdogru);
					dogruBildin = true;
					canavarGonder = true;
					answering = false;
					kutuFirlat = true;
					if (!pattern1Complete)
					{
						pattern1Complete = true;
					}
					else if (!pattern2Complete)
					{
						pattern2Complete = true;
					}
					else if (!pattern3Complete)
					{
						pattern3Complete = true;
					}
					else if (!pattern4Complete)
					{
						pattern4Complete = true;
					}
					else if (!pattern5Complete)
					{
						pattern5Complete = true;
					}
					else if (!pattern6Complete)
					{
						pattern6Complete = true;
						canavarGonder = false;
					}

					timer8 = null;
					timer8 = new FlxTimer();
					timer8.start(TIME * 1, 1, goOn);
					answer = new List<int>();
				}
			}
			else
			{
				FlxG.play(Sfxyanlis);
				yanlisBildin = true;
				answering = false;
				answer = new List<int>();
			}
		}

		private void goOn(FlxTimer _t)
		{
			start();
		}

		public void start()
		{
			if (!asking)
			{
				asking = true;
				answering = false;
				if (!pattern1Complete)
				{
					pattern(patternAsked = new List<int> { 2 });
				}
				else if (!pattern2Complete)
				{
					pattern(patternAsked = new List<int> { 1, 3 });
				}
				else if (!pattern3Complete)
				{
					pattern(patternAsked = new List<int> { 2, 3, 4 });
				}
				else if (!pattern4Complete)
				{
					pattern(patternAsked = new List<int> { 1, 3, 2, 4 });
				}
				else if (!pattern5Complete)
				{
					pattern(patternAsked = new List<int> { 1, 2, 1, 3, 4 });
				}
				else if (!pattern6Complete)
				{
					pattern(patternAsked = new List<int> { 3, 4, 1, 2, 4, 2 });
				}
			}
		}

		private void pattern(List<int> _array)
		{
			timer7set = false;
			if (_array.Count > 0)
			{
				if (_array[0] == 1)
				{
					timer1.start(TIME, 1, light1);
				}
				else if (_array[0] == 2)
				{
					timer1.start(TIME, 1, light2);
				}
				else if (_array[0] == 3)
				{
					timer1.start(TIME, 1, light3);
				}
				else if (_array[0] == 4)
				{
					timer1.start(TIME, 1, light4);
				}
			}
			else if (!timer7set)
			{
				timer7set = true;
				timer7.start(1, 1, asked);
			}

			if (_array.Count > 1)
			{
				if (_array[1] == 1)
				{
					timer2.start(TIME * 2, 1, light1);
				}
				else if (_array[1] == 2)
				{
					timer2.start(TIME * 2, 1, light2);
				}
				else if (_array[1] == 3)
				{
					timer2.start(TIME * 2, 1, light3);
				}
				else if (_array[1] == 4)
				{
					timer2.start(TIME * 2, 1, light4);
				}
			}
			else if (!timer7set)
			{
				timer7set = true;
				timer7.start(TIME + 1, 1, asked);
			}

			if (_array.Count > 2)
			{
				if (_array[2] == 1)
				{
					timer3.start(TIME * 3, 1, light1);
				}
				else if (_array[2] == 2)
				{
					timer3.start(TIME * 3, 1, light2);
				}
				else if (_array[2] == 3)
				{
					timer3.start(TIME * 3, 1, light3);
				}
				else if (_array[2] == 4)
				{
					timer3.start(TIME * 3, 1, light4);
				}
			}
			else if (!timer7set)
			{
				timer7set = true;
				timer7.start(TIME * 2 + 1, 1, asked);
			}

			if (_array.Count > 3)
			{
				if (_array[3] == 1)
				{
					timer4.start(TIME * 4, 1, light1);
				}
				else if (_array[3] == 2)
				{
					timer4.start(TIME * 4, 1, light2);
				}
				else if (_array[3] == 3)
				{
					timer4.start(TIME * 4, 1, light3);
				}
				else if (_array[3] == 4)
				{
					timer4.start(TIME * 4, 1, light4);
				}
			}
			else if (!timer7set)
			{
				timer7set = true;
				timer7.start(TIME * 3 + 1, 1, asked);
			}

			if (_array.Count > 4)
			{
				if (_array[4] == 1)
				{
					timer5.start(TIME * 5, 1, light1);
				}
				else if (_array[4] == 2)
				{
					timer5.start(TIME * 5, 1, light2);
				}
				else if (_array[4] == 3)
				{
					timer5.start(TIME * 5, 1, light3);
				}
				else if (_array[4] == 4)
				{
					timer5.start(TIME * 5, 1, light4);
				}
			}
			else if (!timer7set)
			{
				timer7set = true;
				timer7.start(TIME * 4 + 1, 1, asked);
			}

			if (_array.Count > 5)
			{
				if (_array[5] == 1)
				{
					timer6.start(TIME * 6, 1, light1);
				}
				else if (_array[5] == 2)
				{
					timer6.start(TIME * 6, 1, light2);
				}
				else if (_array[5] == 3)
				{
					timer6.start(TIME * 6, 1, light3);
				}
				else if (_array[5] == 4)
				{
					timer6.start(TIME * 5, 1, light4);
				}
			}
			else if (!timer7set)
			{
				timer7set = true;
				timer7.start(TIME * 5 + 1, 1, asked);
			}

			if (!timer7set)
			{
				timer7set = true;
				timer7.start(TIME * 6 + 1, 1, asked);
			}
		}

		private void light1(FlxTimer _t)
		{
			lightbulb1.light();
		}

		private void light2(FlxTimer _t)
		{
			lightbulb2.light();
		}

		private void light3(FlxTimer _t)
		{
			lightbulb3.light();
		}

		private void light4(FlxTimer _t)
		{
			lightbulb4.light();
		}

		private void asked(FlxTimer _t)
		{
			asking = false;
			answering = true;
			cevapver = true;
		}
	}
}
