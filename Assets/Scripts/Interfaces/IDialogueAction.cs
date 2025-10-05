using UnityEngine;
using System.Collections.Generic;

public abstract class IDialogueAction : MonoBehaviour
{
    [SerializeField] protected List<int> validDialogIDs;
    public abstract void Ping(int dialogueIndex);
}
