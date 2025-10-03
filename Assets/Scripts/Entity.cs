using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField, Range(1f, 50f)] protected float gravityMult = 30f;
    [SerializeField, Range(0.1f, 1f)] protected float floatJumpGravityMult = 0.25f;
    protected bool IsGrounded;
    protected bool FacingRight;
    protected bool IsIncapacitated;
    protected bool IsFloatJumping;

    //tracking
    protected float LastGroundedTime = Mathf.NegativeInfinity;
    private bool WasOnGroundLastFrame;

    //references
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer spr;
    protected virtual void Awake()
    {
        Debug.Log("[ENTITY SCRIPT] Awake");
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        IsGrounded = CheckIfGrounded();
        if (IsGrounded)
        { //on ground
             if (!WasOnGroundLastFrame) OnGroundTouched(); //trigger things that occur when ground is first hit
            LastGroundedTime = Time.time;
            if (rb.linearVelocityY < 0) rb.linearVelocityY = 0; //minimum y vel is 0 when on ground (makes animations snappier)
            WasOnGroundLastFrame = true;
        }
        else //mid-air
        {
            float effectiveGrav = gravityMult * Time.deltaTime;

            if (IsFloatJumping) effectiveGrav *= floatJumpGravityMult; //apply float jump reduction if relevant

            if (rb.linearVelocityY < 0) effectiveGrav *= 2; //gravity is twice as strong when going down (classic platformer stuff!)
            rb.linearVelocityY -= effectiveGrav;
            WasOnGroundLastFrame = false;
        }
    }

    bool CheckIfGrounded()
    {
        Bounds b = col.bounds;

        Vector2 leftPoint = new Vector2(b.min.x, b.min.y);
        Vector2 rightPoint = new Vector2(b.max.x, b.min.y);

        RaycastHit2D leftHit = Physics2D.Raycast(leftPoint, Vector2.down, 0.01f);
        RaycastHit2D rightHit = Physics2D.Raycast(rightPoint, Vector2.down, 0.01f);

        #if UNITY_EDITOR
        Debug.DrawRay(leftPoint, Vector2.down * 0.01f, Color.red); Debug.DrawRay(rightPoint, Vector2.down * 0.01f, Color.red);
        #endif

        if (leftHit.collider != null || rightHit.collider != null) return true; //something is below

        return false;
    }

    protected virtual void OnGroundTouched()
    {
        //pass
    }

    public void FaceRight()
    {
        FacingRight = true;
        spr.flipX = true;
    }

    public void FaceLeft()
    {
        FacingRight = false;
        spr.flipX = false;
    }
}
