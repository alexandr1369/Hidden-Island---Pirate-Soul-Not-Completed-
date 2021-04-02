using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);
    }
    #endregion

    public AudioSource musicSource;
    public List<AudioSource> soundSources;

    public Slider musicSlider;
    public Slider soundSlider;

    public float lowPitch = 0.95f;
    public float highPitch = 1.05f;

    private bool isBgFadeIn;

    void Start()
    {
        // Load volume settings from file
        LoadVolumeSettings();
    }

    // set background music
    public void PlayBackgroundMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    // play single sound
    public void PlaySingle(AudioClip clip)
    {
        foreach(AudioSource source in soundSources)
        {
            if (!source.isPlaying)
            {
                source.clip = clip;
                source.Play();
                break;
            }
        } // foreach
    }
    // playing single sound with decreasing bg sound for N secs
    public void PlaySingle(AudioClip clip, bool effectSound, float decreaseTime)
    {
        foreach (AudioSource source in soundSources)
        {
            if (!source.isPlaying)
            {
                source.clip = clip;
                source.Play();
                break;
            }
        } // foreach

        musicSource.volume /= 2;
        StartCoroutine(TweenAudioVolume(musicSource, musicSlider.value, decreaseTime));
    }
    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randIndex = Random.Range(0, clips.Length);
        float randPitch = Random.Range(lowPitch, highPitch);

        foreach (AudioSource source in soundSources)
        {
            if (!source.isPlaying)
            {
                source.clip = clips[randIndex];
                source.pitch = randPitch;
                source.Play();
                break;
            }
        } // for i
    }
    public void SetMusicVolume()
    {
        float value = musicSlider.value;

        musicSource.volume = value;
        PlayerPrefs.SetFloat("musicVolume", value);
    }
    public void SetSoundVolume()
    {
        float value = soundSlider.value;

        soundSources.ForEach((t) => t.volume = value);
        PlayerPrefs.SetFloat("soundVolume", value);
    }
    private void LoadVolumeSettings()
    {
        float musicVolume, soundVolume;

        if (PlayerPrefs.HasKey("musicVolume"))
            musicVolume = PlayerPrefs.GetFloat("musicVolume");
        else
        {
            musicVolume = 1f;
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
        }

        if (PlayerPrefs.HasKey("soundVolume"))
            soundVolume = PlayerPrefs.GetFloat("soundVolume");
        else
        {
            soundVolume = 1f;
            PlayerPrefs.SetFloat("soundVolume", soundVolume);
        }

        musicSource.volume = musicVolume;
        musicSlider.value = musicVolume;

        soundSources.ForEach((t) => t.volume = soundVolume);
        soundSlider.value = soundVolume;
    }
    private IEnumerator TweenAudioVolume(AudioSource source, float volumeTo, float sec)
    {
        yield return new WaitForSeconds(sec);
        iTween.AudioTo(gameObject, iTween.Hash(
            "audiosource", source,
            "volume", volumeTo,
            "time", 1f,
            "easetype", iTween.EaseType.easeInOutQuad
        ));
    }
}
