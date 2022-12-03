using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    private PollingStation station;
    private void Awake() {
        if (!PollingStation.TryGetPollingStation(out station, gameObject))
            return;
    }

    public GameObject interactionLightPrefab;
    private GameObject interactionLight;

    public virtual void OnInteract() { }

    public virtual void OnInteractStartPossible() {
        interactionLight = Instantiate(interactionLightPrefab, transform);
    }
    public virtual void OnInteractStopPossible() {
        Destroy(interactionLight);
    }
}
