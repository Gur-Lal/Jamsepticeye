using UnityEngine;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
    [SerializeField, Range(1f, 50f)] protected float gravityMult = 30f;
    [SerializeField, Range(0.1f, 1f)] protected float floatJumpGravityMult = 0.25f;
    protected bool IsGrounded;
    protected int IsTouchingWall;
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
        //Debug.Log("[ENTITY SCRIPT] Awake");
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    protected virtual void FixedUpdate()
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

        IsTouchingWall = CheckIfTouchingWall();
    }

    bool CheckIfGrounded()
    {
        Bounds b = col.bounds;

        Vector2 leftPoint = new Vector2(b.min.x, b.min.y);
        Vector2 rightPoint = new Vector2(b.max.x, b.min.y);

        RaycastHit2D[] leftHits = Physics2D.RaycastAll(leftPoint, Vector2.down, 0.01f);
        RaycastHit2D[] rightHits = Physics2D.RaycastAll(rightPoint, Vector2.down, 0.01f);

        List<Collider2D> realHits = new List<Collider2D>();

#if UNITY_EDITOR
        Debug.DrawRay(leftPoint, Vector2.down * 0.01f, Color.red); Debug.DrawRay(rightPoint, Vector2.down * 0.01f, Color.red);
#endif

        float floorTolerance = 0.7f; //floor normal is acceptable if above this
        bool IsFloor(RaycastHit2D hit) => hit.normal.y >= floorTolerance && Mathf.Abs(hit.normal.x) <= (1f - floorTolerance);

        foreach (var hit in leftHits) if (hit.collider != null && !hit.collider.isTrigger && hit.collider != col && IsFloor(hit)) { realHits.Add(hit.collider); return true; } //filter left hits
        foreach(var hit in rightHits) if (hit.collider != null && !hit.collider.isTrigger && hit.collider != col && IsFloor(hit)) {realHits.Add(hit.collider); return true;} //if no left hits, filter right hits

        return false;
    }

    int CheckIfTouchingWall()
    {
        int sign = 1;
        if (!FacingRight) sign = -1;

        Vector2 TopPos;
        Vector2 BottomPos;
        Vector2 MidPos;

        if (FacingRight)
        {
            TopPos = new Vector2(col.bounds.center.x, col.bounds.max.y);
            BottomPos = new Vector2(col.bounds.center.x, col.bounds.min.y);
            MidPos = new Vector2(col.bounds.center.x, col.bounds.center.y);
        }
        else
        {
            TopPos = new Vector2(col.bounds.center.x, col.bounds.max.y);
            BottomPos = new Vector2(col.bounds.center.x, col.bounds.min.y);
            MidPos = new Vector2(col.bounds.center.x, col.bounds.center.y);
        }

        float rayLength = Mathf.Abs(col.bounds.max.x - col.bounds.min.x)*0.5f + 0.1f;

        RaycastHit2D[] hitsTop = Physics2D.RaycastAll(TopPos, Vector2.right * sign, rayLength);
        RaycastHit2D[] hitsBottom = Physics2D.RaycastAll(BottomPos, Vector2.right * sign, rayLength);
        RaycastHit2D[] hitsMid = Physics2D.RaycastAll(MidPos, Vector2.right * sign, rayLength);

#if UNITY_EDITOR
        Debug.DrawRay(TopPos, Vector2.right * sign * rayLength, Color.blue); Debug.DrawRay(BottomPos, Vector2.right * sign * rayLength, Color.blue); Debug.DrawRay(MidPos, Vector2.right * sign * rayLength, Color.blue);
#endif


        RaycastHit2D[] allHits = new RaycastHit2D[hitsTop.Length + hitsBottom.Length + hitsMid.Length];
        hitsTop.CopyTo(allHits, 0);
        hitsBottom.CopyTo(allHits, hitsTop.Length);
        hitsMid.CopyTo(allHits, hitsBottom.Length + hitsTop.Length);

        foreach (var hit in allHits)
        {
            if (hit.collider != null && !hit.collider.isTrigger && hit.collider != col && IsVerticalWall(hit)) return sign;
        }
        return 0;
    }


    bool IsVerticalWall(RaycastHit2D hit)
    {
        Vector2 n = hit.normal;
        return Mathf.Abs(n.x) > 0.99f && Mathf.Abs(n.y) < 0.01f;
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
