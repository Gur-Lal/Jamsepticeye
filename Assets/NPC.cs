using UnityEngine;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    [Header("Dialog Settings")]
    [SerializeField] private string npcName = "Villager";
    [SerializeField] private string[] npcLines;
    [SerializeField] private DialogSystem dialogSystem;
    
    [Header("Interaction Prompt")]
    [SerializeField] private GameObject interactionPrompt; // Optional UI prompt
    
    private bool playerInRange = false;

    void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    void Update()
    {
        // Check for E key press when player is in range
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
    }

    void StartDialog()
    {
        if (dialogSystem != null && npcLines.Length > 0)
        {
            dialogSystem.SetNPCName(npcName);
            dialogSystem.StartDialog(npcLines);
        }
    }

    // Optional: Keep this if you still want to use Input Actions
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange && context.performed)
        {
            StartDialog();
        }
    }
}