using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField, Range(1f, 50f)] protected float gravityMult = 30f;
    protected bool IsGrounded;
    protected bool FacingRight;

    //references
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer spr;
    void Start()
    {
        Debug.Log("[ENTITY SCRIPT] Starting");
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        IsGrounded = CheckIfGrounded();
        if (!IsGrounded)
        {
            float effectiveGrav = gravityMult * Time.deltaTime;
            if (rb.linearVelocityY < 0) effectiveGrav *= 2; //gravity is twice as strong when going down (classic platformer stuff!)
            rb.linearVelocityY -= effectiveGrav;
        }
    }

    bool CheckIfGrounded()
    {
        Bounds b = col.bounds;

        Vector2 leftPoint = new Vector2(b.min.x, b.min.y - 0.01f);
        Vector2 rightPoint = new Vector2(b.max.x, b.min.y - 0.01f);

        Collider2D leftHit = Physics2D.OverlapPoint(leftPoint);
        Collider2D rightHit = Physics2D.OverlapPoint(rightPoint);

        if (leftHit != null || rightHit != null) return true; //something is below

        return false;
    }

    protected void FaceRight()
    {
        FacingRight = true;
        spr.flipX = true;
    }

    protected void FaceLeft()
    {
        FacingRight = false;
        spr.flipX = false;
    }
}
