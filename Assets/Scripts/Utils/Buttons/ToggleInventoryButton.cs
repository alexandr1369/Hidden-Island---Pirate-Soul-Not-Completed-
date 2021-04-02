using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleInventoryButton : ToggleButton
{
    [SerializeField]
    private ParticleSystem bgEffect;

    protected override void Perform()
    {
        // toggle effects
        if (!bgEffect.gameObject.activeSelf)
            bgEffect.gameObject.SetActive(true);
        else
            bgEffect.gameObject.SetActive(false);

        // animated panel toggling
        base.Perform();
    }
}
