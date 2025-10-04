using System.Linq.Expressions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorScript : IButtonActivated
{
    [SerializeField] bool Invert = false; //false = start closed, isOpen when buttons hit. true = start isOpen, close when buttons hit.
    [SerializeField, Range(0f, 0.5f)] float InputRejectionDelay = 0.05f;
    [SerializeField] List<FloorButtonScript> MandatoryButtonsPressed = new List<FloorButtonScript>();
    [SerializeField] List<FloorButtonScript> IllegalButtonsPressed = new List<FloorButtonScript>();
    private List<FloorButtonScript> ButtonsPressed = new List<FloorButtonScript>();
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

    public override void OnButtonTrigger(FloorButtonScript triggered)
    {
        ButtonsPressed.Add(triggered);
        UpdateState();
    }

    public override void OnButtonDisable(FloorButtonScript triggered)
    {
        ButtonsPressed.Remove(triggered);
        UpdateState();
    }

    void UpdateState()
    {
        bool doorShouldBeOpen = CheckState();

        if (!Invert)
        {
            if (!doorShouldBeOpen && isOpen) Close();

            if (doorShouldBeOpen && !isOpen) Open();
        }
        else //opposite configuration
        {
            if (!doorShouldBeOpen && isOpen) Open();

            if (doorShouldBeOpen && !isOpen) Close();
        }
    }

    bool CheckState()
    {
        foreach (var IllegalButton in IllegalButtonsPressed)
        {
            if (ButtonsPressed.Contains(IllegalButton))
            {
                return false; //illegal state!
            }
        }


        foreach (var RequiredButton in MandatoryButtonsPressed)
        {
            if (!ButtonsPressed.Contains(RequiredButton))
            {
                return false; //a required button is misisng
            }
        }

        return true; //no illegal entrances, and all mandatory entrances!
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
