using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class MainMenu : MonoBehaviour
{
    public void Play()
    {

        if (PlayerController.input != null) PlayerController.input.Disable();
        float waitTime = FindFirstObjectByType<MusicPlayer>().EndMusic();
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
        SceneManager.LoadScene("IntroScene", LoadSceneMode.Single); //load new scene
        }
 
    public void Quit()
    {
        Application.Quit();
    }
}