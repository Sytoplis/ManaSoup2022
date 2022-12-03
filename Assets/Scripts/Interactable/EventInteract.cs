using UnityEngine.Events;

public class EventInteract : Interactable
{
    public UnityEvent onInteract;

    public override void OnInteract() {
        base.OnInteract();
        onInteract.Invoke();
    }
}
