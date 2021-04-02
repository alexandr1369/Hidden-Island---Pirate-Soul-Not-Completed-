using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    #region Singleton
    public static BossManager instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    #endregion

    [SerializeField]
    private List<Boss> allBosses; // all bosses scipts

    public Boss currentBoss; // current boss

    private void Start()
    {
        // get current boss script
        switch (PlayerManager.instance.currentMap.animalType)
        {
            case AnimalType.Pug: { currentBoss = allBosses.Find(t => t.GetType() == typeof(BossPug)); } break;
            case AnimalType.Husky: { currentBoss = allBosses.Find(t => t.GetType() == typeof(BossHusky)); } break;
        }
    }

    public void Activate()
    {
        // check for right map stage
        if (PlayerManager.instance.currentMapStage.stageIndex != 5)
            return;

        // activate boss
        currentBoss.Activate();
    }
}
