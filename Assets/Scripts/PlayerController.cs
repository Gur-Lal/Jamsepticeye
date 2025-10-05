using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : Entity
{
    //config vars
    [Header("Main Config")]
    [SerializeField] Vector2 spawnPoint;
    [SerializeField, Range(1f, 15f)] float moveSpeed = 7f;
    [SerializeField, Range(1f, 30f)] float jumpForce = 11f;
    [SerializeField, Range(0.1f, 0.4f)] float maxJumpHoldTime = 0.35f;
    [SerializeField, Range(0.05f, 0.2f)] float coyoteTime = 0.1f;
    [SerializeField, Range(0.1f, 0.5f)] float jumpBuffer = 0.1f;
    
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField, Range(0.1f, 1f)] float footstepInterval = 0.4f;
    [SerializeField, Range(0f, 1f)] float footstepVolume = 0.5f;
    [SerializeField, Range(0.8f, 1.2f)] float footstepPitchMin = 0.9f;
    [SerializeField, Range(0.8f, 1.2f)] float footstepPitchMax = 1.1f;
    
    [Header("References")]
    [SerializeField] GameObject corpsePrefab;
    
    [Header("Debug Stuff")]
    [SerializeField] float DeathRespawnDelay = 1f;

    //reference vars
    static public PlayerActionControls input;
    private Animator animator;

    //tracking vars
    float horizontalMovement;
    float LastJumpRequestTime = Mathf.NegativeInfinity;
    bool isJumping;
    float jumpHoldTimer;

    bool isAlmostGrounded; //used for avoiding fall/jump animations on stairs
    [SerializeField] float isAlmostGroundedDelay = 0.1f;
    float lastGroundedTime;

    // Footstep tracking
    private float footstepTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        //start up input handler
        input = new PlayerActionControls();
        input.Player.Enable();

        animator = GetComponent<Animator>();
        
        //audio source setup
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D sound
        }
    }

    protected void Update()
    {
        if (IsGrounded) { isAlmostGrounded = true; lastGroundedTime = Time.time; }
        else if (isAlmostGrounded && Time.time - lastGroundedTime > isAlmostGroundedDelay) isAlmostGrounded = false;

        if (IsIncapacitated) return;

        //Receive inputs
        if (input.Player.Jump.triggered) Jump(); //trigger start of a jump

        if (isJumping && input.Player.Jump.IsPressed() && jumpHoldTimer > 0f) //detect if holding jump
        {
            IsFloatJumping = true;
            jumpHoldTimer -= Time.deltaTime;
        }
        else
        {
            IsFloatJumping = false;
            jumpHoldTimer = 0; //if you release jump, you can't float later. Consider removing this for a different platforming feel.
        }

        if (input.Player.Jump.WasReleasedThisFrame()) //trigger end of a jump input
        {
            isJumping = false;
            IsFloatJumping = false;
            jumpHoldTimer = 0;
        }

        horizontalMovement = input.Player.Move.ReadValue<Vector2>().x;
        if (horizontalMovement > 0) FaceRight();
        else if (horizontalMovement < 0) FaceLeft();

        if (IsTouchingWall == 1 && horizontalMovement > 0) horizontalMovement = 0; //prevent moving into walls (avoids wall cling)
        else if (IsTouchingWall == -1 && horizontalMovement < 0) horizontalMovement = 0;

        //if (horizontalMovement != 0) Debug.Log("HorizontalMovement = " + horizontalMovement + ", becoming " + (Vector2.right * horizontalMovement * moveSpeed * Time.deltaTime));
        rb.linearVelocityX = horizontalMovement * moveSpeed; //* Time.deltaTime;

        animator.SetBool("IsGrounded", isAlmostGrounded);
        animator.SetFloat("XVel", Mathf.Abs(horizontalMovement));
        animator.SetFloat("YVel", rb.linearVelocityY);
        
        // Footstep sound handler
        HandleFootsteps();
    }
    // Footstep sound handler
    private void HandleFootsteps()
    {
        bool isMoving = Mathf.Abs(horizontalMovement) > 0.1f;
        
        if (IsGrounded && isMoving && footstepSounds != null && footstepSounds.Length > 0)
        {
            footstepTimer += Time.deltaTime;
            
            if (footstepTimer >= footstepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }
    private void PlayFootstepSound()
    {
        if (audioSource == null || footstepSounds == null || footstepSounds.Length == 0)
            return;
        
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        //randomize pitch a bit for variety
        audioSource.pitch = Random.Range(footstepPitchMin, footstepPitchMax);
        //play sound
        audioSource.PlayOneShot(clip, footstepVolume);
    }

    public void Jump()
    {
        LastJumpRequestTime = Time.time;

        if (IsGrounded || (Time.time - LastGroundedTime < coyoteTime))
        {
            jumpHoldTimer = maxJumpHoldTime;
            isJumping = true;
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
        if (CorpseCounter.Instance == null) Debug.LogError("[Player Controller] ERROR: NO CORPSE COUNTER IN THIS SCENE!");
        else CorpseCounter.Instance.DeleteOldestCorpseIfNeeded();
        StartCoroutine(WaitForDeathAnimation(DeathRespawnDelay));
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
        if (entScript != null)
        {
            if (FacingRight) entScript.FaceRight();
            else entScript.FaceLeft();
        }
        if (CorpseCounter.Instance != null)
        {
            CorpseCounter.Instance.RegisterCorpse(corpseObj);
        }
    }
}