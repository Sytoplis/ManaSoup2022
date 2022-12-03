using UnityEngine;

public class PlayerController : MovementController
{
    public event System.Action<PollingStation> onJumpEvent;

    private new void Awake()
    {
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
        //This event is used to add effects like audio and particles - Spyro
        if (onJumpEvent != null && onJumpEvent.GetInvocationList().Length > 0)
        {
            onJumpEvent(station);
        }
    }
    internal override float GetHorizontalInput()
    {
        return station.inputManager.GetSingleAxis(InputManager.InputPreset.Movement);
    }

    internal override bool GetJumpInput()
    {
        return station.inputManager.GetButton(InputManager.InputPreset.Jump);
    }
    
    internal override bool GetDoubleJumpInput(){
        return station.inputManager.GetButtonDown(InputManager.InputPreset.Jump);
    }
}