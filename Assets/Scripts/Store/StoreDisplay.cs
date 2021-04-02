using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StoreDisplay : MonoBehaviour
{
    public ShipInfo shipInfo;

    [SerializeField]
    private Text priceText; // price text 
    [SerializeField]
    private Image currencyImage; // currency image
    [SerializeField]
    private Image shipImage;  // image

    void Start()
    {
        Init();
    }

    private void Update()
    {
        if (CheckForAvailability())
        {
            DisableButton();
        } // if
    }

    // load buy panel
    public void LoadStoreInfoPanel()
    {
        StoreInfoPanelDisplay.instance.LoadInfoPanel(shipInfo);
    }

    // start settings
    private void Init()
    {
        shipImage.sprite = shipInfo.icon;
        priceText.text = shipInfo.price.ToString();
        currencyImage.sprite = Resources.Load<Sprite>("Textures/Store/" + shipInfo.currency.ToString());
    }

    private bool CheckForAvailability()
    {
        return (!shipInfo.isActive && GetComponent<InteractableButton>().isInteractable);
    }
    // disable button from clicking(ship is sold)
    private void DisableButton()
    {
        InteractableButton btn = GetComponent<InteractableButton>();
        btn.isInteractable = false;
        btn.isAnimated = false;
        // add hover image to inform about ship is sold
    }
}

