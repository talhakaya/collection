using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private void Update() {
        if (Input.GetButtonDown("Cancel")) {
            Application.Quit();
        }
    }

    public void OnClickNewGame() {
        SaveSystem.WipeData();
        OnClickContinue();
    }

    public void OnClickContinue() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("main");
    }

    public void OnClickCredits() {
        if (areCreditsPlaying) return;
        StartCoroutine(CreditsAnimation());
    }

    public void OnClickQuit() {
        Application.Quit();
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
