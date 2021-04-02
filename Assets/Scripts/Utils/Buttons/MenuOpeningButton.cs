using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MenuOpener))]
public class MenuOpeningButton : InteractableButton
{
    private MenuOpener menuOpener;

    protected override void Perform()
    {
        // menu opening
        menuOpener = GetComponent<MenuOpener>();
        menuOpener.OpenMenu();
    }
}
