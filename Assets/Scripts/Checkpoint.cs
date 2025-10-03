using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    Collider2D col;
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) other.GetComponent<PlayerController>().SetSpawnPoint(transform.position);
    }
}
