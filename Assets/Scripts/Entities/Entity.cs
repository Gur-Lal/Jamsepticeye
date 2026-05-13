using UnityEngine;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{
    [Header("Base Entity Properties:")]
    [SerializeField, Range(1f, 50f)] protected float gravityMult = 30f;
    [SerializeField, Range(0.1f, 1f)] protected float floatJumpGravityMult = 0.25f;
    [SerializeField, Range(0f, 10f)] float gravityMultWhileGrounded = 2f;
    [SerializeField] protected Vector2 MaxVelocities = new Vector2(20f, 20f);
    protected bool IsGrounded;
    protected int IsTouchingWall;
    protected int IsTouchingSlope;
    protected bool IsStandingOnSlope;
    protected int IsTouchingPushableObject;
    protected bool FacingRight;
    protected bool IsIncapacitated;
    protected bool IsFloatJumping;

    //tracking
    protected float LastGroundedTime = Mathf.NegativeInfinity;
    private bool WasOnGroundLastFrame;

    //references
    protected LayerMask entityLayer;
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer spr;
    protected virtual void Awake()
    {
        //Debug.Log("[ENTITY SCRIPT] Awake");
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spr = GetComponent<SpriteRenderer>();
        entityLayer = LayerMask.NameToLayer("Entity");
    }

    protected virtual void FixedUpdate()
    {
        IsGrounded = CheckIfGrounded();
        if (IsGrounded)
        { //on ground
            if (!WasOnGroundLastFrame) OnGroundTouched(); //trigger things that occur when ground is first hit
            LastGroundedTime = Time.time;
            WasOnGroundLastFrame = true;
        }
        else //mid-air
        {
            
            WasOnGroundLastFrame = false;
        }

        //apply grav even if on ground to fix slope y-vel issues
        float effectiveGrav = gravityMult * Time.deltaTime;

        if (IsFloatJumping) effectiveGrav *= floatJumpGravityMult; //apply float jump reduction if relevant

        if (IsGrounded) effectiveGrav *= gravityMultWhileGrounded;

        if (rb.linearVelocityY < 0) effectiveGrav *= 2; //gravity is twice as strong when going down (classic platformer stuff!)
        rb.linearVelocityY -= effectiveGrav;

        CheckIfTouchingWall();

        if (Mathf.Abs(rb.linearVelocityY) > MaxVelocities.y) rb.linearVelocityY = MaxVelocities.y * Mathf.Sign(rb.linearVelocityY);
        if (Mathf.Abs(rb.linearVelocityX) > MaxVelocities.x) rb.linearVelocityX = MaxVelocities.x * Mathf.Sign(rb.linearVelocityX);

        if (IsGrounded && IsStandingOnSlope && rb.linearVelocityY < 0) rb.linearVelocityY = 0; //minimum y vel is 0 when standing on a slope to avoid sliding down them,
    }

    bool CheckIfGrounded()
    {
        Bounds b = col.bounds;

        float lengthOfRay = b.extents.y + 0.01f;

        Vector2 leftPoint = new Vector2(b.min.x, b.min.y + b.extents.y);
        Vector2 rightPoint = new Vector2(b.max.x, b.min.y + b.extents.y);
        Vector2 midPoint = new Vector2(b.center.x, b.min.y + b.extents.y);

        RaycastHit2D[] leftHits = Physics2D.RaycastAll(leftPoint, Vector2.down, lengthOfRay);
        RaycastHit2D[] rightHits = Physics2D.RaycastAll(rightPoint, Vector2.down, lengthOfRay);
        RaycastHit2D[] midHits = Physics2D.RaycastAll(midPoint, Vector2.down, lengthOfRay);

        List<RaycastHit2D> realHits = new List<RaycastHit2D>();

#if UNITY_EDITOR
        Debug.DrawRay(leftPoint, Vector2.down * lengthOfRay, Color.red); Debug.DrawRay(rightPoint, Vector2.down * lengthOfRay, Color.red); Debug.DrawRay(midPoint, Vector2.down * lengthOfRay, Color.red);
#endif

        float floorTolerance = 0.7f; //floor normal is acceptable if above this
        bool IsFloor(RaycastHit2D hit) => hit.normal.y >= floorTolerance && Mathf.Abs(hit.normal.x) <= (1f - floorTolerance);

        foreach (var hit in leftHits) if (hit.collider != null && !hit.collider.isTrigger && hit.collider != col && (IsFloor(hit) || IsSlope(hit)) ) { realHits.Add(hit); } //filter left hits
        foreach(var hit in rightHits) if (hit.collider != null && !hit.collider.isTrigger && hit.collider != col && (IsFloor(hit) || IsSlope(hit)) ) {realHits.Add(hit); } //filter right hits
        foreach(var hit in midHits) if (hit.collider != null && !hit.collider.isTrigger && hit.collider != col && (IsFloor(hit) || IsSlope(hit)) ) {realHits.Add(hit); } //filter mid hits

        //determine if standing on a slope or not for other uses
        IsStandingOnSlope = false;
        foreach(var hit in realHits)
            if (IsSlope(hit))
            {
                IsStandingOnSlope = true;
                break;
            }

        return realHits.Count > 0;
    }

    void CheckIfTouchingWall()
    {
        int sign = 1;
        if (!FacingRight) sign = -1;

        Vector2 TopPos;
        Vector2 BottomPos;
        Vector2 MidPos;
        Vector2 MidTopPos;
        Vector2 MidBotPos;

        float xPos = col.bounds.center.x; //Start the check in the CENTER of the capsule, or it will make mistakes on the foot of slopes! //FacingRight ? col.bounds.max.x : col.bounds.min.x;

        TopPos = new Vector2(xPos, col.bounds.max.y);
        BottomPos = new Vector2(xPos, col.bounds.min.y + 0.15f);
        MidPos = new Vector2(xPos, col.bounds.center.y);
        MidTopPos = new Vector2(xPos, col.bounds.center.y + col.bounds.extents.y*0.5f);
        MidBotPos = new Vector2(xPos, col.bounds.center.y - col.bounds.extents.y*0.5f);

        float rayLength = Mathf.Abs(col.bounds.max.x - col.bounds.min.x)*0.5f + 0.1f;

        RaycastHit2D[] hitsTop = Physics2D.RaycastAll(TopPos, Vector2.right * sign, rayLength);
        RaycastHit2D[] hitsBottom = Physics2D.RaycastAll(BottomPos, Vector2.right * sign, rayLength);
        RaycastHit2D[] hitsMid = Physics2D.RaycastAll(MidPos, Vector2.right * sign, rayLength);
        RaycastHit2D[] hitsMidTop = Physics2D.RaycastAll(MidTopPos, Vector2.right * sign, rayLength);
        RaycastHit2D[] hitsMidBot = Physics2D.RaycastAll(MidBotPos, Vector2.right * sign, rayLength);

#if UNITY_EDITOR
        Debug.DrawRay(TopPos, Vector2.right * sign * rayLength, Color.blue); Debug.DrawRay(BottomPos, Vector2.right * sign * rayLength, Color.blue); Debug.DrawRay(MidPos, Vector2.right * sign * rayLength, Color.blue); Debug.DrawRay(MidBotPos, Vector2.right * sign * rayLength, Color.blue); Debug.DrawRay(MidTopPos, Vector2.right * sign * rayLength, Color.blue);
#endif


        int fullLength = hitsTop.Length + hitsBottom.Length + hitsMid.Length + hitsMidTop.Length + hitsMidBot.Length;
        RaycastHit2D[] allHits = new RaycastHit2D[fullLength];
        int indexRunningTotal = 0;
        hitsTop.CopyTo(allHits, indexRunningTotal);
        indexRunningTotal += hitsTop.Length;
        hitsBottom.CopyTo(allHits, indexRunningTotal);
        indexRunningTotal += hitsBottom.Length;
        hitsMid.CopyTo(allHits, indexRunningTotal);
        indexRunningTotal += hitsMid.Length;
        hitsMidTop.CopyTo(allHits, indexRunningTotal);
        indexRunningTotal += hitsMidTop.Length;
        hitsMidBot.CopyTo(allHits, indexRunningTotal);

        foreach (var hit in allHits)
        {
            if (hit.collider != null && !hit.collider.isTrigger && hit.collider != col)
                if (hit.collider.gameObject.layer != entityLayer)
                {
                    if (IsVerticalWall(hit))
                    {
                        IsTouchingWall = sign;
                        return;
                    }
                    else if (IsSlope(hit))
                    {
                        IsTouchingSlope = sign;
                        return;
                    }
                }
                else if (hit.collider.gameObject.GetComponentInChildren<GrabConnector>() != null)
                {
                    IsTouchingPushableObject = sign;
                    return;
                }
            
        }
        IsTouchingWall = 0;
        IsTouchingSlope = 0;
        IsTouchingPushableObject = 0;
        return;
    }


    bool IsVerticalWall(RaycastHit2D hit)
    {
        Vector2 n = hit.normal;
        return Mathf.Abs(n.x) > 0.99f && Mathf.Abs(n.y) < 0.01f;
    }

    bool IsSlope(RaycastHit2D hit) //checks for pure 45 deg slopes
    {
        Vector2 n = hit.normal;
        return Mathf.Round(Mathf.Abs(n.x)*100f)/100f == Mathf.Round(Mathf.Abs(n.y)*100f) / 100f; //round to nearest 2dp, abs
    }

    protected virtual void OnGroundTouched()
    {
        //pass to child
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

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }
}
