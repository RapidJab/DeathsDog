using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixeye.Unity;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Components  
    [Foldout("Components", true)]
    private Dialogue dialogue; 
    [SerializeField] private PlayerInput input;
    private Collider2D[] playerColliders;
    [SerializeField] private CapsuleCollider2D groundCheck; //Transform that ground checks originate from 
    [SerializeField] private Collider2D dashLedgeCheck;
    [SerializeField] private CircleCollider2D barkCheck;
    [SerializeField] private SpriteRenderer bubble;
    [SerializeField] private GameObject djObject; 
    [SerializeField] private GameObject grabCollectableObject;
    [SerializeField] private GameObject dashVFX;
    private Transform tran;
    private Animator anim;
    private Rigidbody2D rb2d; 
    #endregion 

    #region Movement Variables
    [Foldout("Movement", true)] 
    [SerializeField] private float groundSlowSpeed; //Horizontal movement on the ground
    [SerializeField] private float groundFastSpeed; //Horizontal movement on the ground
    [SerializeField][ShowOnly] private float groundSpeed; //Horizontal movement on the ground
    [SerializeField] private float airSlowSpeed; //Horizontal movement in the air
    [SerializeField] private float airFastSpeed; //Horizontal movement in the air
    [SerializeField] private float airMinSpeed; //Horizontal movement in the air
    private float tempAirSpeed; //Horizontal movement in the air, unmodified by speed alterations
    [SerializeField] [ShowOnly] private float airSpeed; //Horizontal movement in the air 
    [ShowOnly] [SerializeField] private bool facingRight = true;  //Determines which way player should face
    private float xMove; //Current movement input
    private float yMove; //Current y movement input
    private int normalLayer; //Normal player layer
    private int fallingLayer; //Falling player layer, goes through platforms
    [SerializeField] private float xLimit; //Fastest player can be on x axis
    [SerializeField] private float yPosLimit; //Fastest player can be from a jump 
    [SerializeField] private float yNegLimit; //Fastest player can be from a jump 
    public Vector2 moveModifier { get; set; } //Modify the x movement of the player  
    #endregion

    public bool outsideForce;

    #region Sprint Variables
    [Foldout("Sprint", true)]
    [SerializeField] private float beginSprintTime; //How long the player has to move before they can dash
    private float beginSprintTimer;
    [SerializeField] private float buildSprintTime; //How long the player has to move before they can dash
    private float buildSprintTimer;
    [SerializeField] [ShowOnly] private bool sprinting;
    #endregion

    #region Sit Variables
    [Foldout("Sit", true)]
    [SerializeField] private float sitTime; //How long the player has to move before they can dash 
    [SerializeField] [ShowOnly] public bool sitting;
    #endregion 

    #region Jump Variables
    [Foldout("Jump", true)]
    [SerializeField] private float jumpBufferTime; //Double jump speed
    [SerializeField] private float groundJumpForce; //Jump speed
    [SerializeField] private float airJumpForce; //Double jump speed 
    [SerializeField] private float fallMultiplier = 2.5f; //Force multiplier while falling
    [SerializeField] private float lowJumpMultiplier = 2f; //Velocity multiplier while beginning jump
    [SerializeField] private int maxAirJumps; //Max air jumps 
    [ShowOnly] [SerializeField] private int curAirJumps; //Current air jumps remaining
    [ShowOnly] [SerializeField] public bool groundJump; //Is the ground jump still remaining? 
    [SerializeField] private float groundJumpMoveModifier; //Horizontal movement on the ground 
    [SerializeField] private float airJumpMoveModifier; //Horizontal movement on the ground 
    [SerializeField][ShowOnly] private float jumpMoveModifier; //Horizontal movement on the ground
    private bool curJumping; //Are you currently in the upward arc of a jump?
    private bool bufferingJump;  
    #endregion 

    #region Ground Variables 
    [Foldout("Ground", true)]
    [ShowOnly] [SerializeField] private GameObject _grounded; //Is the player touching ground?  
    public GameObject grounded { get; private set; } 
    [SerializeField] private LayerMask groundLayer; //Layer of ground
    [SerializeField] private float rememberGroundedFor; //Allows player to stay grounded even when technically in the air
                                                        //Useful for jumps near edges
    private float lastTimeGrounded; //The time when the player was last grounded, compared against rememberGroundedFor
    #endregion

    #region Dodge Variables  
    [Foldout("Dodge", true)]
    [SerializeField] private float dashSpeed; //Horizontal movement 
    [SerializeField][Range(0, 1)] private float dashLedgeStopPercent; //What percent of the dash needs to be done before cutting off at the end of a platform?
    [SerializeField] private float dashStartTime; //Time frozen in air before dash
    [SerializeField] private float dashMoveTime; //Time the dash movement lasts
    [SerializeField] private float dashCooldownTime; //Time before another dash can be used
    [ShowOnly] [SerializeField] private bool _dashCooldownReady; //Dash is cooled down
    private bool dashCooldownReady
    {
        get => _dashCooldownReady;
        set
        {
            _dashCooldownReady = value;
            canDash = (dashCooldownReady && dashValid);
        }
    }  
    [ShowOnly] [SerializeField] private bool _dashValid; //Player has become grounded or has jumped since last dash
    private bool dashValid
    {
        get => _dashValid;
        set
        {
            _dashValid = value;
            canDash = (dashValid && dashCooldownReady); 
        }
    } 
     //Variable for whether dash is in cooldown, and whether a jump/grounded has happened beween dashes, and if both then canDash 
    [SerializeField] private bool canDash = true; //Can dash be inputted?  
    #endregion

    #region Fall Variables
    const float fallLockedTime = .3f; //Time before you can stop falling after inputting fall
    private bool canChangeFall = true; //Can you stop falling?
    #endregion

    #region Bark Variables
    [Foldout("Bark", true)]
    [SerializeField] float barkAnimationTime;
    [SerializeField] float barkCoolDownTime;
    [SerializeField] float barkWaitTime = 1.5f;
    [SerializeField] [ShowOnly] private bool canBark = true;
    #endregion

    #region Input Variables 
    [Foldout("Input", true)]
    private bool jump; //Was jump inputted  
    [SerializeField] private bool _canInput; //Can the player perform any actions? Shouldn't be directly affected outside of dashValid/dashCooldownReady
    public bool canInput { get => _canInput; set => _canInput = value; }
    #endregion

    #region Audio Variables 
    [Foldout("Audio", true)]
    [SerializeField] private AudioClip barkAudio; 
    [SerializeField] private AudioClip barkMagicAudio;
    [SerializeField] private AudioClip jumpAudio; 
    [SerializeField] private AudioClip doubleJumpAudio;
    [SerializeField] private AudioClip landAudio;
    [SerializeField] private AudioClip dashAudio; 
    [SerializeField] private AudioClip walkAudio; 
    [SerializeField] private AudioClip sprintAudio;
    [SerializeField] private AudioClip bubbleAudio; 
    [SerializeField] private AudioClip fallAudio;  
    [SerializeField] private AudioSource fallSource; 
    #endregion

    #region Death
    [Foldout("Death", true)]
    [ShowOnly] public bool dead;
    [SerializeField] private float bubblePause;
    [SerializeField] private float moveCurve;
    [SerializeField] private float deathMoveTime;
    [SerializeField] private float deathYOffset;
    #endregion  

    #region Awake
    void Awake()
    {
        playerColliders = GetComponents<Collider2D>();
        facingRight = true;
        normalLayer = LayerMask.NameToLayer("Player"); 
        fallingLayer = LayerMask.NameToLayer("PlayerDown");
        tran = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        curAirJumps = maxAirJumps;
        //canInput = true;
        dashCooldownReady = true;
        dashValid = true; 
        dialogue = Camera.main.GetComponentInChildren<Dialogue>();
        bubble.enabled = false;
        djObject.SetActive(false);
        dashVFX.SetActive(false);
    }
    #endregion

    #region Death
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Death"))
        {
            Dead();
        }
    }
    private void Dead()
    { 
        StopAllCoroutines(); 
        MusicManager.Instance.PlaySound(bubbleAudio, .3f);
        if (VariableManager.Instance.isBarked)
        {
            MusicManager.Instance.PlaySound(barkMagicAudio, .2f);
        }
        VariableManager.Instance.isBarked = false;
        canBark = true;
        canDash = true;
        jump = false;
        curAirJumps = 0;
        groundJump = false; 
        canDash = true;
        dashCooldownReady = true;
        dashValid = true;
        canChangeFall = true;
        gameObject.layer = normalLayer;
        grabCollectableObject.SetActive(false); //Disable grabbing collectables
        foreach (Collider2D col in playerColliders)
        {
            col.enabled = false; //Disable all colliders
        }
        bufferingJump = true;
        grounded = null;
        canInput = false;
        rb2d.velocity = Vector2.zero;
        rb2d.gravityScale = 0;
        dead = true;
        djObject.SetActive(false);
        dashVFX.SetActive(false);
        StartCoroutine(DeathMove());
    }
    private IEnumerator DeathMove()
    {  
        yield return new WaitForSeconds(bubblePause);
        Vector3 endPos = new Vector2(VariableManager.Instance.curCheckpoint.lastPos.x, VariableManager.Instance.curCheckpoint.lastPos.y + deathYOffset);
        Vector3 currentPos = transform.position;
        float bubbleSpeed =  Vector2.Distance(currentPos, endPos) / deathMoveTime; 
        rb2d.velocity = (endPos - currentPos).normalized * bubbleSpeed;
        anim.SetBool("IsBubbled", true);
        bubble.enabled = true;
        yield return new WaitForSeconds(deathMoveTime);
        #region On Pop Variables
        MusicManager.Instance.PlaySound(bubbleAudio, .3f, 3);
        canInput = true;
        bufferingJump = false;
        rb2d.gravityScale = 1;
        grabCollectableObject.SetActive(true);
        foreach (Collider2D col in playerColliders)
        {
            col.enabled = true;
        }
        bubble.enabled = false;
        dead = false;
        anim.SetBool("IsBubbled", false);
        #endregion 
    }
    #endregion 

    void Update()
    {   
        TuneSpeed();
        if (canInput)
        {
            MoveSetup();
            JumpSetup();
            DashSetup();
            BarkSetUp();
        }
        SetSit();

        if (!bufferingJump)
        {
            Fall();
            CheckIfGrounded();
            if (canInput)
            {
                AnimSet();
            }
        }
        FallSound();
    } 

    private void FixedUpdate()
    { 
        if (canInput)
        {
            Move();
            JumpStart();
            TuneJump();
        } 
        ClampSpeed();
    }

    #region Sit
    private void SetSit()
    { 
        if (!sitting)
        {
            if (VariableManager.Instance.inCutscene)
            {
                StartCoroutine(StartSit());
            }
            if(VariableManager.Instance.inDialogue)
            {
                DialogueSit();
            }    
        }
        else if (!VariableManager.Instance.inDialogue && !VariableManager.Instance.inCutscene)
        {
            EndSit();
        }
    }
    public IEnumerator StartSit()
    {
        rb2d.velocity = Vector2.zero;
        sitting = true;
        canInput = false; //Stop input from working
        anim.SetTrigger("IsStartSit");
        anim.SetBool("IsSitting", sitting); 
        yield return new WaitForSeconds(sitTime);
        dialogue.StartDialogue();
    }
    private void DialogueSit()
    { 
        rb2d.velocity = Vector2.zero;
        sitting = true;
        canInput = false; //Stop input from working
        anim.SetTrigger("IsStartSit");
        anim.SetBool("IsSitting", sitting); 
        dialogue.StartDialogue();
    }
    public void EndSit()
    {
        sitting = false;
        anim.SetBool("IsSitting",sitting);
        canInput = true;
    }
    #endregion 

    #region TuneSpeed
    private void TuneSpeed()
    {
        if (xMove > 0 && !facingRight || xMove < 0 && facingRight || xMove == 0) //Movement is not inputted or is in the wrong direction
        {
            ResetSpeed();
        }
        SetSprintBeginning(); 
        SetSprintBuildup();
    }

    private void ResetSpeed()
    { 
        sprinting = false;
        groundSpeed = groundSlowSpeed;
        tempAirSpeed = airSlowSpeed;
        beginSprintTimer = beginSprintTime;
        buildSprintTimer = buildSprintTime;
    }
    private void SetSprintBeginning()
    {
        if ((bufferingJump || grounded || groundJump) && xMove != 0) //Moving on ground
        {
            if (beginSprintTimer < 0) //Player has walked long enough to sprint
            {
                sprinting = input.currentActionMap.FindAction("Sprint").GetButton();
                groundSpeed = sprinting ? groundFastSpeed : groundSlowSpeed;
            }
            else //Player is walking, so decrease time until able to sprint
            {
                beginSprintTimer -= Time.deltaTime;
            }
        }
    }
    private void SetSprintBuildup()
    { 
        if (sprinting) //Sprinting
        { 
            if (grounded && !groundJump) //In the air
            {
                if (buildSprintTimer < 0) //Sprinted for long enough to have max air movement
                {
                    Debug.Log("Sprinting2");
                    airSpeed = airMinSpeed + airFastSpeed;
                }
                else //Weren't at max air movement when jump began
                {
                    Debug.Log("Sprinting3");
                    airSpeed = airMinSpeed + (airFastSpeed * (1 - (buildSprintTimer / buildSprintTime)));
                }
            }
            else //grounded and sprinting, so decrease timer to max air movement
            {
                buildSprintTimer -= Time.deltaTime;
            }
        }
        else //Not sprinting, reset build timer and air movement
        {
            Debug.Log("Sprinting5");
            airSpeed = airMinSpeed;
            buildSprintTimer = buildSprintTime;
        }
        if (rb2d.velocity.x < .001f && rb2d.velocity.x > -.001f && !grounded && !groundJump && !bufferingJump && airSpeed > airMinSpeed)
        {
            Debug.Log("Sprinting4");
            airSpeed = airMinSpeed;
            buildSprintTimer = buildSprintTime;
        }
    }
    #endregion 

    #region Moving 
    public void OnMove(InputAction.CallbackContext context)
    {
        xMove = context.ReadValue<Vector2>().x; //Make movement relative to camera  
        yMove = context.ReadValue<Vector2>().y; 
    }
    void MoveSetup()
    {  
        if (xMove > 0 && !facingRight || xMove < 0 && facingRight) //movement is not in the direction the sprite is facing
        {
            Flip();
        }
    }
    void Move()
    {
        Debug.Log(rb2d.velocity.y + ", " + moveModifier.y);
        rb2d.velocity = new Vector2(((xMove * (grounded ? groundSpeed : airSpeed) * jumpMoveModifier)) + moveModifier.x, rb2d.velocity.y + moveModifier.y);
        moveModifier = Vector2.zero; 
    }
    #endregion 

    #region Jumping
    void JumpSetup()
    {
        if(!input.currentActionMap.FindAction("Jump").GetButtonDown())
            return; 
        jump = true; //Jump inputted
    }
    void JumpStart() 
    { 
        if (jump && (curAirJumps > 0 || groundJump)) //Jump inputted and either have an air or ground jump to use
        {
            StartCoroutine(Jump());
        }
        jump = false; //Remove jump input
    }
    private IEnumerator Jump()
    {
        float actualJumpForce;
        anim.SetTrigger("IsJump"); //Start jump animation
        curJumping = true; //Currently jumping
        if (!groundJump) //Performing an air jump
        {
            //ResetSpeed();
            airSpeed = Mathf.Max(airMinSpeed, airSpeed - 1);
            jumpMoveModifier = airJumpMoveModifier;
            actualJumpForce = airJumpForce; 
            MusicManager.Instance.PlaySound(doubleJumpAudio, .5f);
            anim.SetTrigger("IsJumpSecond"); //Air jump animation
            curAirJumps--; //Decrease air jumps
            djObject.SetActive(true);
        }
        else
        {
            jumpMoveModifier = groundJumpMoveModifier;
            actualJumpForce = groundJumpForce;
            MusicManager.Instance.PlaySound(jumpAudio, .5f);
            anim.SetTrigger("IsJumpFirst"); //Ground jump animation
            groundJump = false; //Can no longer do ground jump 
        }
        dashValid = true; //You've jumped, so dash can be used again 
        canInput = false;
        bufferingJump = true;
        //rb2d.velocity = Vector2.zero;
        rb2d.gravityScale = 0;
        yield return new WaitForSeconds(jumpBufferTime);
        bufferingJump = false;
        canInput = true; 
        rb2d.gravityScale = 1;
        rb2d.velocity = new Vector2(rb2d.velocity.x, actualJumpForce); //Ground jump movement 
    }

    void TuneJump() 
    {
        if (rb2d.velocity.y < 0) //Falling
        {
            rb2d.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.fixedDeltaTime; //Add fallMultiplier to velocity so player falls faster
        } 
        else if (rb2d.velocity.y > 0 && curJumping && !input.currentActionMap.FindAction("Jump").GetButton()) //Going up, jumping, and not pressing jump 
        {
            rb2d.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.fixedDeltaTime; //Stunt the jump's movement
        }   
    }
    #endregion

    #region Dashing
    private void DashSetup()
    {
        if (!input.currentActionMap.FindAction("Dash").GetButtonDown() || !canDash) //Not inputted or unable to dash
            return;
        StartCoroutine(Dash());
    }
    private IEnumerator Dash()
    {
        dashCooldownReady = false;
        dashValid = false;
        canInput = false; //Cannot perform other actions
        rb2d.velocity = Vector2.zero; //Stop all movement
        rb2d.gravityScale = 0; //Stop gravity
        anim.SetTrigger("IsDashStart"); //Trigger dash animation
        dashVFX.SetActive(true);
        yield return new WaitForSeconds(dashStartTime); //Wait for the dash start time to finish  
        MusicManager.Instance.PlaySound(dashAudio, .25f);
        anim.SetTrigger("IsDash"); //Trigger dash animation
        rb2d.velocity = new Vector2(dashSpeed * (facingRight ? 1 : -1) + xMove, 0); //Change velocity  
        float remainingMoveTime = dashMoveTime;
        while (remainingMoveTime > 0)
        {
            if (grounded && dashLedgeStopPercent >= remainingMoveTime / dashMoveTime)
            {
                Collider2D colliders = Physics2D.OverlapCircle(dashLedgeCheck.bounds.center, dashLedgeCheck.bounds.extents.x, groundLayer);
                if (colliders == null) //Player is on a platform, so they are grounded
                {
                    rb2d.velocity = Vector2.zero;
                }
            }
            remainingMoveTime -= Time.deltaTime;
            yield return null;
        }
        rb2d.velocity = Vector2.zero; //Stop all movement
        yield return new WaitForSeconds(.1f);
        rb2d.gravityScale = 1f; //Restart gravity
        canInput = true; //Allow other actions to be performed
        if (dashValid)
            dashCooldownReady = true;
        else
        {
            yield return new WaitForSeconds(dashCooldownTime); //Wait for the dash cooldown time to finish
            dashCooldownReady = true; //Cooldown complete
        }
        dashVFX.SetActive(false);
    }
    #endregion

    #region Falling
    private void Fall()
    {
        if (canChangeFall) //Able to stop/start falling
        {
            if (yMove < 0 && rb2d.velocity.y <= 0) //Inputting down and falling or stationary
            {
                gameObject.layer = fallingLayer; //Fall through platforms
                groundLayer = LayerMask.GetMask("Floor"); //Don't consider platforms ground
                StartCoroutine(FallTimer());
            }
            else if (yMove >= 0) //Inputting up
            {
                gameObject.layer = normalLayer; //Don't fall through platforms
                groundLayer = LayerMask.GetMask("Floor", "Platform"); //Consider platforms ground
            }
        }
    }

    private IEnumerator FallTimer() //Prevent the player from tapping fall and not falling at all
    {
        canChangeFall = false;
        yield return new WaitForSeconds(fallLockedTime); //Player cannot stop falling for .3 seconds
        canChangeFall = true;
    }
    #endregion

    #region CheckIfGrounded  
    private void CheckIfGrounded()
    {
        if (rb2d.velocity.y <= 0) //Falling or stationary
        {   
            Collider2D[] colliders = Physics2D.OverlapCapsuleAll(groundCheck.bounds.center, groundCheck.bounds.size, CapsuleDirection2D.Horizontal, 0, groundLayer);
            Collider2D collider = null;
            foreach(Collider2D col in colliders)
            {
                if(!col.isTrigger)
                {
                    collider = col;
                    break;
                }
            }
            if (collider != null) //Player is on a platform, so they are grounded
            {
                if (!groundJump)
                { 
                    MusicManager.Instance.PlaySound(landAudio, .2f);
                }
                jumpMoveModifier = 1;
                curJumping = false; //No longer jumping
                dashValid = true; //You're on ground, so dash is valid to use again
                grounded = collider.gameObject;
                groundJump = true; //Give back ground jump
                curAirJumps = maxAirJumps; //Reset jumps back to default 
                return;
            } 
        }
        //No longer grounded
        if (grounded)
        {
            lastTimeGrounded = Time.time; //Just left ground, set lastTimeGrounded
        } 
        grounded = null;
        if (rb2d.velocity.y <= 0) //Falling or stationary
        {
            curJumping = false; //No longer in jump
        }
        if (!grounded && Time.time - lastTimeGrounded > rememberGroundedFor) //Falling for too long, lose ground jump
        {
            groundJump = false; //Can no longer ground jump
        } 
    }
    #endregion   

    #region Flip
    private void Flip() //Flip player sprite via changing local scale
    {
        anim.SetTrigger("IsRotate");
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    #endregion

    #region ClampSpeed
    private void ClampSpeed()
    {
        float yCur = rb2d.velocity.y;
        float xCur = rb2d.velocity.x;
        yCur = Mathf.Clamp(yCur, -yNegLimit, yPosLimit);
        xCur = Mathf.Clamp(xCur, -xLimit, xLimit);
        rb2d.velocity = new Vector2(xCur, yCur); 
        moveModifier = Vector2.zero;
    }
    #endregion 

    #region AnimSet
    public void AnimSet()
    {
        anim.SetBool("IsGround", groundJump);
        float verticalVelocity = rb2d.velocity.y;
        anim.SetBool("IsDown", (!groundJump && !curJumping));
        if (groundJump && verticalVelocity == 0)
        {
            anim.SetBool("IsJump", false);
            anim.ResetTrigger("IsJumpFirst");
            anim.ResetTrigger("IsJumpSecond");
            anim.SetBool("IsDown", false);
        }
        if (xMove == 0)
        {
            anim.SetBool("IsRun", false);
            anim.SetBool("IsWalk", false);
            anim.ResetTrigger("IsRotate");
            anim.SetTrigger("stopTrigger");
        }
        else
        { 
            anim.SetBool((sprinting ? "IsRun" : "IsWalk"), true);
            anim.SetBool((sprinting ? "IsWalk" : "IsRun"), false);
        }
        // stop
        if (xMove == 0)
        {
            anim.SetTrigger("stopTrigger");
            anim.ResetTrigger("IsRotate");
            anim.SetBool("IsRun", false);
            anim.SetBool("IsWalk", false);
        }
        else
        {
            anim.ResetTrigger("stopTrigger");
        }
    }
    #endregion

    #region Footsteps
    public void WalkStep()
    { 
        MusicManager.Instance.PlaySound(walkAudio, .5f);
    }
    public void RunStep()
    { 
        MusicManager.Instance.PlaySound(sprintAudio, .5f);
    }
    #endregion

    #region Fall Sound
    private void FallSound()
    {
        if(rb2d.velocity.y < -5)
        {
            float fallVolume = Mathf.Min(.7f, 2 * (1 - (rb2d.velocity.y - (-30)) / (-5 - (-30))));
            fallSource.volume = fallVolume; 
        } else
        {
            fallSource.volume = 0;
        }
    }
    #endregion  

    #region Bark
    void BarkSetUp()
    {
        //Have the Character bark and set isBarked
        if (input.currentActionMap.FindAction("Bark").GetButtonDown() && !VariableManager.Instance.isBarked && canBark)
        { 
            StartCoroutine(Bark());
        }
    } 

    private IEnumerator Bark()
    {
        canBark = false;
        canInput = false;
        anim.SetTrigger("IsBark");
        rb2d.velocity = Vector2.zero;
        rb2d.gravityScale = 0;
        float pitch = Random.Range(.8f, 1.2f);
        MusicManager.Instance.PlaySound(barkAudio, .25f, pitch); 
        yield return new WaitForSeconds(barkAnimationTime); 
        rb2d.gravityScale = 1;
        VariableManager.Instance.isBarked = true;
        canInput = true;
        bool anyBarked = BarkCollider();
        if(anyBarked)
        {
            MusicManager.Instance.PlaySound(barkMagicAudio, .2f);
        }
        yield return new WaitForSeconds(barkCoolDownTime);
        if (anyBarked)
        {
            MusicManager.Instance.PlaySound(barkMagicAudio, .2f);
        }
        VariableManager.Instance.isBarked = false;
        yield return new WaitForSeconds(barkWaitTime);
        canBark = true;
    }
    private bool BarkCollider()
    {
        bool anyBarked = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(barkCheck.bounds.center, barkCheck.radius);
        foreach (Collider2D col in colliders)
        {
            if (col.GetComponent<barkObject>())
            {
                anyBarked = true;
                col.GetComponent<barkObject>().inBarkRange = true;
            }
        }
        return anyBarked;
    }
    #endregion

    public void ReplenishJumps()
    { 
        groundJump = false; //Bounced on a bouncing platform, because you're immediately in air, groundJump is false
        curAirJumps = maxAirJumps; //Air jumps are repleneshed because you did touch a platform
    }
}
