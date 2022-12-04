using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Explosion : MonoBehaviour
{
    public float explosionRadius = 1.0f;
    public float explosionDamage = 1.0f;

    public float destroyTimeSeconds = 1.0f;
    private float initTime;

    private void Awake() {
        initTime = Time.time;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in colliders) {
            if (col.TryGetComponent(out Damagable damagable)) {
                damagable.OnHit(null, explosionDamage);//TODO: maybe add knockback
            }
        }
    }

    private void Update() {
        if (Time.time - initTime >= destroyTimeSeconds)
            Destroy(gameObject);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
