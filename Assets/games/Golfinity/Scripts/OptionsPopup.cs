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
	        // In the collection: sound and music are the collection's own settings (the pause
	        // screen's), so the game's two switches for them are not shown. Both are always on
	        // here (see Game), and how loud they are is the collection's to say.
	        // The buttons under them move up into the two places left empty (the list is laid
	        // out by hand, a button every so far down).
	        RectTransform sound = (RectTransform)buttonSound.transform;
	        RectTransform music = (RectTransform)buttonMusic.transform;
	        float step = sound.anchoredPosition.y - music.anchoredPosition.y;
	        foreach (RectTransform other in sound.parent)
	        {
	            if (other == sound || other == music || other.GetComponent<UnityEngine.UI.Selectable>() == null) continue;
	            if (other.anchoredPosition.y < music.anchoredPosition.y + 0.01f) other.anchoredPosition += new Vector2(0f, step * 2f);
	        }
	        buttonSound.gameObject.SetActive(false);
	        buttonMusic.gameObject.SetActive(false);
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
	        Collection.Saving.SaveManager.Slot.golfinity.reverseShooting = Game.reverseShooting; Collection.Saving.SaveManager.MarkDirty();
	    }

	    public void OnClickHolesOnWalls()
	    {
	        Game.holesOnWalls = !Game.holesOnWalls;
	        buttonHolesOnWalls.text.text = string.Format("{0} {1}", Local.Get("sideholes"), Game.holesOnWalls ? Local.Get("on") : Local.Get("off"));
	        buttonHolesOnWalls.icon.enabled = Game.holesOnWalls;
	        Collection.Saving.SaveManager.Slot.golfinity.holesOnWalls = Game.holesOnWalls; Collection.Saving.SaveManager.MarkDirty();
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
	        Collection.Saving.SaveManager.Slot.golfinity.soundOn = Game.soundOn; Collection.Saving.SaveManager.MarkDirty();
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
	        Collection.Saving.SaveManager.Slot.golfinity.musicOn = Game.musicOn; Collection.Saving.SaveManager.MarkDirty();
	    }

	    public void OnClickOutlines()
	    {
	        OutlineSprite.isOn = !OutlineSprite.isOn;
	        GameEvents.OnOutlineOnOff?.Invoke();
	        buttonOutlines.text.text = string.Format("{0} {1}", Local.Get("outlines"), OutlineSprite.isOn ? Local.Get("on") : Local.Get("off"));
	        buttonOutlines.icon.enabled = OutlineSprite.isOn;
	        Collection.Saving.SaveManager.Slot.golfinity.outlineOn = OutlineSprite.isOn; Collection.Saving.SaveManager.MarkDirty();
	    }

	    public void OnClickTerrainEffect()
	    {
	        Game.terrainEffectOn = !Game.terrainEffectOn;
	        buttonTerrainEffect.text.text = string.Format("{0} {1}", Local.Get("terraineffect"), Game.terrainEffectOn ? Local.Get("on") : Local.Get("off"));
	        buttonTerrainEffect.icon.enabled = Game.terrainEffectOn;
	        Collection.Saving.SaveManager.Slot.golfinity.terrainEffectOn = Game.terrainEffectOn; Collection.Saving.SaveManager.MarkDirty();
	    }

	    public void OnClickCircleHoleEffect()
	    {
	        Game.circleHoleEffectOn = !Game.circleHoleEffectOn;
	        buttonCircleHoleEffect.text.text = string.Format("{0} {1}", Local.Get("circleeffect"), Game.circleHoleEffectOn ? Local.Get("on") : Local.Get("off"));
	        buttonCircleHoleEffect.icon.enabled = Game.circleHoleEffectOn;
	        Collection.Saving.SaveManager.Slot.golfinity.circleHoleEffectOn = Game.circleHoleEffectOn; Collection.Saving.SaveManager.MarkDirty();
	    }

	    public void OnClickLanguage()
	    {
	        Game.lang = (Lang)(((int)Game.lang + 1) % (Enum.GetValues(typeof(Lang)).Length));
	        Local.SetLanguage(Game.lang);
	        Collection.Saving.SaveManager.Slot.golfinity.lang = (int)Game.lang; Collection.Saving.SaveManager.MarkDirty();
	    }

	    public void OnClickBack()
	    {
	        Hide();
	    }
	}

}
