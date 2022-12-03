using UnityEngine;

public class Damagable : MonoBehaviour
{
    public float health = 10.0f;

    public virtual void OnHit(MonoBehaviour attacker, float damage) {
        if (attacker.gameObject == gameObject) return;//can not hurt self

        health = health - damage;
        if(health <= 0.0f) {
            health = 0.0f;
            OnDeath();
        }
    }

    public virtual void OnDeath() {

    }
}
