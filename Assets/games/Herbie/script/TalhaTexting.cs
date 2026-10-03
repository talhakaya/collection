using UnityEngine;
using System.Collections;

namespace Games.Herbie
{
	public class TalhaTexting : MonoBehaviour {

		public string[] texts;
		public string[] texts1;
		public string[] texts2;
		public GameObject prefab;
		public float yOffset;
		private TextMesh main;
		private TextMesh shadow;
		private TextMesh main1;
		private TextMesh shadow1;
		private TextMesh main2;
		private TextMesh shadow2;
		private bool visible = false;
		public int i = 0;
		private float period = 2f;
		private float maxScale = 1.1f;
		private float firstScale = 0.06f;

		void Start ()
		{
			if (texts.Length != 0)
			{
				GameObject textMesh = Instantiate(prefab, transform.position + Vector3.up * (2.75f + yOffset) - Vector3.forward * 5f, Quaternion.identity) as GameObject;
				textMesh.name = "Text";
				textMesh.transform.parent = transform;
				main = textMesh.GetComponent<TextMesh>();
				shadow = textMesh.transform.Find("Shadow").gameObject.GetComponent<TextMesh>();
			}
			if (texts1.Length != 0)
			{
				GameObject textMesh = Instantiate(prefab, transform.position + Vector3.up * (2.5f + yOffset) - Vector3.forward * 5f, Quaternion.identity) as GameObject;
				textMesh.name = "Text";
				textMesh.transform.parent = transform;
				main1 = textMesh.GetComponent<TextMesh>();
				shadow1 = textMesh.transform.Find("Shadow").gameObject.GetComponent<TextMesh>();
			}
			if (texts2.Length != 0)
			{
				GameObject textMesh = Instantiate(prefab, transform.position + Vector3.up * (2.25f + yOffset) - Vector3.forward * 5f, Quaternion.identity) as GameObject;
				textMesh.name = "Text";
				textMesh.transform.parent = transform;
				main2 = textMesh.GetComponent<TextMesh>();
				shadow2 = textMesh.transform.Find("Shadow").gameObject.GetComponent<TextMesh>();
			}
		}

		void Update ()
		{
			float ratio = (Game.time % (period * 2)) / period;
			float ratio2 = (ratio + 0.5f) % 2f;
			float scaleX = 1f;
			float scaleY = 1f;
			if (ratio < 1f)
			{
				scaleX = 1f + (maxScale - 1f) * ratio;
			}
			else
			{
				scaleX = 1f + (maxScale - 1f) * (2 - ratio);
			}
			if (ratio2 < 1f)
			{
				scaleY = 1f + (maxScale - 1f) * ratio2;
			}
			else
			{
				scaleY = 1f + (maxScale - 1f) * (2 - ratio2);
			}
			if (texts.Length != 0)
			{
				main.transform.localScale = new Vector3(firstScale * scaleX / transform.localScale.x, firstScale * scaleY / transform.localScale.y, 1f / transform.localScale.z);
				if (visible)
				{
					if (i < texts.Length)
					{
						main.text = texts[i];
						shadow.text = texts[i];
					}
					else
					{
						main.text = "";
						shadow.text = "";
					}
				}
				else
				{
					main.text = "";
					shadow.text = "";
				}
			}
			if (texts1.Length != 0)
			{
				main1.transform.localScale = new Vector3(firstScale * scaleX / transform.localScale.x, firstScale * scaleY / transform.localScale.y, 1f / transform.localScale.z);
				if (visible)
				{
					if (i < texts1.Length)
					{
						main1.text = texts1[i];
						shadow1.text = texts1[i];
					}
					else
					{
						main1.text = "";
						shadow1.text = "";
					}
				}
				else
				{
					main1.text = "";
					shadow1.text = "";
				}
			}
			if (texts2.Length != 0)
			{
				main2.transform.localScale = new Vector3(firstScale * scaleX / transform.localScale.x, firstScale * scaleY / transform.localScale.y, 1f / transform.localScale.z);
				if (visible)
				{
					if (i < texts2.Length)
					{
						main2.text = texts2[i];
						shadow2.text = texts2[i];
					}
					else
					{
						main2.text = "";
						shadow2.text = "";
					}
				}
				else
				{
					main2.text = "";
					shadow2.text = "";
				}
			}
		}

		public static void next(GameObject go)
		{
			go.GetComponent<TalhaTexting> ().i++;
		}

		public static void prev(GameObject go)
		{
			go.GetComponent<TalhaTexting> ().i--;
		}

		public static int iGet(GameObject go)
		{
			return go.GetComponent<TalhaTexting> ().i;
		}

		public static void changeI(GameObject go, int newI)
		{
			go.GetComponent<TalhaTexting> ().i = newI;
		}

		public static void reset(GameObject go)
		{
			go.GetComponent<TalhaTexting> ().i = 0;
		}

		public static void visibleOnOff(GameObject go, bool onOff)
		{
			go.GetComponent<TalhaTexting> ().visible = onOff;
		}
	}
}
