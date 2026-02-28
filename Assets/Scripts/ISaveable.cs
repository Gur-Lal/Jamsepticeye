using UnityEngine;

public interface ISaveable
{
    string GetEntityID();
    string GetEntityType();
    EntityStateData SaveState();

    void loadState(EntityStateData data);
}
