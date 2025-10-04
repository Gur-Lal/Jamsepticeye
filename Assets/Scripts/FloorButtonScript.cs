using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class FloorButtonScript : MonoBehaviour
{
    [Header("Put the result of the button here:")]
    [SerializeField] List<IButtonActivated> ControlledObjects = new List<IButtonActivated>();
    [Header("Config stuff")]
    [SerializeField] Sprite upSprite;
    [SerializeField] Sprite downSprite;
    [Header("Audio")]
    [SerializeField] AudioClip buttonPressSound;
    [SerializeField] AudioClip buttonReleaseSound;
    [SerializeField, Range(0f, 1f)] float buttonSoundVolume = 0.6f;
    
    Collider2D buttonTriggerCol;
    SpriteRenderer spr;
    AudioSource audioSource;
    List<Rigidbody2D> objectsOnButton = new List<Rigidbody2D>();
    
    void Start()
    {
        if (ControlledObjects.Count == 0) Debug.LogError("[FLOOR BUTTON SCRIPT] Error: Unassigned 'ControlledObjects'. You forgot to attach a door to this button! -J");
        buttonTriggerCol = GetComponent<Collider2D>();
        spr = GetComponentInChildren<SpriteRenderer>();
        spr.sprite = upSprite;

        // AudioSource add
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; //2D sound
    }

    void Activate()
    {
        spr.sprite = downSprite;
        // Play press sound
        if (buttonPressSound != null)
        {
            audioSource.PlayOneShot(buttonPressSound, buttonSoundVolume);
        }
        
        foreach (IButtonActivated ControlledObject in ControlledObjects){
            ControlledObject.OnButtonTrigger(this);
        }
    }
    
    void Deactivate()
    {
        spr.sprite = upSprite;
        
        // Play release sound
        if (buttonReleaseSound != null)
        {
            audioSource.PlayOneShot(buttonReleaseSound, buttonSoundVolume);
        }
        
        foreach (IButtonActivated ControlledObject in ControlledObjects){
            ControlledObject.OnButtonDisable(this);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D otherRB = other.GetComponent<Rigidbody2D>();
        if (otherRB != null && otherRB.bodyType == RigidbodyType2D.Dynamic)
        {
            //if the other component has a dynamic rigidbody, its considered a weighted object
            if (objectsOnButton.Count == 0) Activate();
            objectsOnButton.Add(otherRB);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D otherRB = other.GetComponent<Rigidbody2D>();
        if (otherRB != null && otherRB.bodyType == RigidbodyType2D.Dynamic)
        {
            //if the other component has a dynamic rigidbody, its considered a weighted object
            objectsOnButton.Remove(otherRB);
            if (objectsOnButton.Count == 0) Deactivate();
        }
    }
}