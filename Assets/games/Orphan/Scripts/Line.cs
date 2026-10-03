using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class Line : MonoBehaviour
	{
		private tk2dSprite sprite;
		private bool touched;

		public bool touchingFromNegative;
		public RoomUnit roomUnit;
		public bool visible;
		public bool vertical;
		public bool roomVisibleWhenTouched;
		public float alpha;

		void Awake ()
		{
			roomUnit = transform.parent.GetComponent<RoomUnit>();
			sprite = gameObject.GetComponent<tk2dSprite>();
		}

		void Start ()
		{
			sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, 0f);
		}

		void Update ()
		{
			if (visible && sprite.color.a < alpha)
			{
				sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a + 0.01f);
			}
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.name == "Player")
			{
				roomVisibleWhenTouched= roomUnit.roomVisible;
				if (!touched)
				{
					visible = true;
					touched = true;
				}

				if (!vertical)
				{
					touchingFromNegative = (other.transform.position.y < transform.position.y);
				}
				else
				{
					touchingFromNegative = (other.transform.position.x < transform.position.x);
				}

				roomUnit.lineGettingTouched = this;
				if (!roomUnit.roomVisible)
				{
					roomUnit.makeVisible(true);
				}
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.gameObject.name == "Player")
			{
				roomUnit.lineGettingTouched = null;
			}
		}
	}
}
