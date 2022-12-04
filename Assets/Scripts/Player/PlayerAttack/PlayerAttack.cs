using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private Weapon weapon;
    public Transform playerHand;
    private PollingStation station;

    private float currentCooldown = 0;
    private SpriteRenderer weaponRenderer;

    private Animator animator;


    public struct AttackInfo {
        public PlayerAttack attacker;
        public Vector2 playerPos;
        public Vector2 attackPos;
        public float facing;

        public AttackInfo(PollingStation station, PlayerAttack attacker) {
            playerPos = attacker.transform.position;
            Vector2 mousePos = Mouse.current.position.ReadValue();
            attackPos = (Vector2)Camera.main.ScreenToWorldPoint(mousePos) - playerPos;
            this.attacker = attacker;

            if (!station) facing = 1;
            else facing = station.playerController.facingDir;
        }
    }


    private void Awake() {
        animator = GetComponent<Animator>();
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
                animator.SetTrigger("Attack");
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


    private Transform projectileParent;
    public Transform GetProjectileParent() {
        if(projectileParent == null) {
            projectileParent = new GameObject("ProjectileParent").transform;
        }
        return projectileParent;
    }


    private void OnDrawGizmosSelected() {
        if(weapon != null)
            weapon.OnDrawWeaponGizmos(new AttackInfo(station, this));
    }
}
