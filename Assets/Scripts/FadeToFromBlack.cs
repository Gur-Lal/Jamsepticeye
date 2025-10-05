using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class FadeToFromBlack : MonoBehaviour
{
    [SerializeField] public float fadeDuration = 2f;
    private Image fadeImage;
    private Coroutine fadeRoutine;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 1f); //fully opaque black
        
    }

    void Start()
    {
        FadeIn();
    }

    public void FadeIn()  //from black to transparent
    {
        StartFade(0f);
    }

    public void FadeOut() //from transparent to black
    {
        StartFade(1f);
    }

    public void Fade(float targetAlpha, float duration)
    {
        fadeDuration = duration;
        StartFade(targetAlpha);
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, newAlpha);
            yield return null;
        }

        fadeImage.color = new Color(0f, 0f, 0f, targetAlpha);
    }
}