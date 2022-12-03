using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<PlayerController>()) {
            OnCollect();
        }
    }

    public virtual void OnCollect() {
        Destroy(gameObject);
    }
}
