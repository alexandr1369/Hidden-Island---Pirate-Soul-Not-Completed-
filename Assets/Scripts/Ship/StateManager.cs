using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    #region Singleton
    public static StateManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public Camera mainCamera; // main camera

    private void Update()
    {
        // demo
        if (Input.GetKeyDown(KeyCode.Space))
            Defeat();
        if (Input.GetKeyDown(KeyCode.R))
            Revive();
    }
    // victory animation
    public void Victory()
    {
        /*
         *когда босс или все собаки будут повержены:
         1) кораблик продолжает телепаться [x]
         2) все это после того как босса умер или в середине по event [x]
         3) анимация победы(выстрелы из пушки IDN) [?]
         4) victory panel appearing [+]
         5) выпадение сундука и окошко его открытия [?]
        */

        // pause the game but continue ship dragon bones animations
        GameManager.instance.PauseRound();
        ShipController.instance.isMovementAllowed = false;
        ShipManager.instance.GetComponent<DragonBonesUpdateMode>().ToggleUpdateMode();

        // show victory panel 
        GameObject.Find("MainPanel/VictoryPanel").GetComponent<PlayingFieldToggler>().TogglePanel();
    }
    // defeat animation
    public void Defeat()
    {
        // pause the game but continue ship dragon bones animations
        GameManager.instance.PauseRound();
        ShipController.instance.isMovementAllowed = false;
        ShipManager.instance.GetComponent<DragonBonesUpdateMode>().ToggleUpdateMode();

        // set alpha of score panel to 0
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 1f,
            "to", 0,
            "time", 1.5f,
            "easetype", iTween.EaseType.easeInOutQuad,
            "ignoretimescale", true,
            "onupdate", "UpdateScorePanelAlpha"
        ));

        // scale camera orthographic size
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", mainCamera.orthographicSize,
            "to", 2.5f,
            "time", 1.5f,
            "easetype", iTween.EaseType.easeInOutQuad,
            "ignoretimescale", true,
            "onupdate", "UpdateCameraOrthographicSize"
        ));

        // move camera to ship position
        float yCameraPos = ShipController.instance.shipPanel.position.y;
        Vector3 cameraPosition = new Vector3(0f, yCameraPos, mainCamera.transform.position.z);
        iTween.MoveTo(mainCamera.gameObject, iTween.Hash(
            "position", cameraPosition,
            "time", 1.5f,
            "ignoretimescale", true,
            "easetype", iTween.EaseType.easeInOutQuad
        ));

        // move ship to the center of camera
        float xShipStartPos = 0f; // counted manually
        iTween.MoveTo(ShipController.instance.shipPanel.gameObject, iTween.Hash(
            "x", xShipStartPos,
            "time", 1.5f,
            "easetype", iTween.EaseType.easeInOutQuad,
            "ignoretimescale", true,
            "oncomplete", "PlayDefeatAnimation"
        ));
    }

    // resurrect animation
    public void Revive()
    {
        // hide defeat [anel
        GameObject.Find("MainPanel/DefeatPanel").GetComponent<PlayingFieldToggler>().TogglePanel();

        // use itween for 
        PlayRevivingAnimation();

        // play particle system(souls) in "reverse"
        GameObject.Find("RevivingEffect/Souls").GetComponent<ParticleSystem>().Play();

        // other part of script is processed in ShipManager on dragon bones event
    }

    #region Utils
    public void UpdateCameraOrthographicSize(float size)
    {
        mainCamera.orthographicSize = size;
    }
    public void UpdateScorePanelAlpha(float alpha)
    {
        GameObject.Find("RootPanel/ScorePanel").GetComponent<CanvasGroup>().alpha = alpha;
    }
    private void PlayDefeatAnimation()
    {
        ShipManager.instance.BeginDbAnimation("Loosing");
    }
    private void PlayRevivingAnimation()
    {
        ShipManager.instance.BeginDbAnimation("Reviving");
    }
    #endregion
}

