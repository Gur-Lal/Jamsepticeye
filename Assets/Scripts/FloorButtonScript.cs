using UnityEngine;
using System.Collections.Generic;

public class FloorButtonScript : MonoBehaviour
{
    [Header("Put the result of the button here:")]
    [SerializeField] IButtonActivated ControlledObject;
    Collider2D buttonTriggerCol;
    bool active = false;
    List<Rigidbody2D> objectsOnButton = new List<Rigidbody2D>();
    void Start()
    {
        if (ControlledObject == null) Debug.LogError("[FLOOR BUTTON SCRIPT] Error: Unassigned 'ControlledObject'.");
        buttonTriggerCol = GetComponent<Collider2D>();
    }


    void Activate()
    {
        active = true;
        ControlledObject.OnButtonTrigger();
    }
    void Deactivate()
    {
        active = false;
        ControlledObject.OnButtonDisable();
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
