using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public enum AudioType
	{
	    Effect,
	    Ambient,
	    Music
	}

	public class TalhaAudioSource : MonoBehaviour
	{
	    public bool convertOnValidate;
	    [Range(0.0f, 1.0f)]
	    public float volume = 1f;//dont change this directly, use changeVolume()
	    public AudioClip[] clips;
	    public AudioSource audioSource;
	    [Range(0.0f, 1.0f)]
	    public float is3D;
	    public bool setPitchAtInit;
	    [Range(-2.0f, 2.0f)]
	    public float pitchInit = 1;
	    public bool loop;
	    public bool playOnAwake;
	    public AudioType type;
	    public bool disableAfterPlay;
	    public static int updateVolume;
	    private static float _masterVolume = 1f;
	    private static float _sfxVolume = 1f;
	    private static float _ambientVolume = 1f;
	    private static float _musicVolume = 1f;
	    public static float masterVolume
	    {
	        get { return _masterVolume; }
	        set
	        {
	            _masterVolume = value;
	            updateVolume = 2;
	        }
	    }
	    public static float sfxVolume
	    {
	        get { return _sfxVolume; }
	        set
	        {
	            _sfxVolume = value;
	            updateVolume = 2;
	        }
	    }
	    public static float ambientVolume
	    {
	        get { return _ambientVolume; }
	        set
	        {
	            _ambientVolume = value;
	            updateVolume = 2;
	        }
	    }
	    public static float musicVolume
	    {
	        get { return _musicVolume; }
	        set
	        {
	            _musicVolume = value;
	            updateVolume = 2;
	        }
	    }
	    public bool isPlaying
	    {
	        get { return audioSource.isPlaying; }
	        set { }
	    }
	    public float pitch
	    {
	        get
	        { 
	            if (audioSource == null)
	            {
	                audioSource = gameObject.AddComponent<AudioSource>();
	            }
	            return audioSource.pitch;
	        }
	        set
	        {
	            if (audioSource == null)
	            {
	                audioSource = gameObject.AddComponent<AudioSource>();
	            }
	            audioSource.pitch = value;
	        }
	    }

	    void OnValidate()
	    {
	        if (convertOnValidate)
	        {
	            if (GetComponent<AudioSource>() != null)
	            {
	                audioSource = GetComponent<AudioSource>();
	                is3D = audioSource.spatialBlend;
	                playOnAwake = audioSource.playOnAwake;
	                loop = audioSource.loop;
	                volume = audioSource.volume;
	                clips = new AudioClip[] { audioSource.clip };
	            }
	        }
	    }

		void Awake ()
	    {
	        audioSource = GetComponent<AudioSource>();
	        if (audioSource == null)
	        {
	            audioSource = gameObject.AddComponent<AudioSource>();
	        }
	        audioSource.spatialBlend = is3D;
	        audioSource.playOnAwake = playOnAwake;
	        audioSource.loop = loop;
	        setVolume();
	        if (playOnAwake)
	        {
	            Play();
	        }
	        if (setPitchAtInit) {
	            pitch = pitchInit;
	        }
		}

	    public void ForceAwake()
	    {
	        Awake();
	    }
		
		void Update ()
	    {
		    if (updateVolume > 0)
	        {
	            setVolume();
	        }

	        if (disableAfterPlay && !audioSource.isPlaying)
	        {
	            transform.parent = ObjectPool.audioPool.transform;
	            gameObject.SetActive(false);
	        }
		}

	    public void changeVolume(float v)
	    {
	        volume = v;
	        setVolume();
	    }

	    private void setVolume()
	    {
	        switch (type)
	        {
	            case AudioType.Effect:
	                audioSource.volume = volume * masterVolume * sfxVolume;
	                break;
	            case AudioType.Ambient:
	                audioSource.volume = volume * masterVolume * ambientVolume;
	                break;
	            case AudioType.Music:
	                audioSource.volume = volume * masterVolume * musicVolume;
	                break;
	        }
	    }

	    public void Play()
	    {
	        if (clips.Length > 0)
	        {
	            audioSource.clip = clips[Random.Range(0, clips.Length)];
	            audioSource.Play();
	        }
	    }

	    public void Stop()
	    {
	        audioSource.Stop();
	    }

	    public static TalhaAudioSource PlayInstance(AudioClip audioClip, AudioType type = AudioType.Effect, Transform parent = null, float volume = 1f, float pitch = 1f, float is3D = 0f)
	    {
	        return PlayInstance(new AudioClip[] { audioClip }, type, parent, volume, pitch);
	    }

	    public static TalhaAudioSource PlayInstance(AudioClip[] audioClips, AudioType type = AudioType.Effect, Transform parent = null, float volume = 1f, float pitch = 1f, float is3D = 0f)
	    {
	        if (audioClips != null && audioClips.Length > 0)
	        {
	            GameObject audioPlayer = ObjectPool.audioPool.get(Vector3.zero);
	            TalhaAudioSource tas = audioPlayer.GetComponent<TalhaAudioSource>();
	            tas.clips = audioClips;
	            tas.type = type;
	            tas.transform.parent = parent;
	            if (parent == null)
	            {
	                tas.transform.position = Camera.main.transform.position;
	            }
	            else
	            {
	                tas.transform.localPosition = Vector3.zero;
	            }
	            tas.changeVolume(volume);
	            tas.playOnAwake = false;
	            tas.loop = false;
	            tas.disableAfterPlay = true;
	            tas.pitch = pitch;
	            tas.is3D = is3D;

	            tas.Awake();
	            tas.Play();
	            return tas;
	        }
	        return null;
	    }
	}

}
