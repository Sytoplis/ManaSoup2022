using System;
using UnityEngine;

public delegate void ModifyVelocity(ref Vector2 velocity, MovementController player);

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class MovementController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] public float jumpHeight;
    [SerializeField] public float movementSpeed = 10;
    [SerializeField] public float acceleration = 10, decceleration = 10;
    [SerializeField] public bool enableJump = true;
    [SerializeField] internal Animator animator;

    [Space]
    [Header("Collision Detection")]
    [SerializeField] private float groundCheckOffset;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundCheckMask;

    [Space]
    [Header("Gravity Settings")]
    [SerializeField] public float fallGravity;
    [SerializeField] public float lowJumpGravity;
    [SerializeField] public float upGravity;


    [HideInInspector] public event ModifyVelocity onVelocityModifier;
    [HideInInspector] private Vector3 groundCheckPos => transform.position + transform.up * (groundCheckOffset - 0.5f * groundCheckSize.y);
    [HideInInspector] public Vector2 velocity { get; set; }
    [HideInInspector] internal PollingStation station;
    [HideInInspector] private Rigidbody2D rig;
    [HideInInspector] public float jumpForce => HeightToForce(jumpHeight, upGravity);
    [HideInInspector] public bool doJump { get; internal set; }

    [HideInInspector] public bool grounded { get => GroundCheck(); }
    [HideInInspector] public float gravity { get => DecideGravity(); }

    [HideInInspector] internal float horizontalInput { get; set; }
    [HideInInspector] public float facingDir { get; set; } = 1;
    
    [HideInInspector] public float jumpInput { get; private set; }
    [HideInInspector] public const float jumpBufferStartTime = 0.1f;
    [HideInInspector] public float koyoteTime = 0.3f;
    [HideInInspector] private int airJumpCount = 0;
    [HideInInspector] public float currentKoyoteTime { get; private set; }


    public void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        velocity = Vector2.zero;
    }

    private void Update()
    {
        GetInputs();
    }

    private void FixedUpdate()
    {
        CalculateCoyoteTime();
        ApplyGravity();
        TryJump();
        UpdateVelocity();
    }

    private void TryJump(){
        if (!enableJump)
        {
            return;
        }

        if (currentKoyoteTime > 0)
        {
            //if on ground
            //airJumpCount = 0; //Double Jump
            if (jumpInput > 0)
            {
                DoJump(jumpForce);
            }
        }
        //Double Jump
        /*
        else if (airJumpCount < 1) //not on ground but never jumped in the air
            if (doJump) {//if jump pressed (again)
                velocity = new Vector2(velocity.x, 0);//set velocity.y to 0
                ApplyJump(jumpForce);
                //Debug.Log("Double Jump");
                airJumpCount++;
            }*/

        if (doJump)
        {
            doJump = false;
        }
        jumpInput = 0;
    }

    private void DoJump(float force){
        currentKoyoteTime = 0;
        velocity += Vector2.up * force;
        JumpEffects();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //stop moving up, when you hit a ceiling
        if (velocity.y > 0 && rig.velocity.y < 0.01f)
        { 
            velocity = new Vector2(velocity.x, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = grounded ? Color.green : Color.blue;
        Gizmos.DrawWireCube(groundCheckPos, groundCheckSize);
    }

    private void ApplyGravity()
    {
        var vel = velocity;
        vel.y -= gravity * Time.fixedDeltaTime;
        if (grounded)
        {
            vel.y = 0;
        }
        velocity = vel;
    }

    public void UpdateVelocity()
    {
        Vector2 vel = velocity;
        if (horizontalInput == 0)
        {
            vel.x = Mathf.Lerp(vel.x, 0, decceleration * Time.fixedDeltaTime);
        }
        else
        {
            vel.x = Mathf.Lerp(vel.x, horizontalInput * movementSpeed, acceleration * Time.fixedDeltaTime);
        }
        if (onVelocityModifier != null && onVelocityModifier.GetInvocationList().Length > 0)
        {
            onVelocityModifier(ref vel, this);
        }

        velocity = vel;
        rig.velocity = velocity;
    }

    public static float HeightToForce(float height, float gravity)
    {
        return Mathf.Sqrt(height * 2f * gravity);//returns initial upwards force required to reach given height based on a inputed average gravtity
    }

    private bool GroundCheck()
    {
        return Physics2D.OverlapBox(groundCheckPos, groundCheckSize, transform.eulerAngles.z, groundCheckMask);
    }
    
    private void GetInputs()
    {
        horizontalInput = GetHorizontalInput();
        facingDir = Mathf.Abs(horizontalInput) > 0 ? Mathf.Sign(horizontalInput) : facingDir;
        if (GetJumpInput())
        {
            jumpInput = jumpBufferStartTime;
        }
        else
        {
            jumpInput -= Time.deltaTime;
        }
        doJump = GetDoubleJumpInput();
    }
    internal abstract float DecideGravity();
    internal abstract void JumpEffects();
    internal abstract bool GetJumpInput();
    internal abstract bool GetDoubleJumpInput();
    internal abstract float GetHorizontalInput();
    internal void CalculateCoyoteTime()
    {
        if (grounded)
        {
            currentKoyoteTime = koyoteTime;
        }
        else
        {
            currentKoyoteTime -= Time.fixedDeltaTime;
        }
    }
}

