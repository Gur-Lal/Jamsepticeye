using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Entity
{
    //config vars
    [SerializeField, Range(1f, 15f)] float moveSpeed = 7f;
    [SerializeField, Range(1f, 30f)] float jumpForce = 15f;
    [SerializeField] GameObject corpsePrefab;

    //reference vars
    private PlayerActionControls input;

    //tracking vars
    private Vector2 spawnPoint;
    float horizontalMovement;

    void Awake()
    {
        //start up input handler
        input = new PlayerActionControls();
        input.Player.Enable();
    }

    protected override void Update()
    {
        base.Update(); //base entity physics

        if (input.Player.Jump.triggered && IsGrounded)
        {
            Jump();
        }

        horizontalMovement = input.Player.Move.ReadValue<Vector2>().x;
        if (horizontalMovement > 0) FaceRight();
        else if (horizontalMovement < 0) FaceLeft();

        //if (horizontalMovement != 0) Debug.Log("HorizontalMovement = " + horizontalMovement + ", becoming " + (Vector2.right * horizontalMovement * moveSpeed * Time.deltaTime));
        transform.position += Vector3.right * horizontalMovement * moveSpeed * Time.deltaTime;

    }

    public void Jump()
    {
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
    }

    public void Die()
    {
        SpawnCorpse();
        Respawn();
    }

    public void SetSpawnPoint(Vector2 newPos)
    {
        spawnPoint = newPos;
    }

    void Respawn()
    {
        transform.position = spawnPoint;
        rb.linearVelocity = Vector2.zero;
    }

    [ContextMenu("SpawnCorpse")]
    void SpawnCorpse()
    {
        GameObject corpseObj = GameObject.Instantiate(corpsePrefab, transform.position, Quaternion.identity);
    }
}
