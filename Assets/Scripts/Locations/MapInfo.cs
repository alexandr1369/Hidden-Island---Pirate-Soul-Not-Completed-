using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// whole map info
[CreateAssetMenu(fileName = "New Map", menuName = "MapInfo")]
public class MapInfo : ScriptableObject
{
    public bool isOpened; // check for availability

    [Header(header: "Name")]
    new public string name; // name of map

    [Header(header: "Playing Field")]
    public Sprite sprite; // sprite of current map

    public AnimalType animalType; // type of animal for 1-3rd Stages
    public UniquePrefab uniquePrefab; // unique prefab for 4th Stage
    public ChestType chestType; // type of chest for 1-4 stages(ending core)

    public List<int> scorePoints; // score points for each core
    public List<StageInfo> stagesInfo; // stages info
}
public enum AnimalType
{
    Pug,
    Husky
    // ...
}
public enum UniquePrefab
{
    Tnt
    // ...
}
public enum ChestType
{
    ChinaStar
    // ...
}
