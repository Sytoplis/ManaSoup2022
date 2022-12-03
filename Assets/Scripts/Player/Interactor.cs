using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private PollingStation station;
    private List<Interactable> interactables = new List<Interactable>();

    private void Awake() {
        if (!PollingStation.TryGetPollingStation(out station, gameObject))
            return;
    }

    void Update()
    {
        if (station.inputManager.GetButtonDown(InputManager.InputPreset.Interact)) {
            foreach(Interactable interactable in interactables) {
                interactable.OnInteract();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.TryGetComponent(out Interactable interactable))
            interactables.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.TryGetComponent(out Interactable interactable))
            interactables.Remove(interactable);
    }
}
