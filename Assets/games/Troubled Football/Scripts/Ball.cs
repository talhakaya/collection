using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.TroubledFootball
{
	public class Ball : MonoBehaviour {

		bool isHit;
		bool isRigid;
		public float period;
		float timeCounter;
		float minY = -4;
		float maxY = 5;
		public Rigidbody2D body;
		public Rigidbody2D keeper1;
		public Rigidbody2D keeper2;
		public float bodyOffset;
		public float bodyForceConst;
		private Vector3 bodyInitialPos;
		private Vector3 keeper1InitialPos;
		private Vector3 keeper2InitialPos;

		public static Ball instance;

		public GameObject textTitle;

		void Awake()
		{
			instance = this;
		}

		void Start ()
		{
			bodyInitialPos = body.transform.position;
			keeper1InitialPos = keeper1.transform.position;
			keeper2InitialPos = keeper2.transform.position;
			init ();
		}

		void Update ()
		{
			if (!isRigid)
			{
				if (!isHit)
				{
					if (!float.IsNaN(TaloketoInputManager.mousePosition.x))
					{
						float mouseX = GameManager.mouseX;
						transform.position = new Vector3(mouseX, minY, transform.position.z);
					}
					if (TaloketoInputManager.GetMouseButtonDown(0))
					{
						isHit = true;
						AudioManager.playKick = true;
						textTitle.SetActive(false);
					}
				}
				else
				{
					timeCounter += Time.deltaTime;
					if (timeCounter >= period)
					{
						timeCounter = period;
						physics();
					}
					transform.position = new Vector3(transform.position.x, maxY - (period - timeCounter) / period * (maxY - minY), transform.position.z);
					transform.localScale = new Vector3(1f, 1f, 1f) * (0.35f + 0.65f * (period - timeCounter) / period);
					moveBody();
				}
			}
			else
			{
				moveBody();

				if (TaloketoInputManager.GetButtonDown("Restart") || TaloketoInputManager.GetMouseButtonDown(1))
				{
					init ();
				}
			}
		}

		public void init()
		{
			isHit = false;
			isRigid = false;
			timeCounter = 0f;
			gameObject.GetComponent<Collider2D>().enabled = false;
			GetComponent<Rigidbody2D>().gravityScale = 0f;
			body.transform.position = bodyInitialPos;
			body.linearVelocity = Vector2.zero;
			body.angularVelocity = 0;
			body.transform.rotation = Quaternion.identity;
			GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
			GetComponent<Rigidbody2D>().angularVelocity = 0;
			transform.localScale = new Vector3 (1f, 1f, 1f);
			transform.rotation = Quaternion.identity;
			keeper1.transform.position = keeper1InitialPos;
			keeper1.linearVelocity = Vector2.zero;
			keeper1.angularVelocity = 0;
			keeper2.transform.position = keeper2InitialPos;
			keeper2.linearVelocity = Vector2.zero;
			keeper2.angularVelocity = 0;
			GameManager.gameState = 0;
			if (Random.Range (0f, 1f) < 0.2f)
			{
				textTitle.SetActive(true);
			}
		}

		void physics()
		{
			isRigid = true;
			gameObject.GetComponent<Collider2D>().enabled = true;
			GetComponent<Rigidbody2D>().gravityScale = 2f;
		}

		void moveBody()
		{
			if (GameManager.mouseX > body.transform.position.x + bodyOffset)
			{
	//			body.transform.position += new Vector3(bodyForceConst * Time.deltaTime, 0f, 0f);
				body.AddForce(new Vector2(bodyForceConst * Time.deltaTime, 0f));
	//			body.MovePosition();
			}
			else if (GameManager.mouseX < body.transform.position.x - bodyOffset)
			{
	//			body.MovePosition(new Vector2(-bodyForceConst * Time.deltaTime, 0f));
				body.AddForce(new Vector2(-bodyForceConst * Time.deltaTime, 0f));
	//			body.transform.position += new Vector3(-bodyForceConst * Time.deltaTime, 0f, 0f);
			}
		}

		void OnCollisionEnter2D(Collision2D coll) {
			AudioManager.playBall = true;
		}
	}
}
