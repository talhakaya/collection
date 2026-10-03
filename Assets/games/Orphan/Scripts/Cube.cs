using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class Cube : MonoBehaviour {

		private float x;
		private float y;
		private float z;
		private float tweenTime;
		private float tweenPeriod;
		private bool taken;


		public float radius;
		public Vector3 point;

		void Start ()
		{
			x = Random.Range(-1f, 1f);
			y = Random.Range(-1f, 1f);
			z = Random.Range(-1f, 1f);
			tweenTime = 0.6f * Random.Range (1f, 2f) * 3;
			tweenPeriod = tweenTime * Random.Range (0f, 1f);
			point = transform.position;

		}

		// Update is called once per frame
		void Update ()
		{
			transform.Rotate(new Vector3(x, y, z));

			tweenPeriod += Time.deltaTime;

			if (tweenPeriod > tweenTime)
			{
				tweenPeriod = 0;
				newTween();
			}

			if (taken)
			{
				if (GetComponent<Renderer>().material.color.a <= 0)
				{
					Destroy(transform.parent.gameObject);
				}
				else
				{
					foreach (Transform child in transform.parent)
					{
						if (child.GetComponent<Renderer>() != null)
						{
							child.GetComponent<Renderer>().material.color = new Color(child.GetComponent<Renderer>().material.color.r, 
																		child.GetComponent<Renderer>().material.color.g, 
																		child.GetComponent<Renderer>().material.color.b, 
																		child.GetComponent<Renderer>().material.color.a - 0.0015f);
						}
					}
					transform.parent.localScale = transform.parent.localScale * 1.05f;
				}
			}
		}

		void newTween()
		{
			x = Random.Range(-1f, 1f);
			y = Random.Range(-1f, 1f);
			z = Random.Range(-1f, 1f);
			Vector3[] tweenCoord = new Vector3[6];
			tweenCoord[0] = point;
			tweenCoord[1] = newTweenPoint();
			tweenCoord[2] = newTweenPoint();
			tweenCoord[3] = newTweenPoint();
			tweenCoord[4] = newTweenPoint();
			tweenCoord[5] = newTweenPoint();
			PathTween.MoveTo(gameObject, tweenTime, tweenCoord); // In the collection: was iTween.MoveTo with easeInOutCubic
		}

		Vector3 newTweenPoint()
		{
			float angle = Random.Range(0.0f, Mathf.PI * 2);
			Vector3 pointt = new Vector3(Mathf.Cos (angle), Mathf.Sin (angle), transform.position.z);
			return point + radius * pointt;
		}

		void OnTriggerEnter (Collider collider)
		{
			if (!taken && collider.gameObject.name == "Player")
			{
				taken = true;
				DreamLevelManager.incrementJewelCount ();
			}
		}
	}
}
