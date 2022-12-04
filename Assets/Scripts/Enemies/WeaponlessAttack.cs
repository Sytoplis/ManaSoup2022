using System.Collections;
using UnityEngine;

public class WeaponlessAttack : MonoBehaviour, IAttack
{
    [HideInInspector] private PollingStation pollingStation;
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float attackDelayInSeconds;
    [SerializeField] private float cooldownInSeconds;
    [SerializeField] private Damagable objectToAttack;
    [HideInInspector] public Animator animator { get; set; }
    [HideInInspector] private double lastAttackTime;

    private void Start()
    {
        if (PollingStation.TryGetPollingStation(out pollingStation, this.gameObject) && objectToAttack == null)
        {
            objectToAttack = pollingStation.playerController.GetComponent<Damagable>();
        }
        if (objectToAttack == null)
        {
            Debug.LogWarning($"objectToAttack is null nothing to attack found");
        }
    }
    public void Update()
    {
        DoDamage();
    }
    public void DoDamage()
    {
        var distance = objectToAttack.transform.position - transform.position;
        if (Mathf.Abs(distance.magnitude) < range)
        {
            if(Time.timeAsDouble - lastAttackTime < cooldownInSeconds){
                return;
            }
            lastAttackTime = Time.timeAsDouble;
            StartCoroutine(Attack());
            
        }
    }

    private IEnumerator Attack(){
        yield return new WaitForSeconds(attackDelayInSeconds);
        animator.SetTrigger("Attack");
        objectToAttack?.OnHit(this.gameObject, damage);
    }
}