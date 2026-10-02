using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextTalha : MonoBehaviour
{	
	private List<TextMesh> texts;
	private GameObject textBasic;
	private float effectPeriodCounter;
	
	public bool initByItself;
	public string text;
	public float size;
	public Color color;
	public int depthOfText;
	public float shakeFactor;
	public float effectPeriod;
	public Vector2 distanceBetweenTexts;
	public bool flickering;

	void Start ()
	{
		if (initByItself)
		{
			Init ();
		}
	}
	
	public void Init(string _text = "Naziş", float _size = 10f, int _depthOfText = 5, Color _color = default(Color), 
	float _shakeFactor = 10f, float _effectPeriod = 0.1f, Vector2 _distanceBetweenTexts = default(Vector2), 
		bool _flickering = true)
	{
		if (!initByItself)
		{
			text = _text;
			size = _size;
			depthOfText = _depthOfText;
			if (_color == default(Color))
			{
				color = Color.black;
			}
			else
			{
				color = _color;
			}
			shakeFactor = _shakeFactor;
			effectPeriod = _effectPeriod;
			effectPeriodCounter = effectPeriod;
			if (_distanceBetweenTexts == default(Vector2))
			{
				distanceBetweenTexts = size * (-Vector2.up + Vector2.right) * 0.1f;
			}
			else
			{
				distanceBetweenTexts = size * _distanceBetweenTexts;
			}
			flickering = _flickering;
		}
		
		textBasic = Resources.Load ("TextBasic") as GameObject;
		texts = new List<TextMesh>();
		for (int i = 0; i < depthOfText; i++)
		{
			GameObject newText = Instantiate(textBasic, transform.position
				+ i * new Vector3(distanceBetweenTexts.x, distanceBetweenTexts.y, 0), transform.rotation) as GameObject;
			newText.transform.parent = transform;
			newText.name = "Text" + (i + 1);
			TextMesh newTextMesh = newText.GetComponent<TextMesh>();
			newTextMesh.fontSize = Mathf.FloorToInt(16f * size);
			newTextMesh.text = text;
			texts.Add (newTextMesh);
			float alpha = color.a;
			if (i > 0)
			{
				alpha = alpha * 0.5f * (depthOfText -  i) / (depthOfText - 1);
			}
			newTextMesh.color = new Color(color.r, color.g, color.b, alpha);
		}
	}
	
	void Update ()
	{
		for (int i = 0; i < depthOfText; i++)
		{
			texts[i].fontSize = Mathf.FloorToInt(16f * size);
			texts[i].color = new Color(color.r, color.g, color.b, texts[i].color.a);
			texts[i].text = text;
		}
		if (shakeFactor != 0f)
		{
			effectPeriodCounter += Time.deltaTime;
			
			if (effectPeriodCounter >= effectPeriod)
			{
				effectPeriodCounter = 0f;
				for (int i = 1; i < depthOfText; i++)
				{
					texts[i].transform.rotation = transform.rotation;
					texts[i].transform.Rotate (Vector3.forward * shakeFactor * Random.Range (-1f, 1f) * (depthOfText -  i) / (depthOfText - 1));
				}
				
				if (flickering)
				{
					for (int i = 0; i < depthOfText; i++)
					{
						if (Random.Range(0f, 1f) >= 0.9f)
						{
							float alpha = color.a * 0.5f;
							if (i > 0)
							{
								alpha = alpha * 0.5f * (depthOfText -  i) / (depthOfText - 1);
							}
							texts[i].color = new Color(color.r, color.g, color.b, alpha);
						}
						else
						{
							float alpha = color.a;
							if (i > 0)
							{
								alpha = alpha * 0.5f * (depthOfText -  i) / (depthOfText - 1);
							}
							texts[i].color = new Color(color.r, color.g, color.b, alpha);
						}
					}
				}
				else
				{
					for (int i = 0; i < depthOfText; i++)
					{
						texts[i].color = new Color(color.r, color.g, color.b, color.a);
					}
				}
			}
		}
	}
	
	public static GameObject create(Vector3 position = default(Vector3), string _text = "Naziş", 
		float _size = 10f, int _depthOfText = 5, Color _color = default(Color), 
		float _shakeFactor = 10f, float _effectPeriod = 0.1f, Vector2 _distanceBetweenTexts = default(Vector2), bool _flickering = true)
	{
		GameObject ret = new GameObject("TextTalha");
		ret.AddComponent<TextTalha>();
		ret = Instantiate(ret, position, Quaternion.identity) as GameObject;
		
		TextTalha retTextTalha = ret.GetComponent<TextTalha>();
		retTextTalha.initByItself = false;
		retTextTalha.Init(_text, _size, _depthOfText, _color, _shakeFactor, _effectPeriod, _distanceBetweenTexts, _flickering);
		return ret;
	}
}
