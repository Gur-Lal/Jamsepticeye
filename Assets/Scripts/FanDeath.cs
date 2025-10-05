using UnityEngine;

public class FanDeath : MonoBehaviour
{
    [SerializeField] GameObject prompt;
    private bool playerIsNearby = false;
    private Collider2D playerCol;

    void Start()
    {
        if (prompt != null) prompt.SetActive(false);
    }

    void Update()
    {
        if (playerIsNearby)
        {
            if (prompt != null) prompt.SetActive(true);

            if (playerCol != null && PlayerController.input.Player.Interact.triggered)
            {
                playerCol.GetComponent<PlayerController>().Die();
            }
        }
        else
        {
            if (prompt != null) prompt.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCol = other;
            playerIsNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCol = other;
            playerIsNearby = false;            
        }
    }
}
