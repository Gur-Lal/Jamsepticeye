using System.Linq.Expressions;
using UnityEngine;
using System.Collections;

public class DoorScript : IButtonActivated
{
    [SerializeField] bool Invert = false; //false = start closed, isOpen when buttons hit. true = start isOpen, close when buttons hit.
    [SerializeField, Range(0f, 0.5f)] float InputRejectionDelay = 0.05f;
    Collider2D col;
    SpriteRenderer spr;
    Animator animator;
    bool buttonStateOn;
    bool isOpen;
    void Start()
    {
        isOpen = Invert;
        col = GetComponent<Collider2D>();
        spr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetBool("StartOpen", Invert);

    }

    IEnumerator ChangeStateAfterDelay(bool DesiredState)
    {
        yield return new WaitForSeconds(InputRejectionDelay);
        if (buttonStateOn == DesiredState)  //if the button state is STILL what it was before the delay, set the new state
        {
            if (Invert) DesiredState = !DesiredState;

            if (DesiredState) Open();
            else Close();
        }


    }

    public override void OnButtonTrigger()
    {
        buttonStateOn = true;
        StartCoroutine(ChangeStateAfterDelay(true)); //in X seconds, if the button's state is still this desired state, apply change
        //if (Invert) Close();
        //else Open();
    }

    public override void OnButtonDisable()
    {
        buttonStateOn = false;
        StartCoroutine(ChangeStateAfterDelay(false)); //in X seconds, if the button's state is still this desired state, apply change
        //StartCoroutine(ReleaseAfterDelay(Invert));
        //if (Invert) Open();
        //else Close();
    }

    void Open()
    {
        if (isOpen) return;
        isOpen = true;
        col.enabled = false;
        animator.SetTrigger("DoorOpen");

    }

    void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        col.enabled = true;
        animator.SetTrigger("DoorClose");

    }

}
