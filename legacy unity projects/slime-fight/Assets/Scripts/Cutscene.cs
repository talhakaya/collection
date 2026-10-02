using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cutscene : MonoBehaviour {

	public static Cutscene instance;
	public int id;
	public SpriteRenderer sprite0;
	public SpriteRenderer sprite1;
	public bool isActive;
	private int slideCounter;
	public List<Sprite> cutscene0_0;
	public List<Sprite> cutscene0_1;
	public List<Sprite> cutscene1_0;
	public List<Sprite> cutscene1_1;
	public List<Sprite> cutscene2_0;
	public List<Sprite> cutscene2_1;
	public float shakeConst;
	public float minShakeConst;
	public float maxShakeConst;
	public AudioSource audioSourceBlip;
	public AudioSource audioSourceExplosion;

	void Awake()
	{
		instance = this;
		stopCutscene ();
	}

	void Start ()
	{

	}

	void Update ()
	{
		if (isActive)
		{
			shakeConst -= Time.deltaTime;
			if (shakeConst < minShakeConst)
			{
				shakeConst = minShakeConst;
			}
			if (!GameManager.nPressed)
			{
				sprite0.transform.localPosition = new Vector3(Random.Range(-shakeConst, shakeConst), Random.Range(-shakeConst, shakeConst), sprite0.transform.localPosition.z);
				sprite1.transform.localPosition = new Vector3(Random.Range(-shakeConst, shakeConst), Random.Range(-shakeConst, shakeConst), sprite1.transform.localPosition.z);
			}
			else
			{
				sprite0.transform.localPosition = new Vector3(0f, 0f, sprite0.transform.localPosition.z);
				sprite1.transform.localPosition = new Vector3(0f, 0f, sprite1.transform.localPosition.z);
			}
			if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.N))
			{
				slideCounter++;
				if (id == 0)
				{
					if (slideCounter >= cutscene0_0.Count)
					{
						stopCutscene();
						GameManager.startGame();
					}
					else
					{
						updateSlide();
					}
				}
				else if (id == 1)
				{
					if (slideCounter >= cutscene1_0.Count)
					{
						stopCutscene();
						audioSourceExplosion.Stop();
						Destroy (GameManager.game.gameObject);
						startCutscene(2);
					}
					else
					{
						updateSlide();
					}
				}
				else if (id == 2)
				{
					if (slideCounter >= cutscene2_0.Count)
					{
						Application.Quit();
					}
					else
					{
						updateSlide();
					}
				}
			}
		}
//		else
//		{
//			if (Input.anyKeyDown)
//			{
//				startCutscene(0);
//			}
//		}
	}

	public void startCutscene(int cutsceneID)
	{
		id = cutsceneID;
		isActive = true;
		slideCounter = 0;
		sprite0.gameObject.SetActive (true);
		sprite1.gameObject.SetActive (true);
		updateSlide();
	}

	public void stopCutscene()
	{
		isActive = false;
		sprite0.gameObject.SetActive (false);
		sprite1.gameObject.SetActive (false);
	}

	private void updateSlide()
	{
		shakeConst = maxShakeConst;
		if (id == 0)
		{
			if (slideCounter == 7)
			{
				audioSourceExplosion.pitch = 1;
				audioSourceExplosion.Play ();
			}
			else
			{
				audioSourceBlip.pitch = Random.Range(0.9f, 1.1f);
				audioSourceBlip.Play ();
			}
			sprite0.sprite = cutscene0_0[slideCounter];
			sprite1.sprite = cutscene0_1[slideCounter];
		}
		else if (id == 1)
		{
			audioSourceExplosion.pitch = 0.1f;
			audioSourceExplosion.Play ();
			sprite0.sprite = cutscene1_0[slideCounter];
			sprite1.sprite = cutscene1_1[slideCounter];
		}
		else if (id == 2)
		{
			if (slideCounter < 5)
			{
				audioSourceBlip.pitch = Random.Range(0.9f, 1.1f);
				audioSourceBlip.Play ();
			}
			else
			{
				audioSourceExplosion.pitch = 0.1f;
				audioSourceExplosion.Play ();
			}
			sprite0.sprite = cutscene2_0[slideCounter];
			sprite1.sprite = cutscene2_1[slideCounter];
		}
	}
}
