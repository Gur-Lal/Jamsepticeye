using UnityEngine;
using System.Collections.Generic;

public class CameraOffsetDDialog : IDialogueAction
{
    [SerializeField] float YoffsetVal = 0f;
    CameraFollow scr;
    float original;
    float originalSize;
    void Start()
    {
        scr = GetComponent<CameraFollow>();
        original = scr.Yoffset;
    }

    public override void Ping(int id)
    {
        if (id == 0) scr.Yoffset = YoffsetVal;
        if (id == -1) scr.Yoffset = original;
    }
}
