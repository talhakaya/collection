using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelect : MonoBehaviour {
    public const float WeaponAlpha = 0.5f;
    public CanvasGroup weaponUi;
    public CanvasGroup[] weaponCanvasGroups;
    public Image[] weaponIcons;
    public Sprite iconHand;
    
    void Start() {

    }
    
    void Update() {
        weaponUi.alpha = Mathf.Max(weaponUi.alpha - Game.dt * 0.5f, WeaponAlpha);
    }

    public void ResetState() {
        weaponUi.alpha = WeaponAlpha;
        for (int i = 0, len = weaponCanvasGroups.Length; i < len; i++) {
            if (i >= Player.person.tools.Length) {
                weaponCanvasGroups[i].alpha = 0f;
            }
            else {
                if (i == Player.person.toolIndex) {
                    weaponCanvasGroups[i].alpha = 1f;
                }
                else {
                    weaponCanvasGroups[i].alpha = WeaponAlpha;
                }

                if (Player.person.tools[i] == null) {
                    weaponIcons[i].sprite = iconHand;
                }
                else if (Player.person.tools[i].GetComponent<Remote>() != null) {
                    weaponIcons[i].sprite = Player.person.tools[i].GetComponent<Remote>().gadget.icon;
                }
                else if (Player.person.tools[i].GetComponent<Gun>() != null) {
                    weaponIcons[i].sprite = Player.person.tools[i].GetComponent<Gun>().icon;
                }
            }
        }
    }
}
