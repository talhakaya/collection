using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.Orphan
{
	public class RoomUnit : MonoBehaviour
	{

		public Line lineGettingTouchedOld;

		public Color roomSpriteColor;
		public bool roomVisible;
		public GameObject room;
		public bool updateSpritesOfObjectsInRoom;
		public Line lineGettingTouched;
		public float distance;


		void Start ()
		{
			if (roomVisible)
			{
				roomSpriteColor = new Color (1f, 1f, 1f, 1f);
			}
			else
			{
				roomSpriteColor = new Color (1f, 1f, 1f, 0f);
			}
			room.SetActive(roomVisible);
		}

		void Update ()
		{
			updateSpritesOfObjectsInRoom = false;
			if (lineGettingTouched != null)
			{
				if (room.activeSelf)
				{
					updateSpritesOfObjectsInRoom = true;
					float alpha = 0;

					if (!lineGettingTouched.roomVisibleWhenTouched)
					{
						if (!lineGettingTouched.vertical)
						{
							float deltaY = PlayerScript.instance.GetComponent<Collider>().transform.position.y - lineGettingTouched.transform.position.y;
							if (lineGettingTouched.touchingFromNegative)
							{
								alpha = (deltaY + distance) / (2 * distance);
							}
							else
							{
								alpha = (distance - deltaY) / (2 * distance);
							}
						}
						else
						{
							float deltaX = PlayerScript.instance.GetComponent<Collider>().transform.position.x - lineGettingTouched.transform.position.x;
							if (lineGettingTouched.touchingFromNegative)
							{
								alpha = (deltaX + distance) / (2 * distance);
							}
							else
							{
								alpha = (distance - deltaX) / (2 * distance);
							}
						}
					}
					else
					{
						if (!lineGettingTouched.vertical)
						{
							float deltaY = PlayerScript.instance.GetComponent<Collider>().transform.position.y - lineGettingTouched.transform.position.y;
							if (lineGettingTouched.touchingFromNegative)
							{
								alpha = 1f - (deltaY + distance) / (2 * distance);
							}
							else
							{
								alpha = 1f - (distance - deltaY) / (2 * distance);
							}
						}
						else
						{
							float deltaX = PlayerScript.instance.GetComponent<Collider>().transform.position.x - lineGettingTouched.transform.position.x;
							if (lineGettingTouched.touchingFromNegative)
							{
								alpha = 1f - (deltaX + distance) / (2 * distance);
							}
							else
							{
								alpha = 1f - (distance - deltaX) / (2 * distance);
							}
						}
					}

					if (alpha < 0f)
					{
						alpha = 0f;
					}
					else if (alpha > 1f)
					{
						alpha = 1f;
					}

					if (roomVisible)
					{
						roomSpriteColor = new Color (roomSpriteColor.r, roomSpriteColor.g, roomSpriteColor.b, alpha);
					}
					else
					{
						roomSpriteColor = new Color (roomSpriteColor.r, roomSpriteColor.g, roomSpriteColor.b, 1f - alpha);
					}
				}
			}
			else 
			{
				if (roomSpriteColor.a <= 0f && room.activeSelf)
				{
					room.SetActive(false);
				}

				if (lineGettingTouchedOld != lineGettingTouched)
				{
					updateSpritesOfObjectsInRoom = true;
					if (roomSpriteColor.a >= 0.5f)
					{
						roomSpriteColor = new Color(roomSpriteColor.r, roomSpriteColor.g, roomSpriteColor.b, 1f);
						roomVisible = true;
					}
					else
					{
						roomSpriteColor = new Color(roomSpriteColor.r, roomSpriteColor.g, roomSpriteColor.b, 0f);
						roomVisible = false;
					}
				}
			}

			lineGettingTouchedOld = lineGettingTouched;
		}

		public void makeVisible(bool visible)
		{
			roomVisible = visible;
			if (visible)
			{
				room.SetActive(true);
			}
		}
	}
}
