using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Weapon weapon;
    private PollingStation station;

    private float currentCooldown = 0;


    public struct AttackInfo {
        public PlayerAttack attacker;
        public Vector2 playerPos;
        public Vector2 attackPos;
        public float facing;

        public AttackInfo(PollingStation station, PlayerAttack attacker) {
            playerPos = attacker.transform.position;
            attackPos = (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - playerPos;
            this.attacker = attacker;

            if (!station) facing = 1;
            else facing = station.movementController.facingDir;
        }
    }


    private void Awake() {
        if(PollingStation.TryGetPollingStation(out station, gameObject))
            return;
    }

    private void Update() {
        currentCooldown -= Time.deltaTime;

        if (station.inputManager.GetButtonDown(InputManager.InputPreset.Attack))
            if(currentCooldown <= 0) {
                currentCooldown = weapon.cooldownTimeSeconds;
                weapon.OnAttack(new AttackInfo(station, this));
            }
    }

    private void OnDrawGizmosSelected() {
        if(weapon != null)
            weapon.OnDrawWeaponGizmos(new AttackInfo(station, this));
    }
}
