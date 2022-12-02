using UnityEngine;

public delegate void ModifyVelocity(ref Vector2 velocity, MovementController player);

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour {
    [Space]
    [Header("General")]
    public float movementSpeed = 10;
    public float acceleration = 10, decceleration = 10;
    public float jumpHeight;
    public float jumpBufferTime = 0.1f;
    public float koyoteTime = 0.3f;


    [Space]

    public float fallGravity;
    public float lowJumpGravity;
    public float upGravity;


    [Space]
    [Header("Collision Detection")]
    public float groundCheckOffset;
    public Vector2 groundCheckSize;
    public LayerMask groundCheckMask;

    private PollingStation station;
    private Rigidbody2D rig;


    public event System.Action<PollingStation> onJumpEvent;
    public event ModifyVelocity onVelocityModifier;
    public Vector2 velocity { get; set; }
    public float jumpForce { get { return HeightToForce(jumpHeight, upGravity); } }
    private int airJumpCount = 0;

    public bool grounded {
        get {
            return Physics2D.OverlapBox(groundCheckPos, groundCheckSize, transform.eulerAngles.z, groundCheckMask);
        }
    }

    private Vector3 groundCheckPos => transform.position + transform.up * (groundCheckOffset - 0.5f * groundCheckSize.y);

    public float gravity {
        get {
            if (velocity.y <= 0)
                return fallGravity;
            else if (velocity.y > 0 && !station.inputManager.GetButton(InputManager.InputPreset.Jump))
                return lowJumpGravity;
            return upGravity;
        }
    }

    private float horizontalInput { get; set; }
    public float facingDir { get; set; } = 1;
    public float jumpInput { get; private set; }
    public bool jumpPress { get; private set; }
    public float currentKoyoteTime { get; private set; }
    public float maxYVel { get; private set; }

    public bool enableJump { get; set; } = true;


    public static float HeightToForce(float height, float gravity) {
        return Mathf.Sqrt(height * 2f * gravity);//returns initial upwards force required to reach given height based on a inputed average gravtity
    }


    private void Awake() {
        if (!PollingStation.TryGetPollingStation(ref station, gameObject)) {
            return;
        }

        station.movementController = this;
        rig = GetComponent<Rigidbody2D>();
        velocity = Vector2.zero;
        maxYVel = jumpForce;
    }


    private void OnDrawGizmos() {
        Gizmos.color = grounded ? Color.green : Color.blue;
        Gizmos.DrawWireCube(groundCheckPos, groundCheckSize);
    }

    public void ApplyForce(Vector2 force) { velocity += force; }

    private void ApplyGravity() {
        var vel = velocity;
        vel.y -= gravity * Time.fixedDeltaTime;
        if (grounded)
            vel.y = 0;
        velocity = vel;
    }


    private void Update() {
        horizontalInput = station.inputManager.GetSingleAxis(InputManager.InputPreset.Movement);
        facingDir = Mathf.Abs(horizontalInput) > 0 ? Mathf.Sign(horizontalInput) : facingDir;//update facing direction

        
        if (station.inputManager.GetButtonDown(InputManager.InputPreset.Jump))
            jumpPress = true;

        if (station.inputManager.GetButton(InputManager.InputPreset.Jump))
            jumpInput = jumpBufferTime;
        else
            jumpInput -= Time.deltaTime;
    }


    private void FixedUpdate() {
        if (grounded)
            currentKoyoteTime = koyoteTime;
        else
            currentKoyoteTime -= Time.fixedDeltaTime;

        UpdateVelocity();
        rig.velocity = velocity;

        if (jumpPress)
            jumpPress = false;
    }

    private void UpdateVelocity() {
        ApplyGravity();
        TryJump();

        Vector2 vel = velocity;
        if (horizontalInput == 0)
            vel.x = Mathf.Lerp(vel.x, 0, decceleration * Time.fixedDeltaTime);
        else
            vel.x = Mathf.Lerp(vel.x, horizontalInput * movementSpeed, acceleration * Time.fixedDeltaTime);
        if (onVelocityModifier != null && onVelocityModifier.GetInvocationList().Length > 0)
            onVelocityModifier(ref vel, this);
        velocity = vel;
    }

    private void TryJump() {
        if (!enableJump)
            return;

        if (currentKoyoteTime > 0) {//if on ground
            airJumpCount = 0;
            if (jumpInput > 0)
                Jump(jumpForce);
        }
        /*
        else if (airJumpCount < 1) //not on ground but never jumped in the air
            if (jumpPress) {//if jump pressed (again)
                velocity = new Vector2(velocity.x, 0);//set velocity.y to 0
                Jump(jumpForce);
                //Debug.Log("Double Jump");
                airJumpCount++;
            }*/
    }

    public void Jump(float jumpForce) {
        //This event is used to add effects like audio and particles - Spyro
        if (onJumpEvent != null && onJumpEvent.GetInvocationList().Length > 0)
            onJumpEvent(station);

        jumpInput = 0;
        currentKoyoteTime = 0;
        maxYVel = jumpForce;

        ApplyForce(Vector2.up * jumpForce);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (velocity.y > 0 && rig.velocity.y < 0.01f) velocity = new Vector2(velocity.x, 0);//stop moving up, when you hit a ceiling
    }
}

