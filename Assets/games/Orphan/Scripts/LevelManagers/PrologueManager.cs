using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.Orphan
{
	public class PrologueManager : MonoBehaviour {

		private bool playerMovedEver;
		private bool firstFramePassed;

		public float xBeginningOfDarkening;
		public float xEndOfDarkening;
		public float xBeginningOfScalingUp;
		public float xEndOfScalingUp;
		public List<GameObject> particles;
		public Color darkColor;
		public float maxScaleOfParticles;
		public AudioSource musicSource;
		public AudioSource ambientSource;



		void Start ()
		{

		}

		void Update ()
		{
			if (!firstFramePassed)
			{
				firstFramePassed = true;
				CameraScript.changeZoom(1.5f);
			}

			if (!playerMovedEver && PlayerScript.instance.directionForCamera != Vector3.zero)
			{
				playerMovedEver = true;
				CameraScript.zoomInOut(0.9f, 0.1f);
			}

			//TEMPORARY AUDIO CODE
			/*if (!musicSource.gameObject.activeSelf && PlayerScript.instance.transform.position.x > 110f)
			{
				musicSource.gameObject.SetActive(true);
				ambientSource.gameObject.SetActive(false);
			}*/

			float x = PlayerScript.instance.transform.position.x;
			if (x < xBeginningOfDarkening)
			{
				if (GetComponent<Renderer>().material.color != Color.white)
				{
					GetComponent<Renderer>().material.color = Color.white;
				}
			}
			else if (x >= xBeginningOfDarkening && x < xEndOfDarkening)
			{
				GetComponent<Renderer>().material.color = ((x - xBeginningOfDarkening) / (xEndOfDarkening - xBeginningOfDarkening)) * darkColor 
					+ ((xEndOfDarkening - x) / (xEndOfDarkening - xBeginningOfDarkening)) * Color.white;
			}
			else
			{
				if (GetComponent<Renderer>().material.color != darkColor)
				{
					GetComponent<Renderer>().material.color = darkColor;
				}
			}


			if (x < xBeginningOfScalingUp)
			{
				foreach (GameObject item in particles)
				{
					item.transform.localScale = Vector3.one * 1f;
				}
				if (CameraScript.instance.glitchEffect)
				{
					CameraScript.instance.glitchEffect = false;
				}
			}
			else if (x >= xBeginningOfScalingUp && x < xEndOfScalingUp)
			{
				if (CameraScript.instance.cameraTk2d.ZoomFactor != 1f)
				{
					CameraScript.zoomInOut(1f, 0.1f);
				}
				if (!CameraScript.instance.glitchEffect)
				{
					CameraScript.instance.glitchEffect = true;
				}
				foreach (GameObject item in particles)
				{
					item.transform.localScale = Vector3.one * (((x - xBeginningOfScalingUp) / (xEndOfScalingUp - xBeginningOfScalingUp)) * (maxScaleOfParticles - 1f) + 1f);
				}
			}
			else
			{
				foreach (GameObject item in particles)
				{
					item.transform.localScale = Vector3.one * maxScaleOfParticles;
				}
			}
		}
	}
}
