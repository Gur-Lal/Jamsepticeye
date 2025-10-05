using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTp : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; //name of the scene to load
    private bool playerIsNearby = false;

    void Update() {
        if (playerIsNearby && PlayerController.input.Player.Interact.triggered)
        {

            PlayerController.input.Disable();

            //Check if a music player should be ended
            MusicPlayer m = FindFirstObjectByType<MusicPlayer>();
            float timeToWait = 0f;
            if (m != null && m.PersistAcrossScenes == false)
            {
                timeToWait = m.EndMusic();
            }
            FadeToFromBlack fadeScript = FindFirstObjectByType<FadeToFromBlack>();
            if (fadeScript != null)
            {
                fadeScript.FadeOut();
                timeToWait = Mathf.Max(timeToWait, fadeScript.fadeDuration);
                
            }
            StartCoroutine(LoadAfterTime(timeToWait + 0.2f)); //load new scene

        }
    }

    IEnumerator LoadAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single); //load new scene
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
        }
    }



    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;            
        }
    }
}