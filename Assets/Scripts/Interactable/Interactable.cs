using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    private PollingStation station;
    private void Awake() {
        if (!PollingStation.TryGetPollingStation(out station, gameObject))
            return;
    }

    public virtual void OnInteract() { }
}
