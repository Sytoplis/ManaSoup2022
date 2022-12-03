using UnityEngine;

public class Weapon : ScriptableObject
{
    public float cooldownTimeSeconds = 0.4f;

    public virtual void OnAttack() {}
}
