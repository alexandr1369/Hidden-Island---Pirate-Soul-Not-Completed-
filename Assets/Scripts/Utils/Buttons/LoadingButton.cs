using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingButton : InteractableButton
{
    // сцена для загрузки
    [SerializeField]
    private GameSceneType gameSceneType;
    [SerializeField]
    private MapInfo mapInfo;
    [SerializeField]
    private int stageIndex;

    protected override void Perform()
    {
        //// scene loading
        //if (gameSceneType == GameSceneType.Menu)
        //{
        //    // if loading scene is menu - clear info of current map
        //    PlayerManager.instance.currentMap = null;
        //    PlayerManager.instance.currentMapStageIndex = 0; 
        //}
        //else
        //{
        //    // else - load info of current map
        //    PlayerManager.instance.currentMap = mapInfo;
        //    PlayerManager.instance.currentMapStageIndex = stageIndex;
        //}

        // load scene
        print($"Loading Scene {gameSceneType.ToString()}");
        GameManager.instance.LoadGameScene(gameSceneType);
    }
}
