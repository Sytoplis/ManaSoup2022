using UnityEngine;

[CreateAssetMenu(fileName = "new Ranged Weapon", menuName = "Weapons/RangedWeapon")]
public class RangedWeapon : Weapon
{
    public GameObject projectilePrefab;
    public float projectileSpawnDist;
    public Vector2 GetProjectileSpawnPos(PlayerAttack.AttackInfo attackInfo, out Quaternion rot) {
        Vector2 dir = attackInfo.attackPos.normalized;
        Vector3 perpendicular = Vector3.Cross(Vector3.forward, dir);
        rot = Quaternion.LookRotation(Vector3.forward, perpendicular);

        return attackInfo.playerPos + dir * projectileSpawnDist;
    }

    public override void OnAttack(PlayerAttack.AttackInfo attackInfo) {
        base.OnAttack(attackInfo);
        Projectile projectile = Instantiate(projectilePrefab, GetProjectileSpawnPos(attackInfo, out Quaternion rot), rot, attackInfo.attacker.GetProjectileParent())//spawn a projectile
                                .GetComponent<Projectile>();

        projectile.source = attackInfo.attacker.gameObject;
    }

    public override void OnDrawWeaponGizmos(PlayerAttack.AttackInfo attackInfo) {
        base.OnDrawWeaponGizmos(attackInfo);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetProjectileSpawnPos(attackInfo, out Quaternion rot), 0.1f);
        //Gizmos.DrawWireSphere(attackInfo.attackPos + attackInfo.playerPos, 0.1f);
    }
}
