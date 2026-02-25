using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class GameSaveData
{
    public int currentLevelID;
    public Vector2Data playerPosition;
    public List<EntityStateData> entityStates;

    public GameSaveData()
    {
        entityStates = new List<EntityStateData>();
    }
}
[System.Serializable]
public class Vector2Data
{
    public float x;
    public float y;
    public Vector2Data(Vector2 vector)
    {
        x = vector.x;
        y = vector.y; 
    }
    public Vector2 ToVector2()
    {
        return new Vector2(x,y);
    }
    }

    [System.Serializable]
    public class EntityStateData
    {
    public string entityID;
    public string entityType;
    public Vector2Data position;
    public string customDataJson;

    public EntityStateData(string id, string type, Vector2 pos)
    {
        entityID = id;
        entityType = type;
        position = new Vector2Data(pos);
        customDataJson = "{}";
    }
}

