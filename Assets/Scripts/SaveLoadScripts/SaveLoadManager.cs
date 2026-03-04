using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager _instance;
    public static SaveLoadManager Instance
    {
        get { return _instance; }
    }

    private string saveFilePath;
    private GameSaveData currentSaveData;

    private List<ISaveable> saveableObjects = new List<ISaveable>();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        saveFilePath = Application.persistentDataPath + "/gamesave.json";
        Debug.Log("Save file path: " + saveFilePath);
    }

    public void RegisterSaveable(ISaveable saveable)
    {
        if (!saveableObjects.Contains(saveable))
        {
            saveableObjects.Add(saveable);
            Debug.Log(saveable.GetEntityID() + " added to save system");
        }
    }

    public void UnregisterSaveable(ISaveable saveable)
    {
        saveableObjects.Remove(saveable);
        Debug.Log(saveable.GetEntityID() + " Remove to save system");
    }

    public void SaveGame(string levelName, Vector2 playerPos)
    {
        Debug.Log("the game is being saved...");

        currentSaveData = new GameSaveData();
        currentSaveData.currentLevelName = levelName;
        currentSaveData.playerPosition = new Vector2Data (playerPos);

        foreach (ISaveable saveable in saveableObjects)
        {
            EntityStateData entityData = saveable.SaveState();

            if (entityData != null)
            {
            currentSaveData.entityStates.Add(entityData);
            }
        }

        string json = JsonUtility.ToJson(currentSaveData, true);

        File.WriteAllText(saveFilePath, json);

        Debug.Log("Game saved! Total " + currentSaveData.entityStates.Count + " object saved");
        Debug.Log(json);
    }
    
    public GameSaveData LoadGame()
    {
        Debug.Log("game loading...");

        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("Save file not found!");
            return null;
        }

        string json = File.ReadAllText(saveFilePath);

        currentSaveData = JsonUtility.FromJson<GameSaveData>(json);

        Debug.Log("Game loaded! Level: " + currentSaveData.currentLevelName);
        Debug.Log("Number of objects uploaded: " + currentSaveData.entityStates.Count);

        return currentSaveData;
    }

    public void ApplyLoadedData()
    {
        if (currentSaveData == null)
        {
            Debug.LogWarning("No registration data available!");
            return;
        }
        Debug.Log("The recorded data is being applied to objects...");

        foreach (EntityStateData entityData in currentSaveData.entityStates)
        {
            ISaveable saveable = saveableObjects.Find(s => s.GetEntityID() == entityData.entityID);

            if (saveable != null)
            {
                saveable.loadState(entityData);
                Debug.Log(entityData.entityID + " status uploaded");
            }
            else
            {
                Debug.LogWarning(entityData.entityID + " Couldn't find it on scene!");
            }
        }
        Debug.Log("All data has been applied!");
    }

    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    public string GameSavedLevelName()
    {
        if (currentSaveData != null)
            return currentSaveData.currentLevelName;
        else
            return "";
    }

    public Vector2 GetSavedPlayerPosition()
    {
        if (currentSaveData != null && currentSaveData.playerPosition !=null)
        {
            return currentSaveData.playerPosition.ToVector2();
        }
        else
            return Vector2.zero;
    }
    
}
