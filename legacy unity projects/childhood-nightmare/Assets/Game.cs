using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public float speed = 1f;
	public static float dt = 0f;
	private float timeCounter = 0f;
	private float period1 = 10f;
	private float period2 = 10f;
	private float period3 = 5f;
	private float period4 = 5f;
	private float period5 = 5f;
	private float period6 = 5f;
	private float period7 = 5f;
	private float period8 = 15f;
	private float period9 = 5f;
	private float period10 = 10f;
	private bool trigger1 = false;
//	private bool trigger2 = false;
//	private bool trigger3 = false;
//	private bool trigger4 = false;
//	private bool trigger5 = false;
//	private bool trigger6 = false;
//	private bool trigger7 = false;
	public AudioSource audioCount;
	public AudioSource audioSong;
	public GameObject lights;
	public GameObject mountains;
	public GameObject screens;
	private float finish = 0f;

	void Start ()
	{
		
	}

	void Update ()
	{
		dt = Time.deltaTime * speed;
		camera.backgroundColor = new Color (0.734375f * TintScript.weatherColor.r, TintScript.weatherColor.g, TintScript.weatherColor.b);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


		timeCounter += Time.deltaTime;
		if (timeCounter < period1)
		{
			audioSong.volume = 0;
		}
		else if (timeCounter < period1 + period2)
		{
			audioSong.volume += Time.deltaTime * 0.1f / period2;
		}
		else if (timeCounter < period1 + period2 + period3)
		{
			TintScript.deltaBWeatherColor (-0.5f * Time.deltaTime / period3);
			TintScript.deltaGWeatherColor (-0.5f * Time.deltaTime / period3);
		}
		else if (timeCounter < period1 + period2 + period3 + period4)
		{
			TintScript.deltaRWeatherColor (-0.5f * Time.deltaTime / period4);
			TintScript.deltaGWeatherColor (0.5f * Time.deltaTime / period4);
		}
		else if (timeCounter < period1 + period2 + period3 + period4 + period5)
		{
			TintScript.deltaGWeatherColor (-0.5f * Time.deltaTime / period4);
			TintScript.deltaBWeatherColor (0.5f * Time.deltaTime / period4);
		}
		else if (timeCounter < period1 + period2 + period3 + period4 + period5 + period6)
		{
			TintScript.deltaRWeatherColor (0.5f * Time.deltaTime / period6);
			TintScript.deltaGWeatherColor (0.5f * Time.deltaTime / period6);
			if (!trigger1)
			{
				lights.SetActive(true);
				trigger1 = true;
			}
		}
		else if (timeCounter < period1 + period2 + period3 + period4 + period5 + period6 + period7)
		{
			audioSong.volume += Time.deltaTime * 0.2f / period7;
			foreach (Transform child in mountains.transform)
			{
				TintScript tnt = child.GetComponent<Mountain>().tint;
				tnt.selfColor = new Color(tnt.selfColor.r, tnt.selfColor.g, tnt.selfColor.b, Random.Range(0f, 1f));
				tnt.transform.localScale = new Vector3(tnt.transform.localScale.x, Random.Range (10f, 100f), tnt.transform.localScale.z);
			}

		}
		else if (timeCounter < period1 + period2 + period3 + period4 + period5 + period6 + period7 + period8)
		{
			foreach (Transform child in mountains.transform)
			{
				TintScript tnt = child.GetComponent<Mountain>().tint;
				tnt.selfColor = new Color(tnt.selfColor.r, tnt.selfColor.g, tnt.selfColor.b, Random.Range(0f, 1f));
				tnt.transform.localScale = new Vector3(tnt.transform.localScale.x, Random.Range (10f, 100f), tnt.transform.localScale.z);
			}
			speed += Time.deltaTime * 19 / period8;
		}
		else if (timeCounter < period1 + period2 + period3 + period4 + period5 + period6 + period7 + period8 + period9)
		{
			foreach (Transform child in mountains.transform)
			{
				TintScript tnt = child.GetComponent<Mountain>().tint;
				tnt.selfColor = new Color(tnt.selfColor.r, tnt.selfColor.g, tnt.selfColor.b, Random.Range(0f, 1f));
				tnt.transform.localScale = new Vector3(tnt.transform.localScale.x, Random.Range (10f, 100f), tnt.transform.localScale.z);
			}
			TintScript.deltaBWeatherColor (-1f * Time.deltaTime / period9);
			TintScript.deltaGWeatherColor (-1f * Time.deltaTime / period9);
		}
		else if (timeCounter < period1 + period2 + period3 + period4 + period5 + period6 + period7 + period8 + period9 + period10)
		{
			foreach (Transform child in mountains.transform)
			{
				TintScript tnt = child.GetComponent<Mountain>().tint;
				tnt.selfColor = new Color(tnt.selfColor.r, tnt.selfColor.g, tnt.selfColor.b, Random.Range(0f, 1f));
				tnt.transform.localScale = new Vector3(tnt.transform.localScale.x, Random.Range (10f, 100f), tnt.transform.localScale.z);
			}
		}
		else
		{
			TintScript.deltaRWeatherColor (-0.5f * Time.deltaTime);
			audioCount.volume = 0;
			audioSong.volume = 0;
			if (TintScript.weatherColor.r <= 0)
			{
				screens.transform.parent = transform;
				transform.position = Vector3.up * 1000;
				finish += Time.deltaTime;
				if (finish >= 5f)
				{
					TintScript.weatherColor = new Color(1f, 1f, 1f);
					Application.LoadLevel(0);
				}
			}
		}
		SpriteEffect.rgbSplitConst = Mathf.Sqrt(100f - Mathf.Pow(timeCounter - 10f, 2f));
	}
}
