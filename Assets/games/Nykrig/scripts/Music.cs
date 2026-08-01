using UnityEngine;
using System.Collections.Generic;

namespace Games.Nykrig
{
	public class Music : MonoBehaviour {
	    public static Music instance;
	    public AudioSource[] audios;
	    private float[] volumes;
	    public bool[] onOff;

	    void Awake () {
	        instance = this;
	        volumes = new float[audios.Length];
	        for (int i = 0, len = audios.Length; i < len; i++) {
	            volumes[i] = audios[i].volume;
	        }
	    }
		
		void Update () {
	        for (int i = 0, len = audios.Length; i < len; i++) {
	            float v = onOff[i] ? volumes[i] : 0f;
	            if (audios[i].volume > v + volumes[i] * Game.dt) {
	                audios[i].volume -= volumes[i] * Game.dt;
	            }
	            else if (audios[i].volume < v - volumes[i] * Game.dt) {
	                audios[i].volume += volumes[i] * Game.dt;
	            }
	            else {
	                audios[i].volume = v;
	            }
	        }
	    }

	    public void Set(int[] musicChannels, bool immediate = false) {
	        for (int i = 0, len = audios.Length; i < len; i++) {
	            onOff[i] = false;
	            for (int j = 0; j < musicChannels.Length; j++) {
	                if (musicChannels[j] == i) {
	                    onOff[i] = true;
	                }
	            }
	        }
	        if (immediate) {
	            for (int i = 0, len = audios.Length; i < len; i++) {
	                float v = onOff[i] ? volumes[i] : 0f;
	                audios[i].volume = v;
	            }
	        }
	    }

	    public void Set(List<int> musicChannels, bool immediate = false) {
	        for (int i = 0, len = audios.Length; i < len; i++) {
	            onOff[i] = false;
	            for (int j = 0; j < musicChannels.Count; j++) {
	                if (musicChannels[j] == i) {
	                    onOff[i] = true;
	                }
	            }
	        }
	        if (immediate) {
	            for (int i = 0, len = audios.Length; i < len; i++) {
	                float v = onOff[i] ? volumes[i] : 0f;
	                audios[i].volume = v;
	            }
	        }
	    }
	}

}
