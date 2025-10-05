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
    [SerializeField] private bool PersistAcrossScenes = true;
    [SerializeField, Range(0f, 1f)] public float volume = 1f;
    [SerializeField] private bool PlayOnStart = true;

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

    void Start()
    {
        if (PlayOnStart) PlayMusic();
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

    public void EndMusic(float fadeOutTime = 1f)
    {
        if (!gameObject.activeInHierarchy) return; //in case of unloading
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeOutAndEnd(fadeOutTime));
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



