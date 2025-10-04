using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class FloorButtonScript : MonoBehaviour
{
    [Header("Put the result of the button here:")]
    [SerializeField] IButtonActivated ControlledObject;
    [Header("Config stuff")]
    [SerializeField] Sprite upSprite;
    [SerializeField] Sprite downSprite;
    Collider2D buttonTriggerCol;
    SpriteRenderer spr;
    List<Rigidbody2D> objectsOnButton = new List<Rigidbody2D>();
    void Start()
    {
        if (ControlledObject == null) Debug.LogError("[FLOOR BUTTON SCRIPT] Error: Unassigned 'ControlledObject'. You forgot to attach a door to this button! -J");
        buttonTriggerCol = GetComponent<Collider2D>();
        spr = GetComponentInChildren<SpriteRenderer>();
        spr.sprite = upSprite;
    }


    void Activate()
    {
        spr.sprite = downSprite;
        ControlledObject.OnButtonTrigger(this);
    }
    void Deactivate()
    {
        spr.sprite = upSprite;
        ControlledObject.OnButtonDisable(this);
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
