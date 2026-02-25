using UnityEngine;

public interface ISaveable
{
    string getEntityID();
    string getEntityType();
    EntityStateData SaveState();

    void loadState(EntityStateData data);
}
