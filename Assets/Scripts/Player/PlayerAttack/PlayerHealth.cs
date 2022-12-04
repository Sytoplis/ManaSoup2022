using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : Damagable
{
    public GameObject healthPrefab;
    public Transform healthParent;

    private Animator[] hearts;

    private float maxHealth;

    public int GameOverScene;

    public void Start() {
        maxHealth = health;//maxHealth is health at the start

        if(healthParent == null) {
            Debug.LogError("there is no healthParent assigned on PlayerHealth");
            hearts = new Animator[0];
            return;
        }

        hearts = new Animator[Mathf.FloorToInt(maxHealth)];
        for(int i = 0; i < hearts.Length; i++) {
            hearts[i] = Instantiate(healthPrefab, healthParent).GetComponent<Animator>();
        }
        UpdateHeart();
    }


    public override void OnHit(GameObject attacker, float damage) {
        base.OnHit(attacker, damage);
        UpdateHeart();
    }

    [ContextMenu("Update Heart")]
    private void UpdateHeart() {
        for (int i = 0; i < hearts.Length; i++) {
            hearts[i].SetBool("Pop", health <= i);//0 - 0.999 one heart, 1 - 1.9999 two hearts, etc.
        }
    }


    public override void OnDeath() {
        base.OnDeath();
        Score.SetScore(0.0f);//clear score on death
        SceneManager.LoadScene(GameOverScene, LoadSceneMode.Single);
    }
}
