using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDeselectingButton : ToggleButton
{
    public TrackMovingManager trackMovingManager; // current map TrackMovingManager

    protected override void Perform()
    {
        // reset map animated items
        trackMovingManager.mapItemsAnimators.Find(t => t.name == "Sign").SetBool("Appearing", false);
        trackMovingManager.mapItemsAnimators.Find(t => t.name == "Pistol").SetBool("Falling", false);
        trackMovingManager.mapItemsAnimators.Find(t => t.name == "Holes").SetBool("Shooting", false);

        base.Perform();
    }
}
