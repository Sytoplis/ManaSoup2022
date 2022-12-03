using UnityEngine;

public class ExitBed : Interactable
{
    public override void OnInteract() {
        base.OnInteract();

        Debug.Log("EXIT");
    }
}
