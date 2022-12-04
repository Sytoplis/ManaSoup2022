using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitBed : Interactable
{   
    public int buildIdx;

    public override void OnInteract() {
        base.OnInteract();

        SceneManager.LoadScene(buildIdx, LoadSceneMode.Single);

        Debug.Log("EXIT map");
    }
}
