using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShipButton : InteractableButton
{
    protected override void Perform()
    {
        // ship selecting
        if (gameObject.name == "BtnSelect")
        {
            InventoryScroll.instance.MakeCurrentShipSelected();
        }
    }
}
