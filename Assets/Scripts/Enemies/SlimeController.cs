using ManaSoup2022.Assets.Scripts.Systematic;
using UnityEngine;

public class SlimeController : MovementController, IJumpingMovement
{

    [Space]
    [Header("Slime specific")]
    [SerializeField][Range(0, 100)] private float jumpProbability;

    [HideInInspector] private Animator animator;
    [HideInInspector] public float CurrentHorizontalMovement { get; set; }
    [HideInInspector] public bool NeedsJumping { get; set; }
    [SerializeField] private IAttack attack;

    private new void Awake(){
        animator = GetComponent<Animator>();
        attack = GetComponent<IAttack>();
        attack.animator = animator;
        base.Awake();
    }

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
        return CurrentHorizontalMovement;
    }

    internal override bool GetJumpInput()
    {
        var random = Random.Range(0, 100/jumpProbability);
        if(1 >= random){
            return NeedsJumping;
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