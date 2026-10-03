using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class GhostScript : BasicDialogueCharacter {

		public bool onlyVisibleWhenClose;
		public float distanceForFullVisibility;
		public float distanceForHalfVisibility;
		public float visibleTimeInHalfVisibility;
		private float counterInHalfVisibility = 0f;
		private tk2dSprite sprite;

		void Start ()
		{
			sprite = GetComponent<tk2dSprite>();
			sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
			//ShadowScript.createShadow(transform, Vector2.one * sizeOfShadow, Vector2.up * (-1) * locationOfShadow);
			DialogueStarter.createDialogueStarter(transform, dialogueId, new Vector2(10, 3), repeatDialogue);
		}

		void Update ()
		{
			bool visible = false;
			if (onlyVisibleWhenClose)
			{
				if (PlayerScript.instance != null)
				{
					Vector2 deltaPosition = new Vector2(PlayerScript.instance.transform.position.x - transform.position.x,
						PlayerScript.instance.transform.position.y - transform.position.y);
					float distanceToPlayer = Mathf.Sqrt(deltaPosition.x * deltaPosition.x + deltaPosition.y * deltaPosition.y);
					if (distanceToPlayer > distanceForHalfVisibility)
					{
						visible = false;
						counterInHalfVisibility = 0f;
					}
					else if (distanceToPlayer > distanceForFullVisibility)
					{
						counterInHalfVisibility += Time.deltaTime;
						if (counterInHalfVisibility >= visibleTimeInHalfVisibility)
						{
							visible = false;
						}
						else
						{
							visible = true;
						}
					}
					else
					{
						counterInHalfVisibility = 0f;
						visible = true;
					}
				}
			}
			else
			{
				visible = true;
			}

			if (visible)
			{
				sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Random.Range(0.1f, 0.4f));
			}
			else
			{
				if (sprite.color.a > 0f)
				{
					sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
				}
			}
		}
	}
}
