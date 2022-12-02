using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : ButtonTemplate
{
    public override void OnClick(){
        Debug.Log("Exiting Game");
    }
}
