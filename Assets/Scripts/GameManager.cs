using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (Input.GetKeyDown(KeyCode.F5))
            SaveLoadManager.Instance.SaveGame(currentLevelName, Player.position);

        if (Input.GetKeyDown(KeyCode.F9))
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
