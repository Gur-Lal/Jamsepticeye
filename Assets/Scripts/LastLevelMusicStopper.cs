using UnityEngine;
using UnityEngine.SceneManagement;

public class LastLevelMusicStopper : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicPlayer m = FindFirstObjectByType<MusicPlayer>();
        if (m == null) return;
        m.PersistAcrossScenes = false; //tell it to fade out on load end

        Scene CurrentScene = SceneManager.GetActiveScene();
        SceneManager.MoveGameObjectToScene(m.gameObject, CurrentScene); //get it out of the dont-destroy-on-load bit
    }
    
}
