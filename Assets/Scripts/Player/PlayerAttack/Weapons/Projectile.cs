using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public MonoBehaviour source;
    public float initVel = 1.0f;
    public float acceleration = 0.0f;

    [Space]

    public float impactDamage = 0.0f;
    public bool destroyOnImpact = true;
    public GameObject spawnOnImpact;//mainly explosions

    private Rigidbody2D rig;

    private void Awake() {
        rig = GetComponent<Rigidbody2D>();
        rig.velocity = transform.right * initVel;
    }

    private void FixedUpdate() {
        rig.AddForce(transform.right * acceleration, ForceMode2D.Force);
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.TryGetComponent(out Damagable damagable)) {
            damagable.OnHit(source, impactDamage);
        }

        if (spawnOnImpact)
            Instantiate(spawnOnImpact, transform.position, Quaternion.identity, transform.parent);

        if(destroyOnImpact)
            Destroy(gameObject);
    }
}
