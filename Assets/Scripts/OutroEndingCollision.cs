using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
public class OutroEndingCollision : MonoBehaviour
{
    [SerializeField] public string NextScene;
    [SerializeField] public MusicPlayer musicPlayerToStop;
    bool Lock = false;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && (!Lock))
        {
            Lock = true;
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