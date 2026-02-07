using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this) //unique existence
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
    }

    void OnDisable()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SoundVolume", sfxVolume);
        PlayerPrefs.Save();
    }
}
