using UnityEngine;

public abstract class IButtonActivated : MonoBehaviour
{
    public abstract void OnButtonTrigger(FloorButtonScript triggered);

    public abstract void OnButtonDisable(FloorButtonScript triggered);
}
