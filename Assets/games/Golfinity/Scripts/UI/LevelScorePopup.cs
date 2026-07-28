using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Games.Golfinity
{
	public class LevelScorePopup : Popup
	{
	    private float ribbonSize;
	    private int totalReward;
	    public RectTransform ribbonRect;
	    public CanvasGroup ribbonCanvasGroup;
	    public CanvasGroup[] lineCanvasGroups;
	    public Animator[] stars;
	    public TextMeshProUGUI[] textsLeft;
	    public TextMeshProUGUI[] textsRight;
	    public int[] rewards;
	    public ButtonExtended buttonRetry;
	    public ButtonExtended buttonNext;
	    public ButtonExtended buttonAd;
	    public ButtonExtended buttonUpgrade;
	    private float inputTimer;
	    private const float speedUpTime = 4f;

	    protected override void Awake()
	    {
	        base.Awake();
	        ribbonSize = ribbonRect.sizeDelta.x;
	        //AdManager.OnAdWatched += OnAdWatched;
	    }

	    private void OnDestroy()
	    {
	        //AdManager.OnAdWatched -= OnAdWatched;
	    }

	    private void OnEnable()
	    {
	        inputTimer = 0f;
	    }

	    private void Update()
	    {
	        if (Game.input) inputTimer = 1f;
	        inputTimer = Mathf.Max(0f, inputTimer - Game.dt);
	    }

	    private float DtWithInput()
	    {
	        return Time.deltaTime * (inputTimer > 0f ? speedUpTime : 1f);
	    }

	    public override void Show()
	    {
	        base.Show();
	        foreach (var text in textsLeft) if (text != null) text.enabled = false;
	        foreach (var text in textsRight) if (text != null) text.enabled = false;
	        buttonRetry.gameObject.SetActive(false);
	        buttonNext.gameObject.SetActive(false);
	        buttonAd.gameObject.SetActive(false);
	        buttonUpgrade.gameObject.SetActive(false);
	        buttonAd.button.interactable = true;
	        StartCoroutine(Appear());
	    }

	    IEnumerator Appear()
	    {
	        int numStarsOld = Game.GetNumStars(LevelGenerator.CurrentHoleNo);
	        totalReward = 0;
	        rewards = new int[textsRight.Length];
	        textsLeft[0].text = string.Format("{0}{1}{2}{3}{4}", Local.Get("hole"), "   ", "<color=#ffffff00>", LevelGenerator.CurrentHoleNo + 1, "</color>");
	        rewards[0] = numStarsOld > 0 ? 0 : 3;
	        textsLeft[1].text = Local.Get("strokes");
	        textsRight[1].text = $"{Game.noOfStrokesSinceBeginningOfLevel}/{LevelGenerator.NumHits}";
	        rewards[1] = 0;
	        textsLeft[2].text = Local.Get("laststroke");
	        int distanceToHole = Mathf.RoundToInt(Vector3.Magnitude(Game.lastHitPos - HoleTrigger.pos) / LevelGenerator.tileWidth);
	        textsRight[2].text = string.Format(Local.Get("meters"), distanceToHole);
	        rewards[2] = Mathf.Max(0, distanceToHole - 1);
	        textsLeft[3].text = Local.Get("coins");
	        textsRight[3].text = $"{LevelGenerator.NumCoinsCollected}/{LevelGenerator.NumCoins}";
	        rewards[3] = 0;
	        ribbonRect.sizeDelta = new Vector2(0f, ribbonRect.sizeDelta.y);
	        ribbonCanvasGroup.alpha = 0f;
	        int numStars = Game.instance.level.GetNumStars(LevelGenerator.CurrentHoleNo, Game.noOfStrokesSinceBeginningOfLevel);
	        Game.SetNumStars(LevelGenerator.CurrentHoleNo, numStars);
	        foreach (CanvasGroup line in lineCanvasGroups) line.alpha = 0f;
	        float timer = 0f;
	        for (int i = 0, len = stars.Length; i < len; i++)
	        {
	            stars[i].Play("HideStay");
	        }
	        yield return new WaitForSeconds(0.1f);
	        while (timer < 0.1f)
	        {
	            ribbonCanvasGroup.alpha = Easing.Linear(timer, 0f, 1f, 0.1f);
	            yield return null;
	            timer += Time.deltaTime;
	        }
	        ribbonCanvasGroup.alpha = 1f;
	        timer = 0f;
	        while (timer < 0.2f)
	        {
	            float size = Easing.SineEaseOut(timer, 0f, ribbonSize, 0.2f);
	            ribbonRect.sizeDelta = new Vector2(size, ribbonRect.sizeDelta.y);
	            yield return null;
	            timer += DtWithInput();
	        }
	        ribbonRect.sizeDelta = new Vector2(ribbonSize, ribbonRect.sizeDelta.y);
	        yield return new WaitForSeconds(0.1f / (inputTimer > 0f ? speedUpTime : 1f));
	        timer = 0f;
	        while (timer < 0.2f)
	        {
	            var a = Easing.Linear(timer, 0f, 1f, 0.2f);
	            foreach (CanvasGroup line in lineCanvasGroups) line.alpha = a;
	            yield return null;
	            timer += DtWithInput();
	        }
	        foreach (CanvasGroup line in lineCanvasGroups) line.alpha = 1f;
	        yield return new WaitForSeconds(0.3f / (inputTimer > 0f ? speedUpTime : 1f));
	        textsLeft[0].enabled = true;
	        Game.instance.SoundBallHitWall();
	        yield return new WaitForSeconds(0.2f / (inputTimer > 0f ? speedUpTime : 1f));
	        textsLeft[0].text = string.Format("{0}{1}{2}", Local.Get("hole"), "   ", LevelGenerator.CurrentHoleNo + 1);
	        Game.instance.SoundBallHitWall();
	        if (rewards[0] > 0)
	        {
	            totalReward += rewards[0];
	            CoinIcon.Create(rewards[0], textsLeft[0].rectTransform.position);
	            yield return new WaitForSeconds(0.2f / (inputTimer > 0f ? speedUpTime : 1f));
	        }
	        for (int i = 0, len = stars.Length; i < len; i++)
	        {
	            if (i < numStars)
	            {
	                stars[i].Play("Show");
	                yield return new WaitForSeconds(0.4f / (inputTimer > 0f ? speedUpTime : 1f));
	                if (i > numStarsOld - 1)
	                {
	                    totalReward += i;
	                    CoinIcon.Create(i, stars[i].transform.position);
	                    yield return new WaitForSeconds(0.2f / (inputTimer > 0f ? speedUpTime : 1f));
	                }
	            }
	            else
	            {
	                //stars[i].Play("Hide");
	                yield return new WaitForSeconds(0.2f / (inputTimer > 0f ? speedUpTime : 1f));
	            }
	        }
	        yield return new WaitForSeconds(0.2f / (inputTimer > 0f ? speedUpTime : 1f));
	        for (int i = 1, len = textsLeft.Length; i < len; i++)
	        {
	            textsLeft[i].enabled = true;
	            Game.instance.SoundBallHitWall();
	            yield return new WaitForSeconds(0.2f / (inputTimer > 0f ? speedUpTime : 1f));
	            textsRight[i].enabled = true;
	            Game.instance.SoundBallHitWall();
	            yield return new WaitForSeconds(0.2f / (inputTimer > 0f ? speedUpTime : 1f));
	            if (rewards[i] > 0)
	            {
	                totalReward += rewards[i];
	                CoinIcon.Create(rewards[i], textsRight[i].rectTransform.position);
	                yield return new WaitForSeconds(0.2f / (inputTimer > 0f ? speedUpTime : 1f));
	            }
	            yield return new WaitForSeconds(0.2f / (inputTimer > 0f ? speedUpTime : 1f));
	        }
	        yield return new WaitForSeconds(0.5f / (inputTimer > 0f ? speedUpTime : 1f));
	        yield return StartCoroutine(ShowButtons());
	    }

	    private IEnumerator ShowButtons()
	    {
	        Game.instance.SoundBallHitWall();
	        buttonRetry.gameObject.SetActive(false);
	        buttonNext.gameObject.SetActive(false);
	        buttonAd.gameObject.SetActive(false);
	        buttonUpgrade.gameObject.SetActive(false);

	        if (buttonUpgrade.colorIconFlashing)
	        {
	            buttonUpgrade.gameObject.SetActive(true);
	            yield return new WaitForSeconds(0.5f);
	            Game.instance.SoundBallHitWall();
	            //buttonUpgrade.rectTransform.anchoredPosition = new Vector2(!Game.removedAds ? 0f : 24f, buttonUpgrade.rectTransform.anchoredPosition.y);
	            //buttonNext.rectTransform.anchoredPosition = new Vector2(!Game.removedAds ? -44f : -24f, buttonNext.rectTransform.anchoredPosition.y);
	            //buttonAd.rectTransform.anchoredPosition = new Vector2(44f, buttonAd.rectTransform.anchoredPosition.y);
	        }
	        else
	        {
	            //buttonNext.rectTransform.anchoredPosition = new Vector2(!Game.removedAds ? -24f : 0f, buttonNext.rectTransform.anchoredPosition.y);
	            //buttonAd.rectTransform.anchoredPosition = new Vector2(24f, buttonAd.rectTransform.anchoredPosition.y);
	        }
	        buttonRetry.gameObject.SetActive(true);
	        buttonNext.gameObject.SetActive(true);
	        buttonNext.text.gameObject.SetActive(Game.removedAds);
	        buttonAd.gameObject.SetActive(!Game.removedAds);
	    }

	    public void OnClickRetry()
	    {
	        Hide();

	        Game.instance.SetState(GameState.Map);
	    }

	    public void OnClickNext()
	    {
	        if (Game.removedAds)
	        {
	            CoinIcon.Create(totalReward, buttonAd.rectTransform.position);
	        }
	        Hide();

	        Game.NextLevel();
	    }

	    public void OnClickAd()
	    {
	        //AdManager.Show();
	    }

	    private void OnAdWatched()
	    {
	        if (!gameObject.activeInHierarchy) return;

	        CoinIcon.Create(totalReward, buttonAd.rectTransform.position);
	        buttonAd.button.interactable = false;
	        StartCoroutine(ShowButtons());
	    }
	}

}
