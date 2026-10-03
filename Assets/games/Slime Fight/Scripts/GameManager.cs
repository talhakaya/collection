using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.SlimeFight
{
	public class GameManager : MonoBehaviour {

		public static float shakePeriod = 0.1f;
		public static Transform game;
		public Transform gameplay;
		public GameObject cursor;
		public static float XMax = 8.8f;
		public static float YMax = 4.95f;
		public static float MaxFireWait = 3f;
		public static float MinFireWait = 0.5f;
		public float fireTimeCounter = 0f;
		public float fireAvailableTimeCounter = 0f;
		public bool firing;
		public static float totalSlimeSize = 10f;
		public static int slimeSizeCounter = 10;
		public AudioSource audioSource;
		public static bool nPressed;

		void Start ()
		{
			Cutscene.instance.startCutscene (0);
			game = gameplay;
			AudioListener.volume = 5;
		}

		// In the collection: the game turns the global volume up to 5; it is put back when
		// the game is left.
		void OnDestroy ()
		{
			AudioListener.volume = 1;
		}

		void Update ()
		{
			// In the collection: the Escape-quit is gone (the collection has its own exit).

			if (Collection.Controls.PortHelpers.KeyDown(UnityEngine.InputSystem.Key.N))
			{
				nPressed = !nPressed;
			}

			cursor.transform.position = Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
			if (!Cutscene.instance.isActive)
			{
				DoubleSprite.ShakeConst -= Time.deltaTime;
				if (DoubleSprite.ShakeConst < DoubleSprite.MinShakeConst)
				{
					DoubleSprite.ShakeConst = DoubleSprite.MinShakeConst;
				}
				else if (DoubleSprite.ShakeConst > DoubleSprite.MaxShakeConst)
				{
					DoubleSprite.ShakeConst = DoubleSprite.MaxShakeConst;
				}

				fireAvailableTimeCounter += Time.deltaTime;
				if (TaloketoInputManager.GetMouseButtonDown(0))
				{
					firing = true;
					fireTimeCounter = 0f;
					audioSource.Play ();
				}
				if (TaloketoInputManager.GetMouseButtonUp(0))
				{
					audioSource.Stop ();
					firing = false;
					if (fireAvailableTimeCounter > MinFireWait)
					{
						Explosion.create(0.2f + fireTimeCounter).transform.position = new Vector3(cursor.transform.position.x, cursor.transform.position.y, 0f);
						fireAvailableTimeCounter = 0f;
					}
					DoubleSprite.ShakeConst += (DoubleSprite.MaxShakeConst - DoubleSprite.MinShakeConst) * fireTimeCounter / MaxFireWait;
					fireTimeCounter = 0f;
				}
				if (firing)
				{
					fireTimeCounter += Time.deltaTime;
					if (fireTimeCounter > MaxFireWait)
					{
						fireTimeCounter = MaxFireWait;
					}
					cursor.transform.localScale = new Vector3(1f, 1f, 1f) * (1f + fireTimeCounter * 3f);
				}
				else
				{
					cursor.transform.localScale = new Vector3(1f, 1f, 1f);
				}

				if (slimeSizeCounter == 0)
				{
					if (totalSlimeSize < 1f)
					{
						DoubleSprite.ShakeConst = DoubleSprite.MaxShakeConst;
						Cutscene.instance.startCutscene(1);
					}

					totalSlimeSize = 0f;
				}
				else
				{
					slimeSizeCounter--;
				}
			}
		}

		public static void startGame()
		{
			for (int i = 0; i < 10; i++)
			{
				Slime.create(Random.Range (1f, 2f)).transform.position = randomPosition();
				Slime.create(Random.Range (0.4f, 1f)).transform.position = randomPosition();
				Human.create(Random.Range (0.5f, 1.5f)).transform.position = randomPosition();
				Human.create(Random.Range (0.5f, 1.5f)).transform.position = randomPosition();
			}
			Slime.create(5f).transform.position = Vector3.zero;
		}

		public static Vector3 randomPosition()
		{
			return new Vector3 (Random.Range (-XMax, XMax), Random.Range (-YMax, YMax), 0f);
		}
	}
}
