using UnityEngine;
using System.Collections.Generic;

public class CameraOffsetDDialog : IDialogueAction
{
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
        if (id == validDialogIDs[0]) scr.Yoffset = 0f;
        if (id == validDialogIDs[-1]) scr.Yoffset = original;
    }
}
