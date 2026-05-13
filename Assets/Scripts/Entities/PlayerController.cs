using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;

public class PlayerController : Entity
{
    //config vars
    [Header("Main Config")]
    [SerializeField] Vector2 spawnPoint;
    [SerializeField, Range(1f, 15f)] float moveSpeed = 7f;
    [SerializeField, Range(1f, 300f)] float jumpForce = 11f * 15f;
    [SerializeField, Range(0.1f, 0.4f)] float maxJumpHoldTime = 0.35f;
    [SerializeField, Range(0.05f, 0.2f)] float coyoteTime = 0.1f;
    [SerializeField, Range(0.1f, 0.5f)] float jumpBuffer = 0.1f;
    [SerializeField, Range(0f, 5f)] float speedMultWhileOnSlope = 1.2f;
    [Header("Grab Mechanic")]
    [SerializeField, Range(0f,15f)] float maxGrabDistanceX = 1.25f; //How far away can the grabber be from the grabbed's center before the tether breaks? A non-positive value means infinite range.
    [SerializeField, Range(0f,15f)] float maxGrabDistanceY = 1.25f;
    [SerializeField] float grabHoldOffset = 1.1f; 
    [SerializeField, Range(0f, 1f)] float speedMultWhilePushingWithoutGrab = 1f;
    [SerializeField, Range(0f, 1f)] float speedMultWhilePulling = 0.5f;
    [SerializeField] float grabDistanceCenter = 0.7f;

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

    [Header("Particles")]
    [SerializeField] ParticleSystem dustCloudEffect;

    //reference vars
    static public PlayerActionControls input;
    private Animator animator;

    //tracking vars
    bool isGrabbingSomething;
    bool isPushingSomething;
    bool isPullingSomething;
    float horizontalMovement;
    float LastJumpRequestTime = Mathf.NegativeInfinity;
    bool isJumping;
    float jumpHoldTimer;

    bool isAlmostGrounded; //used for avoiding fall/jump animations on stairs
    [SerializeField] float isAlmostGroundedDelay = 0.1f;
    float lastGroundedTime;
    [SerializeField] GrabConnector grabConnector;
    [SerializeField, Range(0f,1f)] float GrabCooldown = 0.2f;
    float lastGrabTime;

    //footstep tracking
    private float footstepTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        //start up input handler
        input = new PlayerActionControls();
        input.Player.Enable();

