using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class Fadingblackbg : MonoBehaviour
{
    public Image blackOutSquare;
    public float fadeDuration = 2f;
    [SerializeField] private float waitTime = 3f;
    private bool fadeIn = false;
    
    
    public IEnumerator FadeBlackOutSquare(bool fadeIn) {
        Color color = blackOutSquare.color;
            float startAlpha = color.a;
            float targetAlpha = fadeIn ? 1f : 0f;
            float elapsed = 0f;

        while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                color.a = newAlpha;
                blackOutSquare.color = color;
                yield return null;
            }
        // makes sure to fully fade
            color.a = targetAlpha;
            blackOutSquare.color = color;       
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           StartCoroutine(FadeFull());           
        }
    }
    IEnumerator FadeFull() {
        yield return StartCoroutine(FadeBlackOutSquare(true));
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(FadeBlackOutSquare(false));
    }  
}


    
