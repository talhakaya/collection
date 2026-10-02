using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectInRoom : MonoBehaviour
{
	private tk2dSprite sprite;
	private bool hasSprite;
	private bool hasRenderer;
	private RoomUnit roomUnit;
	private List<tk2dSprite> sprites;
	private List<Renderer> renderers;
	
	void Start ()
	{
		Transform parent = transform.parent;
		while (roomUnit == null && parent != null)
		{
			roomUnit = parent.GetComponent<RoomUnit>();
			parent = parent.transform.parent;
		}
		
		sprite = gameObject.GetComponent<tk2dSprite>();
		hasSprite = (sprite != null);
		hasRenderer = (renderer != null);
		sprites = new List<tk2dSprite>();
		renderers = new List<Renderer>();
		if (!hasSprite && !hasRenderer)
		{
			foreach (Transform child in transform)
			{
				tk2dSprite tempSprite = child.gameObject.GetComponent<tk2dSprite>();
				if (tempSprite != null)
				{
					sprites.Add (tempSprite);
				}
				else
				{
					Renderer tempRenderer = child.gameObject.GetComponent<Renderer>();
					if (tempRenderer != null)
					{
						renderers.Add (tempRenderer);
					}
				}
			}
		}
	}
	
	void Update ()
	{
		if (roomUnit != null)
		{
			if (hasSprite && roomUnit.updateSpritesOfObjectsInRoom)
			{
				sprite.color = roomUnit.roomSpriteColor;
			}
			foreach (tk2dSprite tempSprite in sprites)
			{
				InvisWhenTouch invisWhenTouch = tempSprite.GetComponent<InvisWhenTouch>();
				if (invisWhenTouch != null)
				{
					tempSprite.color = new Color(roomUnit.roomSpriteColor.r, roomUnit.roomSpriteColor.g,
						roomUnit.roomSpriteColor.b, roomUnit.roomSpriteColor.a * invisWhenTouch.color.a);
				}
				else
				{
					tempSprite.color = roomUnit.roomSpriteColor;
				}
			}
			foreach (Renderer tempRenderer in renderers)
			{
				tempRenderer.material.color = new Color(tempRenderer.material.color.r, tempRenderer.material.color.g, 
															tempRenderer.material.color.b, roomUnit.roomSpriteColor.a / 8);
			}
		}
	}
}
