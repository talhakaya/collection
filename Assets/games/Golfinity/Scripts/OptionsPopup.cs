using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class OptionsPopup : Popup
	{
	    public ButtonExtended buttonReverseShooting;
	    public ButtonExtended buttonHolesOnWalls;
	    public ButtonExtended buttonSound;
	    public ButtonExtended buttonMusic;
	    public ButtonExtended buttonOutlines;
	    public ButtonExtended buttonTerrainEffect;
	    public ButtonExtended buttonCircleHoleEffect;
	    public ButtonExtended buttonBack;

	    private void Start()
	    {
	        Local.OnLanguageChange += OnLanguageChange;
	        buttonReverseShooting.icon.enabled = Game.reverseShooting;
	        buttonHolesOnWalls.icon.enabled = Game.holesOnWalls;
	        buttonSound.icon.enabled = Game.soundOn;
	        buttonMusic.icon.enabled = Game.musicOn;
	        buttonOutlines.icon.enabled = OutlineSprite.isOn;
	        buttonTerrainEffect.icon.enabled = Game.terrainEffectOn;
	        buttonCircleHoleEffect.icon.enabled = Game.circleHoleEffectOn;
	        OnLanguageChange();
	    }

	    private void OnDestroy()
	    {
	        Local.OnLanguageChange -= OnLanguageChange;
	    }

	    private void OnLanguageChange()
	    {
	        buttonReverseShooting.text.text = string.Format("{0} {1}", Local.Get("reverse"), Game.reverseShooting ? Local.Get("on") : Local.Get("off"));
	        buttonHolesOnWalls.text.text = string.Format("{0} {1}", Local.Get("sideholes"), Game.holesOnWalls ? Local.Get("on") : Local.Get("off"));
	        buttonSound.text.text = string.Format("{0} {1}", Local.Get("sound"), Game.soundOn ? Local.Get("on") : Local.Get("off"));
	        buttonMusic.text.text = string.Format("{0} {1}", Local.Get("music"), Game.musicOn ? Local.Get("on") : Local.Get("off"));
	        buttonOutlines.text.text = string.Format("{0} {1}", Local.Get("outlines"), OutlineSprite.isOn ? Local.Get("on") : Local.Get("off"));
	        buttonTerrainEffect.text.text = string.Format("{0} {1}", Local.Get("terraineffect"), Game.terrainEffectOn ? Local.Get("on") : Local.Get("off"));
	        buttonCircleHoleEffect.text.text = string.Format("{0} {1}", Local.Get("circleeffect"), Game.circleHoleEffectOn ? Local.Get("on") : Local.Get("off"));
	    }

	    public void OnClickReverseShooting()
	    {
	        Game.reverseShooting = !Game.reverseShooting;
	        buttonReverseShooting.text.text = string.Format("{0} {1}", Local.Get("reverse"), Game.reverseShooting ? Local.Get("on") : Local.Get("off"));
	        buttonReverseShooting.icon.enabled = Game.reverseShooting;
	        PlayerPrefs.SetInt("Game.reverseShooting", Game.reverseShooting ? 1 : 0);
	    }

	    public void OnClickHolesOnWalls()
	    {
	        Game.holesOnWalls = !Game.holesOnWalls;
	        buttonHolesOnWalls.text.text = string.Format("{0} {1}", Local.Get("sideholes"), Game.holesOnWalls ? Local.Get("on") : Local.Get("off"));
	        buttonHolesOnWalls.icon.enabled = Game.holesOnWalls;
	        PlayerPrefs.SetInt("Game.holesOnWalls", Game.holesOnWalls ? 1 : 0);
	    }

	    public void OnClickSound()
	    {
	        Game.soundOn = !Game.soundOn;
	        buttonSound.text.text = string.Format("{0} {1}", Local.Get("sound"), Game.soundOn ? Local.Get("on") : Local.Get("off"));
	        buttonSound.icon.enabled = Game.soundOn;
	        //if (Game.soundOn) {
	        //    buttonSound.GetComponent<AudioSource>().Play();
	        //}
	        //else {
	        //    buttonSound.GetComponent<AudioSource>().Stop();
	        //}
	        PlayerPrefs.SetInt("Game.soundOn", Game.soundOn ? 1 : 0);
	    }

	    public void OnClickMusic()
	    {
	        Game.musicOn = !Game.musicOn;
	        if (Game.musicOn)
	        {
	            if (Music.instance.audioSource.isPlaying)
	                Music.instance.audioSource.UnPause();
	            else
	                Music.instance.audioSource.Play();
	        }
	        else
	        {
	            Music.instance.audioSource.Pause();
	        }
	        buttonMusic.text.text = string.Format("{0} {1}", Local.Get("music"), Game.musicOn ? Local.Get("on") : Local.Get("off"));
	        buttonMusic.icon.enabled = Game.musicOn;
	        PlayerPrefs.SetInt("Game.musicOn", Game.musicOn ? 1 : 0);
	    }

	    public void OnClickOutlines()
	    {
	        OutlineSprite.isOn = !OutlineSprite.isOn;
	        GameEvents.OnOutlineOnOff?.Invoke();
	        buttonOutlines.text.text = string.Format("{0} {1}", Local.Get("outlines"), OutlineSprite.isOn ? Local.Get("on") : Local.Get("off"));
	        buttonOutlines.icon.enabled = OutlineSprite.isOn;
	        PlayerPrefs.SetInt("OutlineSprite.isOn", OutlineSprite.isOn ? 1 : 0);
	    }

	    public void OnClickTerrainEffect()
	    {
	        Game.terrainEffectOn = !Game.terrainEffectOn;
	        buttonTerrainEffect.text.text = string.Format("{0} {1}", Local.Get("terraineffect"), Game.terrainEffectOn ? Local.Get("on") : Local.Get("off"));
	        buttonTerrainEffect.icon.enabled = Game.terrainEffectOn;
	        PlayerPrefs.SetInt("Game.terrainEffectOn", Game.terrainEffectOn ? 1 : 0);
	    }

	    public void OnClickCircleHoleEffect()
	    {
	        Game.circleHoleEffectOn = !Game.circleHoleEffectOn;
	        buttonCircleHoleEffect.text.text = string.Format("{0} {1}", Local.Get("circleeffect"), Game.circleHoleEffectOn ? Local.Get("on") : Local.Get("off"));
	        buttonCircleHoleEffect.icon.enabled = Game.circleHoleEffectOn;
	        PlayerPrefs.SetInt("Game.circleHoleEffectOn", Game.circleHoleEffectOn ? 1 : 0);
	    }

	    public void OnClickLanguage()
	    {
	        Game.lang = (Lang)(((int)Game.lang + 1) % (Enum.GetValues(typeof(Lang)).Length));
	        Local.SetLanguage(Game.lang);
	        PlayerPrefs.SetInt("Game.lang", (int)Game.lang);
	    }

	    public void OnClickBack()
	    {
	        PlayerPrefs.Save();
	        Hide();
	    }
	}

}
