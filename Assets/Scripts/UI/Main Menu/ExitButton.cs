using UnityEngine;

public class ExitButton : ButtonTemplate
{
    public override void OnClick(){
        Debug.Log("Exiting Game");
        Application.Quit();
    }
}
