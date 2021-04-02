using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyShipButton : ToggleButton
{
    protected override void Perform()
    {
        // ship buying
        if (gameObject.name == "BtnYes")
            StoreInfoPanelDisplay.instance.BuyShip();

        // animated panel toggling
        base.Perform();
    }
}
