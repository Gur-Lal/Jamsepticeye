using UnityEngine;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    [SerializeField] private string[] npcLines;
    private DialogSystem dialogSystem;
    private bool playerInRange = false;

    void Start()
    {
        dialogSystem = FindAnyObjectByType<DialogSystem>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange && context.performed)
        {
            dialogSystem.StartDialog(npcLines);
        }
    }
}
