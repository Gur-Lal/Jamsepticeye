using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : Entity
{
    //config vars
    [SerializeField, Range(1f, 15f)] float moveSpeed = 7f;
    [SerializeField, Range(1f, 30f)] float jumpForce = 15f;
    [SerializeField, Range(0.05f, 0.2f)] float coyoteTime = 0.1f;
    [SerializeField, Range(0.1f, 0.5f)] float jumpBuffer = 0.1f;
    [SerializeField] GameObject corpsePrefab;

    //reference vars
    private PlayerActionControls input;
    private Animator animator;

    //tracking vars
    private Vector2 spawnPoint;
    float horizontalMovement;
    float LastJumpRequestTime = Mathf.NegativeInfinity;

    protected override void Awake()
    {
        base.Awake();
        //start up input handler
        input = new PlayerActionControls();
        input.Player.Enable();

        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update(); //base entity physics

        if (IsIncapacitated) return;

        if (input.Player.Jump.triggered)
        {
            Jump();
        }

        horizontalMovement = input.Player.Move.ReadValue<Vector2>().x;
        if (horizontalMovement > 0) FaceRight();
        else if (horizontalMovement < 0) FaceLeft();

        //if (horizontalMovement != 0) Debug.Log("HorizontalMovement = " + horizontalMovement + ", becoming " + (Vector2.right * horizontalMovement * moveSpeed * Time.deltaTime));
        rb.linearVelocityX = horizontalMovement * moveSpeed; //* Time.deltaTime;

        animator.SetFloat("XVel", Mathf.Abs(horizontalMovement));
        animator.SetFloat("YVel", rb.linearVelocityY);

    }

    public void Jump()
    {
        LastJumpRequestTime = Time.time;

        if (IsGrounded || (Time.time - LastGroundedTime < coyoteTime))
        {
            rb.linearVelocityY = 0; //reset y vel
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        }
    }

    protected override void OnGroundTouched()
    {
        if (Time.time - LastJumpRequestTime < jumpBuffer) Jump(); //jump buffer system, neato
    }

    public void Die()
    {
        if (IsIncapacitated) return;
        IsIncapacitated = true;
        rb.linearVelocityX = 0;
        rb.linearVelocityY = 0;
        animator.SetTrigger("Death");
        animator.SetBool("IsDead", true);
        StartCoroutine(WaitForDeathAnimation(1f));
    }

    public void SetSpawnPoint(Vector2 newPos)
    {
        spawnPoint = newPos;
    }

    void Respawn()
    {
        transform.position = spawnPoint;
        rb.linearVelocity = Vector2.zero;
        IsIncapacitated = false;
        animator.SetBool("IsDead", false);
        animator.SetTrigger("Respawn");
    }

    public IEnumerator WaitForDeathAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        SpawnCorpse();
        Respawn();
    }

    [ContextMenu("SpawnCorpse")]
    void SpawnCorpse()
    {
        GameObject corpseObj = GameObject.Instantiate(corpsePrefab, transform.position, Quaternion.identity);
        Entity entScript = corpseObj.GetComponent<Entity>();
        if (FacingRight) entScript.FaceRight();
        else entScript.FaceLeft();
    }
}
