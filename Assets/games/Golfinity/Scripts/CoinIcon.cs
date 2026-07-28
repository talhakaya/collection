using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class CoinIcon : MonoBehaviour
	{
	    private static CoinIcon instance;
	    private RectTransform rect;

	    void Start()
	    {
	        instance = this;
	        rect = GetComponent<RectTransform>();
	    }

	    public static void Create(int amount, Vector3 position)
	    {
	        Game.gold += amount;
	        PlayerPrefs.SetInt("gold", Game.gold);
	        PlayerPrefs.Save();
	        Game.goldAnimating += amount;
	        instance.StartCoroutine(instance.DoCreate(amount, position));
	    }

	    private IEnumerator DoCreate(int amount, Vector3 position)
	    {
	        for (int i = 0; i < amount; i++)
	        {
	            GameObject go = Pool.coinIconPool.get(position);
	            RectTransform coinRect = go.GetComponent<RectTransform>();
	            StartCoroutine(DoAnimate(coinRect));
	            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
	        }
	    }

	    private IEnumerator DoAnimate(RectTransform coin)
	    {
	        Game.instance.SoundPickup();
	        Vector3 scale = coin.localScale;
	        Vector2 firstPos = coin.anchoredPosition;
	        Vector2 deltaPos = rect.anchoredPosition - firstPos;
	        Vector2 randomCurvePos = new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));
	        float timer = 0f;
	        float period = Random.Range(0.5f, 1f);
	        coin.localScale = new Vector3(0f, 0f, 1f);
	        while (timer < period)
	        {
	            yield return null;
	            timer += Time.deltaTime;
	            float animRatio = timer / period;
	            float animRatioCurve = animRatio > 0.5f ? Easing.SineEaseOut(1f - (animRatio - 0.5f) * 2f, 0f, 1f, 1f) : Easing.SineEaseOut((animRatio) * 2f, 0f, 1f, 1f);
	            float animRatioBackIn = Easing.BackEaseIn(animRatio, 0f, 1f, 1f);
	            coin.anchoredPosition = firstPos + deltaPos * animRatioBackIn + randomCurvePos * animRatioCurve;
	            float animRatioSineOutX2 = animRatio > 0.8f ? 1f : Easing.BackEaseOut(animRatio * 1.25f, 0f, 1f, 1f);
	            coin.localScale = new Vector3(animRatioSineOutX2 * scale.x, animRatioSineOutX2 * scale.y, 1f);
	        }
	        Game.instance.SoundGold();
	        coin.anchoredPosition = firstPos + deltaPos;
	        coin.localScale = scale;
	        yield return null;
	        Game.goldAnimating--;
	        coin.gameObject.SetActive(false);
	    }
	}

}
