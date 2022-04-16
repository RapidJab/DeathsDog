using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Singleton
    public static MusicManager Instance { get; private set; }
    private List<AudioSource> effectSources;
    [SerializeField] private RandomNoises noises;
    [SerializeField] private AudioSource MusicSource;
    [SerializeField] public AudioClip normalMusic;
    [SerializeField] public AudioClip memoryMusic; 
    private List<AudioClip> playedClips;
    [SerializeField] Slider master;
    [SerializeField] Slider music;
    [SerializeField] Slider sfx;
    [SerializeField] AudioMixer musicMixer;
    [SerializeField] AudioMixer sfxMixer;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        effectSources = new List<AudioSource>();
        foreach (Transform child in transform)
        { 
            if (child.GetComponent<AudioSource>() && child.GetComponent<AudioSource>().clip == null)
            {
                effectSources.Add(child.GetComponent<AudioSource>());
            }
        }
        MusicSource.volume = 0;
        ChangeMusic(3f, normalMusic);

        playedClips = new List<AudioClip>();
        Debug.Log("Music: " + music.name);
    } 

    void Update()
    {
        if (playedClips.Count > 0)
        {
            playedClips.Clear();
        }
    }
    public void ChangeMusic(float duration = .5f, AudioClip clip = null)
    {
        StopCoroutine(ChangingMusic());
        StartCoroutine(ChangingMusic(duration, clip));
    }
    private IEnumerator ChangingMusic(float duration = .5f, AudioClip clip = null)
    { 
        float counter = 0;
        float fromVal = MusicSource.volume; 
        if (MusicSource.volume > 0) //Music is still playing
        { 
            while (counter < duration)
            {
                counter += Time.deltaTime; 
                MusicSource.volume = Mathf.Lerp(fromVal, 0, counter / duration);
                yield return null; 
            } 
        }
        counter = 0; 
        if (clip != null)
        {
            MusicSource.Stop();
            MusicSource.clip = clip;
            MusicSource.Play();
            while (counter < duration)
            {
                counter += Time.deltaTime;
                MusicSource.volume = Mathf.Lerp(0, 1, counter / duration);
                yield return null;
            }
        }
    } 

    public void PlaySound(AudioClip clip, float volume = 1, float pitch = 1)
    {
        if (!playedClips.Contains(clip))
        { 
            for (int i = 0; i < effectSources.Count; i++)
            {
                if (!effectSources[i].isPlaying)
                {
                    effectSources[i].clip = clip;
                    effectSources[i].volume = volume; 
                    effectSources[i].pitch = pitch;
                    effectSources[i].Play(); 
                    playedClips.Add(clip);
                    return;
                }
            } 
        }
    }

    #region Set Volume 
    public void SetMasterVolume()
    {
        AudioListener.volume = master.value;
    }

    public void SetMusicVolume()
    {
        Debug.Log("Music: " + music.name);
        musicMixer.SetFloat("musicVolume", Mathf.Log10(music.value) * 20);
    }

    public void SetSFXVolume()
    { 
        sfxMixer.SetFloat("sfxVolume", Mathf.Log10(sfx.value) * 20);
    }
    #endregion 


    #region Random Noises 
    public AudioClip SetPlatformerSong()
    {
        int randNum = Random.Range(0, noises.platformerSongs.Length);
        return noises.platformerSongs[randNum];
    }
    public AudioClip SetMemorySong()
    {
        int randNum = Random.Range(0, noises.memorySongs.Length);
        return noises.memorySongs[randNum];
    }
    public AudioClip SetRandomFallingPlatformSound()
    {
        int randNum = Random.Range(0, noises.fallingPlatformSounds.Length);
        return noises.fallingPlatformSounds[randNum];
    }
    #endregion 

}
