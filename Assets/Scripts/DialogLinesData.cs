using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogSpeakerData", menuName = "Dialogue/DialogLines")]
public class DialogLinesData : ScriptableObject
{
    public List<DialogLine> lines = new List<DialogLine>();
}
