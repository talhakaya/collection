using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class UIReferences : MonoBehaviour
	{
	    public static UIReferences instance;

	    public static LevelScorePopup levelScorePopup => instance._levelScorePopup;
	    public static OptionsPopup optionsPopup => instance._optionsPopup;
	    public static UpgradePopup upgradePopup => instance._upgradePopup;
	    public static CheatPopup cheatPopup => instance._cheatPopup;
	    public static GameObject blocker => instance._blocker;
	    public static LogoPopup logo => instance._logo;
	    public static TutorialPopup tutorial => instance._tutorial;

	    public LevelScorePopup _levelScorePopup;
	    public OptionsPopup _optionsPopup;
	    public UpgradePopup _upgradePopup;
	    public CheatPopup _cheatPopup;
	    public GameObject _blocker;
	    public LogoPopup _logo;
	    public TutorialPopup _tutorial;

	    private void Awake()
	    {
	        instance = this;
	    }
	}

}
