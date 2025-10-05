using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneEndDialogAction : IDialogueAction
{
    [SerializeField] string NextScene = "Room0";
    [SerializeField] MusicPlayer musicPlayerToStop;

    void Start()
    {
    }

    public override void Ping(int id)
    {
        if (id == -1)
        {
            PlayerController.input.Disable();
            float waitTime = musicPlayerToStop.EndMusic();
            FadeToFromBlack fadeScript = FindFirstObjectByType<FadeToFromBlack>();
            if (fadeScript != null)
            {
                float timeToWait = Mathf.Max(fadeScript.fadeDuration, waitTime);
                fadeScript.FadeOut();
                StartCoroutine(WaitThenLoad(timeToWait + 0.2f));
            }
            else StartCoroutine(WaitThenLoad(waitTime + 0.2f));
        }
    }

    IEnumerator WaitThenLoad(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(NextScene, LoadSceneMode.Single); //load new scene
    }
}
