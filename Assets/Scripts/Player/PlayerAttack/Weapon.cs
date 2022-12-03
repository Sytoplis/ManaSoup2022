using UnityEngine;

public class Weapon : ScriptableObject
{
    public float cooldownTimeSeconds = 0.4f;

    //NOTE: position is in local space and will give, when normalized, the direction of attack
    public virtual void OnAttack(PlayerAttack.AttackInfo attackInfo) {}

    public virtual void OnDrawWeaponGizmos(PlayerAttack.AttackInfo attackInfo) { }
}
