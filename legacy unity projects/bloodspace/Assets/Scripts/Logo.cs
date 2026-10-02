using UnityEngine;
using System.Collections;

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
	}

	void Update ()
	{
		Color random = new Color(Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
		tint.selfColor = random;
		SpriteEffect.blurConst = Random.Range (0f, 1f);
		SpriteEffect.rgbSplitConst = Random.Range (0f, 6f);


		for (int i = 0; i < textsToFlip.Length; i++)
		{
			textsToFlip[i].color = random;
		}

		if (Input.GetButtonDown("Select0") || Input.GetButtonDown("Select1"))
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
