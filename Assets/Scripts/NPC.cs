using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable] public struct DialogLine
{
    public DialogSpeakerData speakerData;
    [TextArea] public string Line;
    
}

public class NPC : MonoBehaviour
{
    [Header("Dialog Settings")]
    [SerializeField] private bool CanBeInterrupted = true;
    private bool HidePrompt = false;
    [SerializeField] private DialogLinesData dialogLines;
    [SerializeField] private DialogSystem dialogSystem;

    [Header("Interaction Prompt")]
    [SerializeField] private GameObject interactionPrompt;

    private bool playerInRange = false;

    void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            interactionPrompt.SetActive(false);
            StartDialog();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactionPrompt != null && !HidePrompt)
                interactionPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }
        if (CanBeInterrupted) dialogSystem.EndDialog();
    }

    void StartDialog()
    {
        if (dialogSystem != null && dialogLines.lines.Count > 0)
        {
            if (!CanBeInterrupted) HidePrompt = true; //avoids prompt re-appearing if you leave and come back while he's still talking
            dialogSystem.StartDialog(dialogLines.lines, this);
        }
    }

    public void EndOfDialogue() //called by dialogue system when it ends
    {
        HidePrompt = false;
    }

}