using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(PanelToggler))]
public class StoreInfoPanelDisplay : MonoBehaviour
{
    #region Singleton
    public static StoreInfoPanelDisplay instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject panel; // store info panel
    public GameObject errorPanel; // missing money info panel(todo)

    public Image icon; // ship icon

    public Text price; // ship price
        
    public GameObject[] currency; // currency icon (0 - coin, soul - 1)
    public GameObject[] labelWraps; // label wraps which are set manually

    private ShipInfo shipInfo; // current shipInfo

    public void LoadInfoPanel(ShipInfo ship)
    {
        // getting ship info
        shipInfo = ship;

        // set ship icon
        icon.sprite = shipInfo.icon;

        // set name label
        SetLabel(shipInfo.label);

        // set info about price
        price.text = $"Would you like to buy ship ship for       {shipInfo.price}?";

        // set currency icon
        currency[0].SetActive(false); currency[1].SetActive(false); // hide all

        if (shipInfo.currency == Currency.Coin)
            currency[0].SetActive(true); // show coin
        else
            currency[1].SetActive(true); // show soul
    }
    // buy ship
    public void BuyShip()
    {
        // buy the ship
        shipInfo.BuyShip();
    }
    public void NotEnoughMoney()
    {
        // show error panel
        GetComponent<PanelToggler>().TogglePanel();
    }

    // setting right label name
    private void SetLabel(string name)
    {
        foreach (GameObject labelWrap in labelWraps)
        {
            labelWrap.SetActive(false);
            if (labelWrap.name == name)
                labelWrap.SetActive(true);
        } // foreach
    }
}
