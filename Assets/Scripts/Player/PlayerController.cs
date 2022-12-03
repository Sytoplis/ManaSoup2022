using UnityEngine;

public class PlayerController : MovementController
{
    [Space]
    [Header("Player Specific")]
    [HideInInspector] private Animator animator;
    public event System.Action<PollingStation> onJumpEvent;

    private new void Awake()
    {
        animator = GetComponent<Animator>();
        if (PollingStation.TryGetPollingStation(out station, gameObject))
        {
            station.movementController = this;
        }
        base.Awake();
    }

    internal override float DecideGravity()
    {

        if (velocity.y <= 0)
        {
            return fallGravity;
        }
        else if (velocity.y > 0 && !station.inputManager.GetButton(InputManager.InputPreset.Jump))
        {
            return lowJumpGravity;
        }
        return upGravity;

    }

    internal override void JumpEffects()
    {
        animator.SetTrigger("Jump");
        //This event is used to add effects like audio and particles - Spyro
        if (onJumpEvent != null && onJumpEvent.GetInvocationList().Length > 0)
        {
            onJumpEvent(station);
        }
    }
    internal override float GetHorizontalInput()
    {
        var speed = station.inputManager.GetSingleAxis(InputManager.InputPreset.Movement);
        animator.SetFloat("Speed", speed);
        if (speed < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (speed > 0)
        {
            spriteRenderer.flipX = false;
        }
        return speed;
    }

    internal override bool GetJumpInput()
    {
        return station.inputManager.GetButton(InputManager.InputPreset.Jump);
    }

    internal override bool GetDoubleJumpInput()
    {
        return station.inputManager.GetButtonDown(InputManager.InputPreset.Jump);
    }
}