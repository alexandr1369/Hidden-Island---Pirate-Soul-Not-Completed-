using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager instance;
    void Awake()
    {
        if(instance == null)
            instance = this;

        // init 
        Init();

        // forbid destroying on load
        DontDestroyOnLoad(instance);
    }
    #endregion

    public int coinAmount; // current amount of coins
    public int soulAmount; // current amount of souls

    public Text coinAmountText; // coin amount text
    public Text soulAmountText; // soul amount text

    public ShipInfo[] myShips; // array of all ships

    public ShipInfo currentShip; // current chosen ship

    public MapInfo currentMap; // current playing map
    public StageInfo currentMapStage; // stage index

    // adding coins
    public void AddCoins(int amount)
    {
        coinAmount += amount;
        PlayerPrefs.SetInt("CoinAmount", coinAmount);

        if(SceneManager.GetActiveScene().name == "Menu")
            coinAmountText.text = coinAmount.ToString();
    }
    // adding soul 
    public void AddSouls(int amount)
    {
        soulAmount += amount;
        PlayerPrefs.SetInt("SoulAmount", soulAmount);

        if (SceneManager.GetActiveScene().name == "Menu")
            soulAmountText.text = soulAmount.ToString();
    }
    // get all purchased ships
    public List<ShipInfo> GetPurchasedShips()
    {
        List<ShipInfo> purchasedShips = new List<ShipInfo>();
        foreach(ShipInfo ship in myShips)
            if (!ship.isActive)
                purchasedShips.Add(ship);

        return purchasedShips;
    }
    public void SetCurrentShip(string name)
    {
        foreach (ShipInfo ship in myShips)
        {
            if (ship.label == name)
            {
                PlayerPrefs.SetString("CurrentShip", name);
                currentShip = ship;
                break;
            } // if
        } // foreach
    }

    // start settings
    private void Init() 
    {
        #region load money info
        if (PlayerPrefs.HasKey("CoinAmount"))
            coinAmount = PlayerPrefs.GetInt("CoinAmount");
        else
        {
            coinAmount = 1000;
            PlayerPrefs.SetInt("CoinAmount", coinAmount);
        }

        if (PlayerPrefs.HasKey("SoulAmount"))
            soulAmount = PlayerPrefs.GetInt("SoulAmount");
        else
        {
            soulAmount = 50;
            PlayerPrefs.SetInt("SoulAmount", soulAmount);
        }

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            coinAmountText.text = coinAmount.ToString();
            soulAmountText.text = soulAmount.ToString();
        }
        #endregion
        #region load ship info
        string _name = string.Empty;
        if (!PlayerPrefs.HasKey("CurrentShip"))
        {
            _name = "Black Flag";
            SetCurrentShip(_name); // Balck Flag should be the first added
        }
        else
        {
            _name = PlayerPrefs.GetString("CurrentShip");
            SetCurrentShip(_name);
        } // if else
        #endregion
        #region load maps info
        // TODO create map class and add CreateAssetMenu attr 
        // then make the same thing as with achievements or ships
        // class must cantain name(string), image(Sprite) and isActive(bool)
        #endregion
    }
}
