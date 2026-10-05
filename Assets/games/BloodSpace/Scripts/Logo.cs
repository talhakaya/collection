using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.BloodSpace
{
	public class Logo : MonoBehaviour {

		private TintScript tint;
		public GameObject otherLogo;
		public Transform referenceForEffect;
		public GameObject[] objectsToSetActive;
		public GameObject[] objectsToDestroy;
		public TextMesh[] textsToFlip;

		void Start ()
		{
			tint = GetComponent<TintScript> ();

	//		Vector2 relativeVector = new Vector2 (transform.position.x + 10, transform.position.y);

			SpriteEffect.make (Effect.Blur, gameObject, false, true, referenceForEffect);
			SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, referenceForEffect);

	//		relativeVector = new Vector2 (otherLogo.transform.position.x + 3, otherLogo.transform.position.y);

			SpriteEffect.make (Effect.Blur, otherLogo, false, true, referenceForEffect);
			SpriteEffect.make (Effect.RGBSplit, otherLogo, false, true, referenceForEffect);

			Game.PrefabInit ();

			// In the collection: the line read "F4 for fullscreen, ENTER to begin". F4 is gone (the
			// collection has its own display settings), and ENTER is the collection's glyph for
			// the device in use: a prompt object under the text, which flickers with it below.
			foreach (TextMesh mesh in textsToFlip)
			{
				if (mesh != null && beginPrompt == null)
				{
					beginPrompt = mesh.GetComponentInChildren<TMPro.TMP_Text>(true);
				}
			}
		}

		private TMPro.TMP_Text beginPrompt;

		void Update ()
		{
			Color random = new Color(Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
			tint.selfColor = random;
			SpriteEffect.blurConst = Random.Range (0f, 1f);
			SpriteEffect.rgbSplitConst = Random.Range (0f, 6f);


			for (int i = 0; i < textsToFlip.Length; i++)
			{
				textsToFlip[i].color = random;
				if (beginPrompt != null) beginPrompt.color = random;
			}

			if (TaloketoInputManager.GetButtonDown("Select0") || TaloketoInputManager.GetButtonDown("Select1"))
			{
				ExplosionScript.Create(transform.position, 2.5f);
				ExplosionScript.Create(otherLogo.transform.position, 2.5f);
				for (int i = 0; i < objectsToSetActive.Length; i++)
				{
					objectsToSetActive[i].SetActive(true);
				}
				for (int i = 0; i < objectsToDestroy.Length; i++)
				{
					Destroy (objectsToDestroy[i]);
				}
				Destroy (gameObject);
			}
		}
	}
}
