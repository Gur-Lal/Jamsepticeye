using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTp : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; //name of the scene to load
    private bool playerIsNearby = false;

    void Update() {
        if (playerIsNearby && PlayerController.input.Player.Interact.triggered) {

            PlayerController.input.Disable();
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single); //load new scene
        }
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