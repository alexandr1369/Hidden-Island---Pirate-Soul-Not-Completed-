using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreCoinsWrapManager : MonoBehaviour
{
    #region Singleton
    public static StoreCoinsWrapManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public Image label; // name label

    public TextMeshProUGUI coinAmount; 
    public TextMeshProUGUI soulAmount;

    public GameObject shipsCoinsWrap;
    public GameObject shipsSoulsWrap;

    private void Start()
    {
        print(PlayerManager.instance.currentShip.label);
    }

    public void LoadCoinStore()
    {
        // set name label
        label.sprite = Resources.Load<Sprite>("Textures/Store/CoinStore");

        // show ships panel
        shipsCoinsWrap.SetActive(true);
        shipsSoulsWrap.SetActive(false);

        // refresh currency panel
        UpdateWrapInfo();
    }
    public void LoadSoulStore()
    {
        // set name label
        label.sprite = Resources.Load<Sprite>("Textures/Store/SoulStore");

        // show ships panel
        shipsCoinsWrap.SetActive(false);
        shipsSoulsWrap.SetActive(true);

        // refresh currency panel
        UpdateWrapInfo();
    }
    private void UpdateWrapInfo()
    {
        coinAmount.text = PlayerManager.instance.coinAmountText.text;
        soulAmount.text = PlayerManager.instance.soulAmountText.text;
    }
}
