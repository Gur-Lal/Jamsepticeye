using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : Entity
{
    //config vars
    [SerializeField, Range(1f, 15f)] float moveSpeed = 7f;
    [SerializeField, Range(1f, 30f)] float jumpForce = 15f;
    [SerializeField] GameObject corpsePrefab;

    //reference vars
    private PlayerActionControls input;
    private Animator animator;

    //tracking vars
    private Vector2 spawnPoint;
    float horizontalMovement;

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

        if (input.Player.Jump.triggered && IsGrounded)
        {
            Jump();
        }

        horizontalMovement = input.Player.Move.ReadValue<Vector2>().x;
        if (horizontalMovement > 0) FaceRight();
        else if (horizontalMovement < 0) FaceLeft();

        //if (horizontalMovement != 0) Debug.Log("HorizontalMovement = " + horizontalMovement + ", becoming " + (Vector2.right * horizontalMovement * moveSpeed * Time.deltaTime));
        transform.position += Vector3.right * horizontalMovement * moveSpeed * Time.deltaTime;

        animator.SetFloat("XVel", Mathf.Abs(horizontalMovement));
        animator.SetFloat("YVel", rb.linearVelocityY);

    }

    public void Jump()
    {
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
    }

    public void Die()
    {
        if (IsIncapacitated) return;
        IsIncapacitated = true;
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
