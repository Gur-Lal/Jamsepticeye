using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Entity
{
    //config vars
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;

    //reference vars
    private PlayerActionControls input;

    //tracking vars
    float horizontalMovement;

    void Awake()
    {
        //start up input handler
        input = new PlayerActionControls();
        input.Player.Enable();
    }

    void Update()
    {
        if (input.Player.Jump.triggered)
        {
            Jump();
        }

        if (horizontalMovement != 0) Debug.Log("HorizontalMovement = " + horizontalMovement + ", becoming " + (Vector2.right * horizontalMovement * moveSpeed * Time.deltaTime));
        transform.Translate(Vector2.right * horizontalMovement * moveSpeed * Time.deltaTime);

    }
    public void Move(InputAction.CallbackContext context)
    {
        Debug.Log("MOVE CALLED");
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump()
    {
        Debug.Log("[PLAYER CONTROLLER] Jumping");
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
    }
}
