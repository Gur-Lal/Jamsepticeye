using TMPro;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogSpeakerData", menuName = "Dialogue/DialogSpeakerData")]
public class DialogSpeakerData : ScriptableObject
{
    public string speakerName = "???";
    public Sprite portrait;
    public TMP_FontAsset font;
    public float secondsPerLetterTyped = 0.03f;
    public bool textShake = false;
}

