using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TheEndManager : MonoBehaviour {
    public static TheEndManager instance;
    public GameObject background;
    public TextMeshProUGUI text;
    public TextMeshProUGUI textThanks;

    void Awake() {
        instance = this;
    }

    public void Show() {
        LevelEditor.OnAchieveEnding();
        background.SetActive(true);
        AudioPlayer.instance.Play(AudioPlayer.instance.clipComedyDrum);
        StartCoroutine(EndAnimation());
    }

    private IEnumerator EndAnimation() {
        yield return new WaitForSeconds(0.5f);
        text.text = Localization.Get("THE_END");
        yield return new WaitForSeconds(2.5f);
        textThanks.text = Localization.Get("THANKS_FOR_PLAYING");
        AudioPlayer.instance.Play(AudioPlayer.instance.clipDoor);
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
    }
}
