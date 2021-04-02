using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationManager : MonoBehaviour
{
    public Text scorePanel; // score panel

    public List<GameObject> backgrounds; // all backgrounds

    public void Start()
    {
        // get map name
        string mapName = PlayerManager.instance.currentMap.name;

        // set background
        foreach (GameObject background in backgrounds)
            if (background.name != mapName && background.activeSelf)
                background.SetActive(false);
            else if (background.name == mapName && !background.activeSelf)
                background.SetActive(true);

        // set score text color
        Dictionary<string, Color32> scoreTextColors = new Dictionary<string, Color32>()
        {
            { "Sunset Fog", new Color32(224, 193, 193, 100)},
            { "Summer Beach", new Color32(227, 229, 223, 160)}
        };

        scorePanel.color = scoreTextColors[mapName];
    }
}
