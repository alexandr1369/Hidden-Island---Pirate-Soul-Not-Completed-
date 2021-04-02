using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ShipInfo ship;

    public Image inventorySlot;

    // update data info
    public void UpdateInfo()
    {
        inventorySlot.sprite = Resources.Load<Sprite>(@"Textures\Inventory\Slots\" + ship.label);
    }
}
