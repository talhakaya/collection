using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace Games.CrimeFactory
{
	public class TalkUI : MonoBehaviour {
	    public static TalkUI instance;
	    public TextMeshProUGUI text;
	    public Image imageBubble;
	    public Image imageTail;
	    public CanvasGroup canvasGroup;
	    public Camera cam;
	    public RectTransform bubbleParent;
	    public RectTransform characterBg;
	    public Image imageCharacter;
	    public Image[] imageCharEffects;
	    public Color[] colors;
	    public NPCKey[] npcs;
	    public AudioSource audioSource;
	    private Dictionary<NPC.Char, Sprite> npcDict;
	    private bool isExploding;
	    private float explodeTimer;
	    private float charTimer;
	    private const float CharPeriod = 1f / 24f;

	    void Start () {
	        instance = this;
	        npcDict = new Dictionary<NPC.Char, Sprite>();
	        foreach (NPCKey npc in npcs) {
	            npcDict.Add(npc.charKey, npc.sprite);
	        }
	        int iCount = 1;
	        foreach (Image i in imageCharEffects) {
	            i.rectTransform.anchoredPosition = new Vector2(iCount * 2f, 0f);
	            iCount++;
	        }
	    }

		public void UpdateUI(float dt, NPC npc, PhysBox player) {
			if (isExploding) {
	            explodeTimer += dt;
	            if (explodeTimer > 0.3f) {
	                isExploding = false;
	                canvasGroup.alpha = 0f;
	                transform.localScale = new Vector3(1f, 1f, 1f);
	            }
	            else {
	                canvasGroup.alpha = 1f - (0.3f - explodeTimer) / 0.3f;
	                transform.localScale = new Vector3(1f + explodeTimer * 2f, 1f + explodeTimer * 2f, 1f);
	            }
	        }
	        imageBubble.rectTransform.sizeDelta = new Vector2(imageBubble.rectTransform.sizeDelta.x, imageBubble.rectTransform.sizeDelta.y + 15f * dt * ((text.rectTransform.sizeDelta.y + 30f) - imageBubble.rectTransform.sizeDelta.y));
	        if (npc != null) {
	            bubbleParent.position = cam.WorldToScreenPoint(npc.isPlayerTalking ? player.transform.position : npc.transform.position);
	            bubbleParent.anchoredPosition += new Vector2(0f, 300f);
	            SetCharSprite(npc.isPlayerTalking ? NPC.Char.Player : npc.charKey);
	            characterBg.localScale = new Vector3((npc.isPlayerTalking ? 1f : -1f), 1f, 1f);
	            characterBg.anchoredPosition = new Vector2((npc.isPlayerTalking ? -1f : 1f) * Mathf.Abs(characterBg.anchoredPosition.x), characterBg.anchoredPosition.y);
	            charTimer += dt;
	            while (charTimer >= CharPeriod) {
	                charTimer -= CharPeriod;
	                int iCount = 1;
	                float firstSin = Sin(1);
	                foreach (Image i in imageCharEffects) {
	                    i.rectTransform.anchoredPosition = new Vector2(iCount * 2f, -firstSin + Sin(iCount));
	                    iCount++;
	                    if (UnityEngine.Random.value < 0.04f) {
	                        i.color = colors[UnityEngine.Random.Range(0, colors.Length)];
	                    }
	                }
	            }
	            if (npc.state == NPC.State.LineAppearing) {
	                if (!audioSource.isPlaying) audioSource.UnPause();
	                if (!audioSource.isPlaying) audioSource.Play();
	            }
	            else {
	                if (audioSource.isPlaying) audioSource.Pause();
	            }
	        }
	    }

	    public void Explode() {
	        isExploding = true;
	        explodeTimer = 0f;
	    }

	    private NPC.Char key;
	    private void SetCharSprite(NPC.Char key) {
	       if (this.key != key) {
	            this.key = key;
	            if (!npcDict.ContainsKey(key)) {
	                Debug.LogError(string.Format("NPC Char enum value {0} does not have a Sprite assigned", key.ToString()));
	            }
	            else {
	                Sprite s = npcDict[key];
	                imageCharacter.sprite = s;
	                foreach (Image i in imageCharEffects) {
	                    i.sprite = s;
	                    i.color = colors[UnityEngine.Random.Range(0, colors.Length)];
	                }
	            }
	        }
	    }

	    private float Sin(int iCount) {
	        return 16f * Mathf.Sin((Time.realtimeSinceStartup) + iCount / 8f);
	    }

	    [Serializable]
	    public class NPCKey {
	        public NPC.Char charKey;
	        public Sprite sprite;
	    }
	}
}
