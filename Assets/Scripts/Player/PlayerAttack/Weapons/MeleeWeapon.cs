using UnityEngine;

[CreateAssetMenu(fileName = "new Melee Weapon",menuName = "Weapons/MeleeWeapon")]
public class MeleeWeapon : Weapon
{
    public float damage = 1.0f;
    public Bounds hitBounds;

    private Bounds GetHitBounds(PlayerAttack.AttackInfo attackInfo) {
        Vector2 center = hitBounds.center;
        center.x *= attackInfo.facing;
        return new Bounds(attackInfo.playerPos + center, hitBounds.size);
    }

    public override void OnAttack(PlayerAttack.AttackInfo attackInfo) {
        base.OnAttack(attackInfo);

        Bounds currentHitBounds = GetHitBounds(attackInfo);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(currentHitBounds.center, currentHitBounds.size, 0.0f);
        foreach(Collider2D col in colliders) {
            if(col.TryGetComponent(out Damagable damagable)) {
                damagable.OnHit(attackInfo.attacker, damage);
            }
        }
    }

    public override void OnDrawWeaponGizmos(PlayerAttack.AttackInfo attackInfo) {
        base.OnDrawWeaponGizmos(attackInfo);

        Gizmos.color = Color.red;
        Bounds currentHitBounds = GetHitBounds(attackInfo);
        Gizmos.DrawWireCube(currentHitBounds.center, currentHitBounds.size);
    }
}
