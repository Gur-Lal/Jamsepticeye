using UnityEngine;

public class HarmObject : MonoBehaviour
{
    Collider2D trigCol;
    void Start()
    {
        trigCol = GetComponent<Collider2D>();
        trigCol.isTrigger = true;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerController>().Die();
    }
}
