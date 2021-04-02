using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

[CreateAssetMenu(fileName = "New Ship", menuName = "ShipInfo")]
public class ShipInfo : ScriptableObject
{
    public bool isActive; // check for purchasing current ship(false - bought)

    [Header(header: "Name")]
    public string label; // name

    [Header(header: "Money")]
    public int price; // price 
    public Currency currency; // currency type
    public Sprite icon; // ship icon

    [Header(header: "Animation")]
    public bool isAnimated; // check for animated ship

    // TODO: понять как экспортировать и потом импортировать particle system settings(возможно сделать префабы)
    //public GameObject ShipEffect; // animated ShipEffect with particle system
    public ShipEffect ShipEffect;

    // buy the ship method
    public void BuyShip()
    {
        // remove money
        if (currency == Currency.Coin)
        {
            if (PlayerManager.instance.coinAmount < price)
            {
                StoreInfoPanelDisplay.instance.NotEnoughMoney();
                return;
            }
            PlayerManager.instance.AddCoins(price * -1);
        }
        else
        {
            if (PlayerManager.instance.soulAmount < price)
            {
                StoreInfoPanelDisplay.instance.NotEnoughMoney();
                return;
            }
            PlayerManager.instance.AddSouls(price * -1);
        }

        // update information
        isActive = false;

        // update inventory information
        InventoryScroll.instance.LoadData();
    }
}

// тип валюты
public enum Currency
{
    Coin,
    Soul
}

public enum ShipEffect
{
    Stars,
    Fire,
    Snow,
    Smoke
}
