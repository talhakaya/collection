using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Games.Golfinity
{
	public class UpgradePopup : Popup
	{
	    [Header("Upgrades")]
	    public TextMeshProUGUI textUpgrades;
	    public TextMeshProUGUI textUpgradesDesc;
	    public int[] upgradeCosts;
	    public ButtonExtended[] buttonUpgrades;
	    public TextMeshProUGUI[] buttonUpgradeTexts;
	    public GameObject[] ticks;
	    public GameObject[] xs;
	    [Space]
	    [Header("IAP")]
	    public TextMeshProUGUI textDonations;
	    public TextMeshProUGUI textDonationsDesc;
	    public ButtonExtended buttonRestore;
	    public ButtonExtended[] buttonIAPs;

	    private void Start()
	    {
	        Local.OnLanguageChange += OnLanguageChange;
	        OnLanguageChange();
	    }

	    private void OnDestroy()
	    {
	        Local.OnLanguageChange -= OnLanguageChange;
	    }

	    public override void Show()
	    {
	        base.Show();
	        SetButtonStates();
	        if (PlayerPrefs.GetInt("accessedUpgradePopup", 0) == 0)
	        {
	            PlayerPrefs.SetInt("accessedUpgradePopup", 1);
	            PlayerPrefs.Save();
	            Game.SendAnalytics("accessedUpgradePopup");
	        }
	    }

	    public void SetButtonStates()
	    {
	        //for (int i = 0, len = upgradeCosts.Length; i < len; i++)
	        //{
	        //    switch (i)
	        //    {
	        //        case 0:
	        //            buttonUpgrades[i].button.interactable = Game.unlock0Bought || Game.gold >= upgradeCosts[i];
	        //            buttonUpgradeTexts[i].gameObject.SetActive(!Game.unlock0Bought);
	        //            xs[i].gameObject.SetActive(Game.unlock0Bought && !Game.unlock0Enabled);
	        //            ticks[i].gameObject.SetActive(Game.unlock0Bought && Game.unlock0Enabled);
	        //            break;
	        //        case 1:
	        //            buttonUpgrades[i].button.interactable = Game.unlock1Bought || Game.gold >= upgradeCosts[i];
	        //            buttonUpgradeTexts[i].gameObject.SetActive(!Game.unlock1Bought);
	        //            xs[i].gameObject.SetActive(Game.unlock1Bought && !Game.unlock1Enabled);
	        //            ticks[i].gameObject.SetActive(Game.unlock1Bought && Game.unlock1Enabled);
	        //            break;
	        //        default:
	        //            throw new System.NotImplementedException();
	        //    }
	        //}
	#if UNITY_IOS
	        bool hasRestoreButton = true;
	#else
	        bool hasRestoreButton = false;
	#endif
	        int firstX = hasRestoreButton ? -25 : 0;
	        buttonRestore.gameObject.SetActive(hasRestoreButton);
	        for (int i = 0, len = buttonIAPs.Length; i < len; i++)
	        {
	            buttonIAPs[i].text.text = "";// Purchaser.instance.GetPrice(Purchaser.instance.productIds[i]);
	            buttonIAPs[i].rectTransform.anchoredPosition = new Vector2(firstX + i * 50, buttonIAPs[i].rectTransform.anchoredPosition.y);
	        }
	    }

	    private void OnLanguageChange()
	    {
	        textUpgrades.text = Local.Get("unlock");
	        textUpgradesDesc.text = Local.Get("unlockdesc");
	        textDonations.text = Local.Get("removeads");
	        textDonationsDesc.text = Local.Get("removeadsdesc");
	        for (int i = 0, len = buttonUpgradeTexts.Length; i < len; i++)
	        {
	            buttonUpgradeTexts[i].text = upgradeCosts[i].ToString();
	        }
	    }

	    public void OnClickUpgrade(int index)
	    {
	        //bool unlockBought = false;
	        //switch (index)
	        //{
	        //    case 0:
	        //        unlockBought = Game.unlock0Bought;
	        //        break;
	        //    case 1:
	        //        unlockBought = Game.unlock1Bought;
	        //        break;
	        //    default:
	        //        throw new System.NotImplementedException();
	        //}
	        //if (!unlockBought && Game.gold < upgradeCosts[index]) return;
	        //if (unlockBought)
	        //{
	        //    switch (index)
	        //    {
	        //        case 0:
	        //            Game.unlock0Enabled = !Game.unlock0Enabled;
	        //            PlayerPrefs.SetInt("Game.unlock0Enabled", Game.unlock0Enabled ? 1 : 0);
	        //            Game.SendAnalytics("unlock0Enabled");
	        //            break;
	        //        case 1:
	        //            Game.unlock1Enabled = !Game.unlock1Enabled;
	        //            PlayerPrefs.SetInt("Game.unlock1Enabled", Game.unlock1Enabled ? 1 : 0);
	        //            Game.SendAnalytics("unlock1Enabled");
	        //            break;
	        //        default:
	        //            throw new System.NotImplementedException();
	        //    }
	        //}
	        //else
	        //{
	        //    Game.gold -= upgradeCosts[index];
	        //    PlayerPrefs.SetInt("gold", Game.gold);
	        //    switch (index)
	        //    {
	        //        case 0:
	        //            Game.unlock0Bought = true;
	        //            PlayerPrefs.SetInt("Game.unlock0Bought", 1);
	        //            PlayerPrefs.Save();
	        //            break;
	        //        case 1:
	        //            Game.unlock1Bought = true;
	        //            PlayerPrefs.SetInt("Game.unlock1Bought", 1);
	        //            PlayerPrefs.Save();
	        //            break;
	        //        default:
	        //            throw new System.NotImplementedException();
	        //    }
	        //}
	        //PlayerPrefs.Save();
	        //Game.instance.level.GroupTerrain();
	        //SetButtonStates();
	    }

	    public void OnClickRestore()
	    {
	        //Purchaser.instance.RestorePurchases();
	    }

	    public void OnClickIAP(int index)
	    {
	        //Purchaser.instance.BuyProductID(Purchaser.instance.productIds[index]);
	    }

	    public void OnClickFacebook()
	    {
	        Application.OpenURL("https://www.facebook.com/kayabrosgames");
	    }

	    public void OnClickTwitter()
	    {
	        Application.OpenURL("https://twitter.com/kayabros");
	    }

	    public void OnClickMail()
	    {
	        Application.OpenURL("mailto:kayabrosgames@gmail.com");
	    }

	    public void OnClickBack()
	    {
	        Hide();
	    }
	}

}
