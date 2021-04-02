using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChapterStage))]
public class AsyncLoadingButton : InteractableButton
{
    public ChapterStage chapterStage;

    public GameSceneType sceneType;

    public GameObject loadingPanel;

    public Animator loadingPanelAnimator;

    protected override void Perform()
    {
        // check for moving of scroll rect
        if (upPosition != downPosition)
            return;

        // set map stage
        PlayerManager.instance.currentMapStage = PlayerManager.instance.currentMap.stagesInfo.Find(t => t.stageIndex == chapterStage.stageInfo.stageIndex);

        // play loading animation randomly
        if (animator == null) return;

        loadingPanel.SetActive(true);

        string[] animationNames = { "LoadingOwl", "LoadingMole" };
        loadingPanelAnimator.SetTrigger(animationNames[Random.Range(0, animationNames.Length)]);

        // start async loading
        LoadingManager.instance.StartLoadingScene(sceneType);
    }
}
