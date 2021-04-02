using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AchievementDisplay))]
public class AchievementDisplayOpeningButton : ToggleButton
{
    private AchievementDisplay achDisplay;

    protected override void Perform()
    {
        // check for moving of scroll rect
        if (downPosition != upPosition)
        {
            return;
        }

        // achievement display opening
        achDisplay = GetComponent<AchievementDisplay>();
        achDisplay.OpenAchievementInfoPanel();


        // animated panel toggling
        base.Perform();
    }
}
