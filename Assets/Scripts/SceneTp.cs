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
            FadeToFromBlack fadeScript = FindFirstObjectByType<FadeToFromBlack>();
            if (fadeScript != null) { fadeScript.FadeOut(); StartCoroutine(LoadAfterTime(fadeScript.fadeDuration+0.2f)); }
            else SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single); //load new scene

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