using UnityEngine;
using System.Collections.Generic;

public class DeathDialogueRedEyeAction : IDialogueAction
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void Ping(int id)
    {
        if (!validDialogIDs.Contains(id)) return;

        if (id == validDialogIDs[0]) animator.SetTrigger("Appear");
        if (id == validDialogIDs[1]) animator.gameObject.SetActive(false);
    }
}
