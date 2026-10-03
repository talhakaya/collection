using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Collection.Controls;

namespace Games.Herbie
{
	public class End0 : MonoBehaviour {

		public static End0 instance;
		public GameObject lips;
		public GameObject herbie;
		public GameObject mother;
		public GameObject motherFace;
		public GameObject motherDead;
		public GameObject grave;
		public GameObject theend;
		private TintScript lipsTint;
		private TintScript motherTint;
		private TintScript motherFaceTint;
		private TintScript motherDeadTint;
		private TintScript theendTint;
		public Vector3 motherLastPosition;
		private Vector3 motherStep = new Vector3(-10f, -10f, -10f);
		private float motherDeltaScale = -1;
		bool kissed = false;
		private float period = 2f;
		private float maxScale = 1.5f;
		private bool motherPlaced = false;


		void Start ()
		{
			Game.miniGame = gameObject;
			Game.isThereMiniGame = true;
			instance = this;
			motherTint = mother.GetComponent<TintScript> ();
			motherDeadTint = motherDead.GetComponent<TintScript> ();
			motherFaceTint = motherFace.GetComponent<TintScript> ();
			lipsTint = lips.GetComponent<TintScript> ();
			theendTint = theend.GetComponent<TintScript> ();
			motherDeadTint.selfColor = new Color(motherDeadTint.selfColor.r, motherDeadTint.selfColor.g, motherDeadTint.selfColor.b, 0f);
			theendTint.selfColor = new Color(theendTint.selfColor.r, theendTint.selfColor.g, theendTint.selfColor.b, 0f);
		}

		void Update ()
		{
			if (!kissed)
			{
				float ratio = (Game.time % (period * 2)) / period;
				float ratio2 = (ratio + 0.5f) % 2f;
				float scaleX = 1f;
				float scaleY = 1f;
				if (ratio < 1f)
				{
					scaleX = 1f + (maxScale - 1f) * ratio;
				}
				else
				{
					scaleX = 1f + (maxScale - 1f) * (2 - ratio);
				}
				if (ratio2 < 1f)
				{
					scaleY = 1f + (maxScale - 1f) * ratio2;
				}
				else
				{
					scaleY = 1f + (maxScale - 1f) * (2 - ratio2);
				}
				lips.transform.localScale = 10 * new Vector3(scaleX, scaleY, 1f);
				if (TaloketoInputManager.GetAxisRaw("Horizontal") != 0 || TaloketoInputManager.GetAxisRaw("Vertical") != 0)
				{
					Vector3 force = Geometry.normalizeVector2(new Vector2(TaloketoInputManager.GetAxisRaw("Horizontal"), TaloketoInputManager.GetAxisRaw("Vertical")), 1000);
					lips.GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
				}
			}
			else
			{
				if (lips.transform.localScale.y <= 100)
				{
					lips.transform.localScale += 100 * Vector3.up * Game.dt;

				}
				else
				{
					if (motherTint.selfColor.a > 0)
					{
						if (lipsTint.selfColor.a > 0)
						{
							lipsTint.selfColor = new Color(lipsTint.selfColor.r, lipsTint.selfColor.g, lipsTint.selfColor.b, lipsTint.selfColor.a - Game.dt / 2f);
						}
						motherTint.selfColor = new Color(motherTint.selfColor.r, motherTint.selfColor.g, motherTint.selfColor.b, motherTint.selfColor.a - Game.dt / 5f);
						motherFaceTint.selfColor = new Color(motherFaceTint.selfColor.r, motherFaceTint.selfColor.g, motherFaceTint.selfColor.b, motherFaceTint.selfColor.a - Game.dt / 5f);
						motherDeadTint.selfColor = new Color(motherDeadTint.selfColor.r, motherDeadTint.selfColor.g, motherDeadTint.selfColor.b, motherDeadTint.selfColor.a + Game.dt / 5f);
					}
					else
					{
						if (motherStep.x == -10)
						{
							motherStep = motherLastPosition - motherDead.transform.position;
						}
						if (motherDeltaScale == -1)
						{
							motherDeltaScale = motherDead.transform.localScale.x - herbie.transform.localScale.x;
						}
						if (!motherPlaced && motherDead.transform.position != motherLastPosition)
						{
							if (motherDead.transform.localScale.x <= herbie.transform.localScale.x)
							{
								motherDead.transform.position = motherLastPosition;
								motherDead.transform.localScale = herbie.transform.localScale;
								herbie.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
								grave.SetActive(true);
								motherPlaced = true;
							}
							else
							{
								motherDead.transform.position += motherStep * Game.dt / 5f;
								herbie.transform.position += Vector3.right * 3.35f * Game.dt / 5f;
								motherDead.transform.localScale -= new Vector3(motherDeltaScale, motherDeltaScale, 0f) * Game.dt / 5f;
							}
						}
						else
						{
							if (grave.transform.position.x > 2f)
							{
								grave.transform.position -= Vector3.right * Game.dt * 1f;
								if (grave.transform.position.x < 5f)
								{
									motherDead.transform.position += Vector3.right * Game.dt * 1.25f;
								}
							}
							else
							{
								if (grave.transform.position.y > -2.5f || grave.transform.eulerAngles.z != 0f)
								{
									if (grave.transform.position.y > -2.5f)
									{
										grave.transform.position -= Vector3.up * Game.dt * 0.35f;
									}
									if (grave.transform.eulerAngles.z > 0f && grave.transform.eulerAngles.z <= 300f)
									{
										grave.transform.Rotate(-Vector3.forward * Game.dt * 15f);
									}
									else
									{
										grave.transform.rotation = Quaternion.identity;
									}
									motherDead.transform.position += Vector3.right * Game.dt * 1.25f;
								}
								else
								{
									if (theendTint.selfColor.a < 1f)
									{
										theendTint.selfColor = new Color(theendTint.selfColor.r, theendTint.selfColor.g, theendTint.selfColor.b, theendTint.selfColor.a + Game.dt / 4f);
									}
									else
									{
										if (TaloketoInputManager.GetButtonDown("Select"))
										{
											SceneManager.LoadScene(SceneManager.GetActiveScene().path);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void kiss()
		{
			kissed = true;
		}
	}
}
