using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivingButton : InteractableButton
{
    protected override void Perform()
    {
        if (gameObject.name == "BtnRevive")
        {
            // TODO: add advertisment and on ending -> continue game with reviving(animation)
        }
    }
}
