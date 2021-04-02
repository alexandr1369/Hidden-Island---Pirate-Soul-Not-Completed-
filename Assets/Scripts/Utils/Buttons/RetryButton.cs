using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton : InteractableButton
{
    protected override void Perform()
    {
        // round refreshing
        if (gameObject.name == "BtnRetry")
            GameManager.instance.RefreshRound();
    }
}
