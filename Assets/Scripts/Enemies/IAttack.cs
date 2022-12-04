using UnityEngine;

internal interface IAttack
{
    public Animator animator {get; set;}
    public void DoDamage();
}