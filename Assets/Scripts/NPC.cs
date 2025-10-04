using UnityEngine;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    [Header("Dialog Settings")]
    [SerializeField] private string npcName = "Villager";
    [SerializeField] private string[] npcLines;
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
        if (dialogSystem != null && npcLines.Length > 0)
        {
            dialogSystem.SetNPCName(npcName);
            dialogSystem.StartDialog(npcLines);
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