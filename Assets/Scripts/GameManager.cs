using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    [SerializeField] Transform Player;

    string currentLevelName => SceneManager.GetActiveScene().name;

    void Awake()
    {
        if (Instance != null && Instance != this) {Destroy(gameObject); return;}
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Update()
    {
        if (Keyboard.current.f5Key.wasPressedThisFrame)
            SaveLoadManager.Instance.SaveGame(currentLevelName, Player.position);

        if (Keyboard.current.f9Key.wasPressedThisFrame)
            StartCoroutine(LoadGameRoutine());
    }

    IEnumerator LoadGameRoutine()
    {
        var saveData = SaveLoadManager.Instance.LoadGame();
        if (saveData == null) yield break;

        AsyncOperation op = SceneManager.LoadSceneAsync(saveData.currentLevelName);
        yield return op;

        yield return null;

        SaveLoadManager.Instance.ApplyLoadedData();
        
        Player = GameObject.FindWithTag("Player").transform;
        Player.position = SaveLoadManager.Instance.GetSavedPlayerPosition();
    }
}
