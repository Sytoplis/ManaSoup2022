using UnityEngine;

public class SlimeController : MovementController
{

    [Space]
    [Header("Slime specific")]
    [SerializeField][Range(-1, 1)] private float maxLeftMovement;
    [SerializeField][Range(-1, 1)] private float maxRightMovement;
    [SerializeField][Range(0, 100)] private float jumpProbability;
    

    internal override float DecideGravity()
    {

        if (velocity.y <= 0)
        {
            return fallGravity;
        }
        // else if (velocity.y > 5)
        // {
        //     return lowJumpGravity;
        // }
        return upGravity;

    }

    internal override float GetHorizontalInput()
    {
        return Random.Range(maxLeftMovement, maxRightMovement);
    }

    internal override bool GetJumpInput()
    {
        var random = Random.Range(0, 100/jumpProbability);
        if(1 >= random){
            return true;
        }
        return false;
    }

    internal override bool GetDoubleJumpInput()
    {
        return false;
    }
    internal override void JumpEffects()
    {
        animator.SetTrigger("Jump");
    }
}