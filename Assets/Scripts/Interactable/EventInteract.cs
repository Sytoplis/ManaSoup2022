using UnityEngine.Events;

public class EventInteract : Interactable
{
    public UnityEvent onInteract;
    public UnityEvent onStopInteract;

    public override void OnInteract() {
        base.OnInteract();
        onInteract.Invoke();
    }

    public override void OnInteractStopPossible() {
        base.OnInteractStopPossible();
        onStopInteract.Invoke();
    }
}
