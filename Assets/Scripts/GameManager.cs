using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;
    void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField]
    private UnityEngine.UI.Text fpsText;

    public void Start()
    {
        // для управления движением камеры
        // (по дефолту перед началом всегда будет ставиться Camera Mode, да бы адаптироваться под текущий экран)
        // после будет замена на World Space Mode и будет доступно изменение размера камеры
        if (SceneManager.GetActiveScene().name != GameSceneType.Menu.ToString())
        {
            // change render mode
            GameObject.Find("BackUI").GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;

            // play start animation(fight)
            StartAnimation();
        } // if
    }
    public void Update()
    {
        // DEMO
        fpsText.text = (1f / Time.smoothDeltaTime).ToString("0.0");

        #region General
        if (Input.GetKeyDown(KeyCode.C))
            RefreshRound();

        if (Input.GetKeyDown(KeyCode.B))
        {
            // time.timeScale = .5f;
            BossManager.instance.Activate();
        }
        if (Input.GetKeyDown(KeyCode.X))
            BossManager.instance.currentBoss.animator.SetTrigger("Attack1");
            // BossManager.instance.currentBoss.GetDamage(125f);

        // TODO: разделить всех классы боссов на дочерние одного класса BOSS
        // добавлять ссылку на скрипт босса на текущей карты в информацию о текущей карты
        // и от туда брать его и вызывать активацию босса на 5м этапе карты(продумать в дальнейшем) 
        // if (Input.GetKeyDown(KeyCode.T))
        //     BossManager.instance.Activate();
        #endregion

        #region Battle
        // if(SceneManager.GetActiveScene().name != GameSceneType.Menu.ToString())
        // {
        //    // check for round finishing
        //    if (ScoreManager.instance.totalSpawnedCores == PlayerManager.instance.currentMapStage.prefabAmount && CoreManager.instance.IsInvoking("StartAddingCores"))
        //         CoreManager.instance.StopAddingCores();



        //    if (ScoreManager.instance.totalDeadCores == PlayerManager.instance.currentMapStage.prefabAmount)
        //         PlayRoundFinishingAnimation();
        //}
        #endregion
    }

    #region Utils
    // root of round(pause continue)
    public void PauseRound()
    {
        Time.timeScale = 0;
    }
    public void ContinueRound()
    {
        Time.timeScale = 1f;
    }
    // root on game scene(load refresh)
    public void RefreshRound()
    {
        // munual mode controlled achievement
        // Achievement ach = AchievementManager.instance.achievements.Find((t) => t.title == "Beautiful Death");
        // if (ach.isActive == true && !ach.goal.isReached())
        //    ach.Complete();

        // fixing the death
        //AchievementManager.instance.deathAchievement(ModeType.All, GoalType.Death);

        // reloading the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ContinueRound();
    }

    // game scene loading by name
    public void LoadGameScene(GameSceneType gameSceneType)
    {
        if (gameSceneType == GameSceneType.Menu)
            SceneManager.LoadScene("Menu");
        else if (gameSceneType == GameSceneType.Normal)
            SceneManager.LoadScene("Normal Mode");
        else if (gameSceneType == GameSceneType.Hardcore)
            SceneManager.LoadScene("Hardcore Mode");
        else
            SceneManager.LoadScene("Reverse");
    }
    public void PlayRoundFinishingAnimation()
    {
        // TODO: make animation of happy ending of round
        // with scale camera on main hero(ship) and invoking finishing panel
        // maybe it'll be better to split finishing panels up for NORMAL and BOSS rounds
        //
        // and to split PlayRoundFinishingAnimation for 1-4 and 5(boss) stages
        // 1-4 stages have to end with slowmotion to make game looks cooler
        // 5 stage will has it's own boss ending animation, so there's no need to make it here
        print("Round is finished!");

        ShipController.instance.isMovementAllowed = false;

        // UPD: replace this method to StateManager "Victory"
    }
    // start animation controlled manually
    private void StartAnimation()
    {
        // start adding live units in 1 seconds after starting bottom effect
        LifeUnitsManager.instance.Invoke("ShowLifeUnits", 3.8f);

        if(PlayerManager.instance.currentMapStage.stageIndex != 5)
        {
            // start spawning cores after adding all live inits(5 * 0.2 = 1 + -0.2)
            CoreManager.instance.Invoke("StartAddingCores", 4.2f);
        }
        else
        {
            // boss activating after N seconds
            BossManager.instance.Invoke("Activate", 4.2f);
        }

    }
    #endregion
}
public enum GameSceneType
{   
    Menu,
    Normal, 
    Hardcore,
    Story
}
