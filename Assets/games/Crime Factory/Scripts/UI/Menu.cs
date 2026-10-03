using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Collection.Controls;

namespace Games.CrimeFactory
{
	public class Menu : MonoBehaviour
	{
	    public TextMeshProUGUI textStart;
	    public TextMeshProUGUI textContinue;
	    public TextMeshProUGUI textNewGame;
	    public TextMeshProUGUI textCredits;
	    public TextMeshProUGUI textQuit;
	    public GameObject[] creditObjects;
	    public TextMeshProUGUI[] creditTexts;
	    public AudioSource audioCredits;
	    private bool areCreditsPlaying;
	    // Start is called before the first frame update
	    void Start() {
	        Localization.Init();
	        textStart.text = Localization.Get("START");
	        textContinue.text = Localization.Get("CONTINUE");
	        textNewGame.text = Localization.Get("NEW_GAME");
	        textCredits.text = Localization.Get("CREDITS");
	        textQuit.text = Localization.Get("QUIT");
	        creditTexts[0].text = Localization.Get("GAME_BY");
	        creditTexts[2].text = Localization.Get("BACKGROUND_BY");
	        bool hasSave = SaveSystem.HasSave();
	        textStart.gameObject.SetActive(!hasSave);
	        textContinue.gameObject.SetActive(hasSave);
	        textNewGame.gameObject.SetActive(hasSave);
	#if UNITY_WEBGL
	        textQuit.gameObject.SetActive(false);
	#endif
	    }

	    // In the collection: the menu could only be clicked with the mouse. Up and down now move
	    // a "> " marker over the visible buttons, and Jump (Z, gamepad A) or Enter presses the
	    // marked one. The marker stays hidden until a key or the pad is used, so with the mouse
	    // the menu looks as it did.
	    private TextMeshProUGUI[] navButtons;
	    private string[] navLabels;
	    private int navIndex = -1;
	    private float navVerticalOld;

	    private void UpdateNavigation() {
	        if (navButtons == null) {
	            navButtons = new TextMeshProUGUI[] { textStart, textContinue, textNewGame, textCredits, textQuit };
	            navLabels = new string[navButtons.Length];
	            for (int i = 0; i < navButtons.Length; i++) navLabels[i] = navButtons[i].text;
	        }
	        float vertical = TaloketoInputManager.GetAxisRaw("Vertical");
	        int move = 0;
	        if (vertical > 0.5f && navVerticalOld <= 0.5f) move = -1;
	        if (vertical < -0.5f && navVerticalOld >= -0.5f) move = 1;
	        navVerticalOld = vertical;
	        var keyboard = UnityEngine.InputSystem.Keyboard.current;
	        bool confirm = TaloketoInputManager.GetButtonDown("Jump")
	            || (keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame));
	        if (areCreditsPlaying || !creditObjects[0].activeInHierarchy) return;
	        if (navIndex < 0) {
	            if (move == 0 && !confirm) return;
	            navIndex = 0;
	            while (!navButtons[navIndex].gameObject.activeSelf) navIndex++;
	            move = 0;
	            confirm = false;
	        }
	        while (move != 0) {
	            navIndex = (navIndex + move + navButtons.Length) % navButtons.Length;
	            if (navButtons[navIndex].gameObject.activeSelf) move = 0;
	        }
	        for (int i = 0; i < navButtons.Length; i++) {
	            navButtons[i].text = (i == navIndex ? "> " : "") + navLabels[i];
	        }
	        if (confirm) {
	            navButtons[navIndex].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
	        }
	    }

	    private void Update() {
	        UpdateNavigation();
	        if (Platformer.CancelPressed()) {
	            GlobalInputManager.ReturnToMainMenu(); // In the collection: was Application.Quit().
	        }
	    }

	    public void OnClickNewGame() {
	        SaveSystem.WipeData();
	        OnClickContinue();
	    }

	    public void OnClickContinue() {
	        UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/games/Crime Factory/Scenes/main.unity"); // In the collection: by path.
	    }

	    public void OnClickCredits() {
	        if (areCreditsPlaying) return;
	        StartCoroutine(CreditsAnimation());
	    }

	    public void OnClickQuit() {
	        GlobalInputManager.ReturnToMainMenu(); // In the collection: was Application.Quit().
	    }

	    private IEnumerator CreditsAnimation() {
	        areCreditsPlaying = true;
	        audioCredits.Play();
	        creditObjects[0].SetActive(false);
	        yield return new WaitForSeconds(0.5f);
	        creditObjects[1].SetActive(true);
	        yield return new WaitForSeconds(1.5f);
	        creditTexts[0].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[1].gameObject.SetActive(true);
	        yield return new WaitForSeconds(1.5f);
	        creditTexts[2].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[3].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[4].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[5].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[6].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[3].gameObject.SetActive(false);
	        creditTexts[7].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[4].gameObject.SetActive(false);
	        creditTexts[8].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[5].gameObject.SetActive(false);
	        creditTexts[9].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[6].gameObject.SetActive(false);
	        creditTexts[10].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[7].gameObject.SetActive(false);
	        creditTexts[11].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[8].gameObject.SetActive(false);
	        creditTexts[12].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[9].gameObject.SetActive(false);
	        creditTexts[13].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        creditTexts[10].gameObject.SetActive(false);
	        creditTexts[14].gameObject.SetActive(true);
	        yield return new WaitForSeconds(0.5f);
	        foreach (var text in creditTexts) text.gameObject.SetActive(false);
	        creditObjects[1].SetActive(false);
	        creditObjects[0].SetActive(true);
	        yield return new WaitForSeconds(1.5f);
	        audioCredits.Stop();
	        areCreditsPlaying = false;
	    }
	}
}