        animator = GetComponent<Animator>();

    }

    void Start()
    {
         //audio source setup
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D sound
            if(AudioManager.Instance != null)  audioSource.volume *= AudioManager.Instance.sfxVolume;
        }
    }

    protected void Update()
    {
        if (IsGrounded) { isAlmostGrounded = true; lastGroundedTime = Time.time; }
        else if (isAlmostGrounded && Time.time - lastGroundedTime > isAlmostGroundedDelay) isAlmostGrounded = false;

        if (!IsGrounded && !isAlmostGrounded && isGrabbingSomething ) Grab(); //immediately drop grabbed object if not on stable ground to prevent flying on them

        horizontalMovement = 0;
        rb.linearVelocityX = 0;
        if (IsIncapacitated) return;

        //Receive inputs
        if (input.Player.Grab.triggered) Grab();

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

        //NOTE: MOVE THIS INTO FIXED UPDATE?
        horizontalMovement = input.Player.Move.ReadValue<Vector2>().x;
        if (!isGrabbingSomething) //only update player orientation visuals while not grabbing things
        {
            if (horizontalMovement > 0) FaceRight();
            else if (horizontalMovement < 0) FaceLeft();
        }

        if (IsTouchingWall == 1 && horizontalMovement > 0) horizontalMovement = 0; //prevent moving into walls (avoids wall cling)
        else if (IsTouchingWall == -1 && horizontalMovement < 0) horizontalMovement = 0;
        
        //determine if touching slope and needs to get a speed boost to make it up
        bool needsSlopeSpeedBoost = (IsTouchingSlope == 1 && horizontalMovement > 0) || (IsTouchingSlope == -1 && horizontalMovement < 0); 

        //if (horizontalMovement != 0) Debug.Log("HorizontalMovement = " + horizontalMovement + ", becoming " + (Vector2.right * horizontalMovement * moveSpeed * Time.deltaTime));
        rb.linearVelocityX = horizontalMovement * moveSpeed * ((isPushingSomething && !isGrabbingSomething)? speedMultWhilePushingWithoutGrab : 1) * (isPullingSomething? speedMultWhilePulling : 1) * (needsSlopeSpeedBoost? speedMultWhileOnSlope : 1) ; //* Time.deltaTime;

        animator.SetBool("IsGrounded", isAlmostGrounded);
        animator.SetFloat("XVel", Mathf.Abs(horizontalMovement));
        animator.SetFloat("YVel", rb.linearVelocityY);

        bool pushableObjectAhead = IsTouchingPushableObject != 0 && (Mathf.Sign(IsTouchingPushableObject) == Mathf.Sign(horizontalMovement));
        bool isPushingGrabbedObject = UpdateGrabbedObject(); //pushing state can ALSO be enabled by this, but cannot be disabled by it.
        isPullingSomething = animator.GetBool("Pulling");
        isPushingSomething = horizontalMovement != 0 && !isPullingSomething && (pushableObjectAhead || isPushingGrabbedObject);
    
        animator.SetBool("Pushing", isPushingSomething); //as long as pulling isn't occurring, pushing can occur due to either of these two states.

        HandleFootsteps();
    }

    private bool UpdateGrabbedObject()
    {
        if (!isGrabbingSomething || grabConnector.GetGrabbedObject() == null )
        {
            animator.SetBool("Pulling", false);
            return false;
        }

        GameObject grabbedObj = grabConnector.GetGrabbedObject();
        //enforce distance limits
        float xDist = Mathf.Abs(transform.position.x - grabbedObj.transform.position.x);
        float yDist = Mathf.Abs(transform.position.y - grabbedObj.transform.position.y);

        if (xDist > maxGrabDistanceX || yDist > maxGrabDistanceY) //immediately release item
        {
            Grab();
            return false;
        }

        //otherwise steer it towards the hold pos
        Vector2 holdTarget = (Vector2)transform.position + Vector2.right * (FacingRight ? 1 : -1) * grabHoldOffset;
        grabConnector.GetGrabbedConnector().SteerTowardPos(holdTarget);

        //handle push/pull animation states
        animator.SetBool("Pulling", Mathf.Sign(horizontalMovement) == Mathf.Sign(transform.position.x - grabbedObj.transform.position.x));
        //these bools mark whether or not the push/pull walk anims should override the basic walk cycle animation
        return true;
    }

    //footstep sound handler
    private void HandleFootsteps()
    {
        bool isMoving = Mathf.Abs(horizontalMovement) > 0.1f;

        if (IsGrounded && isMoving && footstepSounds != null && footstepSounds.Length > 0)
        {
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= footstepInterval * (isPullingSomething  ? (1f/speedMultWhilePulling) : 1f) * (isPushingSomething  ? (1f/speedMultWhilePushingWithoutGrab) : 1f))
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = footstepInterval + 1f; //always start the first cycle with an instant footstep
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

    public void Grab()
    {
        if (isGrabbingSomething)
        {
            GameObject GrabbedObject = grabConnector.GetGrabbedObject();
            if (GrabbedObject != null && Time.time - lastGrabTime > GrabCooldown)
            {
                grabConnector.SetGrabbedObject(null);
                isGrabbingSomething = false;
            }
        }
        else
        {
            //Check for grabbable objects in front of the player
            //Find the closest one
            //Parent it to the GrabConnector
            //Link the two grabConnectors
            GameObject nearestGrabbableObject = FindNearestGrabbableObjectInBox();
            grabConnector.SetGrabbedObject(nearestGrabbableObject);
            if (grabConnector.GetGrabbedObject()!=null) isGrabbingSomething = true;
        }

        lastGrabTime = Time.time;
    }

    private GameObject FindNearestGrabbableObjectInBox()
    {
        Vector2 center = (Vector2)transform.position
            + Vector2.right * (FacingRight? 1 : -1) * grabDistanceCenter
            + Vector2.up * (spr.bounds.extents.y - 0.2f);
        Vector2 size = new Vector2(1.1f, 1f);

        DebugUtils.DebugDrawBox2D(center, size, Color.red, 0.2f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);

        float closestDistSqr = Mathf.Infinity;
        GameObject closestObject = null;

        foreach (Collider2D c in hits)
        {
            if (c.CompareTag("Grabbable"))
            {
                Debug.Log(">>> Found grabbable " + c.name);
                float distSqr = Vector2.SqrMagnitude((Vector2)c.transform.position - center);
                if (distSqr < closestDistSqr)
                {
                    closestDistSqr = distSqr;
                    closestObject = c.gameObject;
                }
            }
        }
        return closestObject;
    }

    public void Jump()
    {
        LastJumpRequestTime = Time.time;

        if (isGrabbingSomething) Grab(); //Immediately drop grabbed object if you jump

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
        //spawn dust cloud effect when landing
        if (dustCloudEffect != null)
        {
            Instantiate(dustCloudEffect, transform.position + Vector3.down * 0.5f, Quaternion.identity);
        }

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

    public void Incapacitate() { IsIncapacitated = true; }
    public void Deincapacitate() { IsIncapacitated = false; }
}