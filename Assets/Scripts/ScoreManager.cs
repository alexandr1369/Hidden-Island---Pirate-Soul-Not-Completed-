using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    #region Singleton
    public static ScoreManager instance;
    void Awake()
    {
        instance = this;
    }
    #endregion

    public int score; // score of current battle
    public int scoreMultiplier; // score multiplier

    public int lifeUnits; // amount of lifeUnits

    // statistics
    public int totalSpawnedCores; // amount of spawned cores
    public int totalDeadCores; // amount of killed animals duaring current battle
    public int totalSpawnedEffects; // amount of spawned effects(buffs)
    public int totalObtainedEffects; // amount of obtained effects(buffs)

    private Dictionary<MobType, int> deadCores; // list of dead cores with info about MobType

    private Dictionary<MobType, int> coreRanks; // list of core's points(according to it's rank)

    public float acceleration; // acceleration of current core's prefab


    private Text scoreTable; // score table on the top of battle map

    void Start()
    {
        Init();
    }

    // print the score
    void OnGUI()
    {
        // print the score
        scoreTable.text = score.ToString();
    }

    #region Utils
    // init settings
    void Init()
    {
        // set inits
        score = totalSpawnedEffects = totalObtainedEffects = totalDeadCores = totalSpawnedCores = 0;
        scoreMultiplier = scoreMultiplier < 1 ? 1 : scoreMultiplier;

        // get the scores for any rank of mob
        coreRanks = new Dictionary<MobType, int>();
        coreRanks.Add(MobType.Common, 1);
        coreRanks.Add(MobType.Rare, 3);
        coreRanks.Add(MobType.Royal, 5);

        // set count of dead mobs of every rank
        deadCores = new Dictionary<MobType, int>();
        deadCores.Add(MobType.Common, 0);
        deadCores.Add(MobType.Rare, 0);
        deadCores.Add(MobType.Royal, 0);

        // get score table
        scoreTable = GameObject.Find("RootPanel/ScorePanel").GetComponent<Text>();
    }
    // event of killing the core
    public void KillCore(MobType type)
    {
        // increment total amount of kills(dead mobs)
        totalDeadCores++;

        // increment types of dead mobs(1-3)
        deadCores[type]++;
        score += coreRanks[type] * scoreMultiplier;
        string info = $"Amount: {totalDeadCores} Common: {deadCores[MobType.Common]} Rare: {deadCores[MobType.Rare]} Royal: {deadCores[MobType.Royal]} Saved: {totalSpawnedCores - totalDeadCores}";
    }
    // save core(ship missed core in the battle)
    public void SaveCore()
    {
        // get damage
        GetDamage();
    }
    public void SpawnCore()
    {
        // increase spawned cores amount
        totalSpawnedCores++;
    }
    // ship gets damage(from hitting or missing cores)
    public void GetDamage()
    {
        //decrease live units
       lifeUnits--;

        //decreace live points
        LifeUnitsManager.instance.RemoveLifeUnit();

        if (lifeUnits == 0)
            StateManager.instance.Defeat();
    }
    #endregion
}
public enum MobType
{
    Common,
    Rare,
    Royal
}

