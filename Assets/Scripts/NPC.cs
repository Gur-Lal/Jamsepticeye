using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable] public struct DialogLine
{
    [TextArea] public string Line;
    public string SpeakerName;
    public Sprite speakerImage;
    public TMP_FontAsset Font;
}

public class NPC : MonoBehaviour
{
    [Header("Dialog Settings")]
    [SerializeField] private DialogLine[] dialogLines;
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
            StartDialog();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactionPrompt != null)
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
        dialogSystem.EndDialog();
    }

    void StartDialog()
    {
        if (dialogSystem != null && dialogLines.Length > 0)
        {
            dialogSystem.StartDialog(dialogLines);
        }
    }

    //InputAction handler. Move this jurisdiction over to the player controller at some point?
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange && context.performed && !dialogSystem.Busy)
        {
            StartDialog();
            interactionPrompt.SetActive(false);
        }
    }
}