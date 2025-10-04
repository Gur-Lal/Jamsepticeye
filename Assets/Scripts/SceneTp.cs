using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTp : MonoBehaviour
{
    public string sceneToLoad; // Name of the scene to load 
    private string currentSceneName; // To track the current scene
    private bool playerIsNearby = false;

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    void Update() {
        if (playerIsNearby && PlayerController.input.Player.Interact.triggered) {
            
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive); // Load new scene additively
           // SceneManager.sceneLoaded += OnSceneLoaded; // Register callback for scene load completion
        }
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;

            
            //DontDestroyOnLoad(other.gameObject); // Prevent player object from being destroyed

            
        }
    }



    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;            
        }
    }

    /**void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == sceneToLoad)
        {
            // Find the spawn point in the newly loaded scene
            GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
            if (spawnPoint != null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    // Teleport the player to the spawn point
                    player.transform.position = spawnPoint.transform.position;
                }
            }

            // Unload the previous scene
            SceneManager.UnloadSceneAsync(currentSceneName);

            // Update the current scene name and unregister the event handler
            currentSceneName = sceneToLoad;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
}

**/

}