using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Weapon weapon;
    private PollingStation station;

    private float currentCooldown = 0;

    private void Awake() {
        if(PollingStation.TryGetPollingStation(out station, gameObject))
            return;
    }

    private void Update() {
        currentCooldown -= Time.deltaTime;

        if (station.inputManager.GetButtonDown(InputManager.InputPreset.Attack))
            if(currentCooldown <= 0) {
                currentCooldown = weapon.cooldownTimeSeconds;
                weapon.OnAttack();
            }
    }
}
