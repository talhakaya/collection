using UnityEngine;
using System.Collections;

namespace Games.TroubledFootball
{
	public class Body : MonoBehaviour {

		public Sprite happy_normal;
		public Sprite happy_echo;
		public Sprite happy_wahwah;
		public Sprite goal_normal;
		public Sprite goal_echo;
		public Sprite goal_wahwah;
		public Sprite sad_normal;
		public Sprite sad_echo;
		public Sprite sad_wahwah;
		public Sprite nasty_normal;
		public Sprite nasty_echo;
		public Sprite nasty_wahwah;

		private int oldState = 50;

		public TintScript head;

		void Start ()
		{

		}

		void Update ()
		{
			if (oldState != GameManager.gameState)
			{
				if (GameManager.gameState == 0)
				{
					if (Random.Range(0f, 2f) >= 1f)
					{
						head.spriteChange (happy_normal, happy_echo, happy_wahwah);
					}
					else
					{
						head.spriteChange (nasty_normal, nasty_echo, nasty_wahwah);
					}
				}
				else if (GameManager.gameState == 1)
				{
					head.spriteChange (goal_normal, goal_echo, goal_wahwah);
				}
				else if (GameManager.gameState == 2)
				{
					head.spriteChange (sad_normal, sad_echo, sad_wahwah);
				}
			}


			oldState = GameManager.gameState;
		}
	}
}
