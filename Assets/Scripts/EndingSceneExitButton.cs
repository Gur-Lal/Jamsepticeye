using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingSceneExitButton : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Main Menu";

    public void LoadScene()
    {
        float waitTime = 0f;
        FadeToFromBlack fadeScript = FindFirstObjectByType<FadeToFromBlack>();
        if (fadeScript != null)
        {
            float timeToWait = Mathf.Max(fadeScript.fadeDuration, waitTime);
            fadeScript.FadeOut();
            StartCoroutine(WaitThenLoad(timeToWait + 0.2f));
        }
        else StartCoroutine(WaitThenLoad(waitTime + 0.2f));
    }
    
    IEnumerator WaitThenLoad(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single); //load new scene
    }
}