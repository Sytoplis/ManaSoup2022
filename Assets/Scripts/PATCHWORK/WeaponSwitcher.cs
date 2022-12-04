using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAttack))]
public class WeaponSwitcher : MonoBehaviour
{
    public Weapon[] weapons;
    public float switchCooldown = 0.1f;
    public int current = 0;
    private float lastSwitch = 0.0f;

    private PlayerAttack attacker;

    private void Awake() {
        attacker = GetComponent<PlayerAttack>();
    }

    private void Update() {
        if (Mouse.current.rightButton.isPressed) {
            if(lastSwitch <= 0.0f) {
                lastSwitch = switchCooldown;

                SwitchWeapon();
            }
        }

        lastSwitch -= Time.deltaTime;
    }

    private void SwitchWeapon() {
        attacker.SetWeapon(weapons[current]);
        current++;
        current %= weapons.Length;
    }
}
