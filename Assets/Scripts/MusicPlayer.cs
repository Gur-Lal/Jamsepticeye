using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))] //no point in having a music player without one of these!
public class MusicPlayer : MonoBehaviour
{
    [Header("Audio clips")]
    [SerializeField] private AudioClip introClip;
    [SerializeField] private AudioClip loopClip;
    //[SerializeField] private AudioClip outroClip; //if add relevant logic down below

    [Header("Settings")]
    [SerializeField] public bool PersistAcrossScenes = true;
    [SerializeField, Range(0f, 1f)] public float volume = 1f;
    [SerializeField] private bool PlayOnStart = true;
    [SerializeField] private float PlayOnStartDelay = 1f;

    private AudioSource source;
    private Coroutine fadeRoutine; //keep track of running coroutine


    void Awake()
    {

        if (PersistAcrossScenes)
        {
            var allPlayers = FindObjectsByType<MusicPlayer>(FindObjectsSortMode.None);
            if (allPlayers.Length > 1)
            {
                Destroy(gameObject);
                return; //Disallow the existence of more than 1 audio player in the scene, if persistent
            }

            DontDestroyOnLoad(gameObject);
        }

        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.loop = false;
        source.volume = volume;
    }

    void UpdateMusicVol()
    {
        if(AudioManager.Instance != null)  source.volume = AudioManager.Instance.musicVolume;
    }

    void Start()
    {
        UpdateMusicVol();
        if (PlayOnStart) StartCoroutine(WaitThenPlay(PlayOnStartDelay));
    }

    void FixedUpdate()
    {
        if(AudioManager.Instance != null)  if (source.volume != AudioManager.Instance.musicVolume)
        UpdateMusicVol(); //regularly check
    }

    private IEnumerator WaitThenPlay(float duration)
    {
        yield return new WaitForSeconds(duration);
        PlayMusic();
    }

    public void PlayMusic()
    {
        StopAllCoroutines();
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        //get through intro once
        if (introClip)
        {
            source.clip = introClip;
            source.loop = false;
            source.Play();
            yield return new WaitForSeconds(introClip.length);
        }
        //get through loop as many times as it takes

        if (loopClip)
        {
            source.clip = loopClip;
            source.loop = true;
            source.Play();
        }
    }

    public float EndMusic(float fadeOutTime = 1f)
    {
        if (!gameObject.activeInHierarchy) return 0f; //in case of unloading
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOutAndEnd(fadeOutTime));
        return fadeOutTime;
    }

    private IEnumerator FadeOutAndEnd(float duration)
    {
        float startVol = source.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVol;

        //maybe put outro logic here???
    }
}



