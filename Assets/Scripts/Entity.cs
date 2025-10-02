using UnityEngine;

public class Entity : MonoBehaviour
{
    protected bool IsGrounded;

    //references
    protected Rigidbody2D rb;
    void Start()
    {
        Debug.Log("[ENTITY SCRIPT] Starting");
        rb = GetComponent<Rigidbody2D>();
    }
}
