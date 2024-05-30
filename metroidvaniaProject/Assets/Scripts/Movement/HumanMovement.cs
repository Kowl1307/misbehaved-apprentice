using UnityEngine;

public class HumanMovement : MonoBehaviour, ITransformation
{
    [SerializeField] private float walkingSpeed = 6;
    [SerializeField] private float sprintSpeed = 10;

    [Header("Walking Settings")] 
    [SerializeField]
    private float acceleration = 13;
    [SerializeField]
    private float deceleration = 9;
    [SerializeField]
    private float friction = 0.2f;
    [SerializeField]
    private float velPower = 1.2f;

    [Header("Jump settings")] 
    [SerializeField] private float jumpForce = 5;

    [SerializeField] [Range(0,1)] private float jumpCutMultiplier = .1f;
    [SerializeField] private float fallGravityMultiplier = 1.1f;
    [SerializeField] private float coyoteTime = .2f;
    private bool isJumping;
    private float gravityScale;
    
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(.3f, .1f);
    private bool isGrounded;
    private float lastGrounded = 0;
    [SerializeField] private LayerMask groundLayer;
    
    Rigidbody2D rb;

    private Animator _animator;
    private bool inIdle = true;
    
    //Inputs in this frame
    private float horizontalInput;
    private bool pressedJump;
    private bool pressedSprint;

    [SerializeField] private bool showLogs = false;
    
    private static readonly int AnimIsJumping = Animator.StringToHash("IsJumping");
    private static readonly int AnimIsWalking = Animator.StringToHash("IsWalking");
    private static readonly int AnimIsIdle = Animator.StringToHash("IsIdle");

    //knock back variables
    public float KBForceHuman = 10;
    public float KBCounterHuman;
    public float KBTotalTimeHuman = 0.3f;
    public bool KnockFromRightHuman;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        gravityScale = rb.gravityScale;
        
    }

    // Update is called once per frame
    void Update()
    {
        //can only move if no knckback exists
        if(KBCounterHuman <= 0)
        {
            //Gets input from "Horizontal axis", between -1 and 1 (left to right)
            horizontalInput = Input.GetAxis("Horizontal");
            pressedJump = Input.GetAxis("Jump") > .1f;
            pressedSprint = Input.GetAxis("Sprint") > .1f;
        }
        //knockback
        else
        {
            if(KnockFromRightHuman == true)
            {
                rb.velocity = new Vector2(-KBForceHuman, KBForceHuman);
            }

            if (KnockFromRightHuman == false)
            {
                rb.velocity = new Vector2(KBForceHuman, KBForceHuman);
            }

            KBCounterHuman -= Time.deltaTime;
        }
        

        if (showLogs)
        {
            //Debug.Log("X input: "+horizontalInput);
            //Debug.Log("Holding Jump: "+pressedJump);
            //Debug.Log("Holding sprint: "+pressedSprint);
        }
       
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleWalking();
        HandleJumping();
        HandleAnimation();
    }

    private void CheckGrounded()
    {
        //This is currently not working correctly, fix it!
        isGrounded = Physics2D.OverlapBox(groundCheckPosition.position, Vector3.one * .3f, 0, groundLayer);
        if (isGrounded)
        {
            isJumping = false;
            lastGrounded = Time.time;
        }
        if(showLogs)
            Debug.Log("Is Grounded:"+isGrounded);
    }

    private void HandleAnimation()
    {
        _animator.SetBool(AnimIsJumping, !isGrounded);
        _animator.SetBool(AnimIsWalking, isGrounded && horizontalInput != 0);
        _animator.SetBool(AnimIsIdle, isGrounded && horizontalInput == 0);
    }
    
    /// <summary>
    /// Calculates and applies the force to move the gameObject.
    /// Force is given by ((targetSpeed - currentSpeed) * accelerationRate)^power * direction
    /// Also applies a friction force that will gradually slow down the gameObject if there is no input
    /// </summary>
    private void HandleWalking()
    {
        var targetSpeed = horizontalInput * (pressedSprint ? sprintSpeed : walkingSpeed);
        var speedDif = targetSpeed - rb.velocity.x;
        var accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        var movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        if (showLogs)
        {
            Debug.Log("Target speed: " + targetSpeed);
            Debug.Log("Speed diff: "+speedDif);
            Debug.Log("Movement: "+movement);
        }
        
        //Friction
        if(isGrounded && Mathf.Abs(horizontalInput) < 0.01f)
        {
            //Either stop movement completely (if velocity is smaller) or apply the friction
            var frictionAmount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(friction));
            frictionAmount *= Mathf.Sign(rb.velocity.x);
            rb.AddForce(Vector2.right * -frictionAmount, ForceMode2D.Impulse);
            if(showLogs)
                Debug.Log("Friction: " + frictionAmount);
        }

        
        if (horizontalInput != 0) transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);

        rb.AddForce(movement * Vector2.right);
    }

    private void HandleJumping()
    {
        if (pressedJump && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
        }
        
        //TODO: Fix it
        /*
        if((pressedJump && !isGrounded && !isJumping && lastGrounded >= Time.time - coyoteTime))
        {
            Debug.Log("Coyote Jump");
            rb.velocity = new(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
        }
        */
        

        //JumpCut (applies force so that movement is cancelled out)
        if (rb.velocity.y > 0 && isJumping && !pressedJump)
        {
            rb.AddForce(Vector2.down * (rb.velocity.y * (1 - jumpCutMultiplier)), ForceMode2D.Impulse);
            
        }

        if(isJumping && !pressedJump && rb.velocity.y < 0)
        {
            //Increased Fall Gravity
            rb.gravityScale = rb.velocity.y < 0 ? gravityScale * fallGravityMultiplier : gravityScale;
        }
        
        if(isGrounded)
        {
            rb.gravityScale = gravityScale;
        }
    }

    public void InitTransformation()
    {
        
    }

    public void OnLeaveTransformation()
    {
        
    }
}