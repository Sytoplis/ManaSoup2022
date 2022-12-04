using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private Weapon weapon;
    public Transform playerHand;
    private PollingStation station;

    private float currentCooldown = 0;
    private SpriteRenderer weaponRenderer;


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

    private void Start() {
        SetWeapon(weapon);
    }

    private void Update() {
        currentCooldown -= Time.deltaTime;

        if (station.inputManager.GetButtonDown(InputManager.InputPreset.Attack))
            if(currentCooldown <= 0) {
                currentCooldown = weapon.cooldownTimeSeconds;
                weapon.OnAttack(new AttackInfo(station, this));
            }
    }


    public void SetWeapon(Weapon weapon) {
        ClearPlayerHand();
        this.weapon = weapon;        
        weaponRenderer = Instantiate(weapon.handPrefab, playerHand).GetComponent<SpriteRenderer>();//Create weapon renderer
    }
    private void ClearPlayerHand() {
        for (int i = playerHand.childCount-1; i >= 0; i--) {
            Destroy(playerHand.GetChild(i).gameObject);
        }
    }


    private void OnDrawGizmosSelected() {
        if(weapon != null)
            weapon.OnDrawWeaponGizmos(new AttackInfo(station, this));
    }
}
