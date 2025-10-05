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
    [Header("Audio")]
    [SerializeField] AudioClip doorOpenSound;
    [SerializeField] AudioClip doorCloseSound;
    [SerializeField, Range(0f, 1f)] float doorSoundVolume = 0.7f;
    
    private List<FloorButtonScript> ButtonsPressed = new List<FloorButtonScript>();
    Collider2D col;
    SpriteRenderer spr;
    Animator animator;
    AudioSource audioSource;
    bool buttonStateOn;
    bool isOpen;
    
    void Start()
    {
        isOpen = Invert;
        col = GetComponent<Collider2D>();
        spr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetBool("StartOpen", Invert);

        //audio source setup
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
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

    void FixedUpdate()
    {
        UpdateState();
    }

    public override void OnButtonTrigger(FloorButtonScript triggered)
    {

        if (!ButtonsPressed.Contains(triggered)) ButtonsPressed.Add(triggered);
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
                //Debug.Log("Door " + this.gameObject.name + "found an illegal button called " + IllegalButton.transform.name);
                return false; //illegal state!
            }
        }

        foreach (var RequiredButton in MandatoryButtonsPressed)
        {
            if (!ButtonsPressed.Contains(RequiredButton))
            {
                //Debug.Log("Door " + this.gameObject.name + "found an missing reuired button called " + RequiredButton.transform.name);
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
        
        //play opening sound
        if (doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound, doorSoundVolume);
        }
    }

    void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        if (col == null) return;
        col.enabled = true;
        animator.SetTrigger("DoorClose");
        
        //play closing sound
        if (doorCloseSound != null)
        {
            audioSource.PlayOneShot(doorCloseSound, doorSoundVolume);
        }
    }
}