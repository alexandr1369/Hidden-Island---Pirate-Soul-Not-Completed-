using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StoreDisplay))]
public class StoreDisplayOpeningButton : ToggleButton
{
    private StoreDisplay storeDisplay;

    protected override void Perform() 
    {
        // check for moving of scroll rect
        if (downPosition != upPosition)
            return;

        // store display opening
        storeDisplay = GetComponent<StoreDisplay>();
        storeDisplay.LoadStoreInfoPanel();

        // animated panel toggling
        base.Perform();
    }
}
