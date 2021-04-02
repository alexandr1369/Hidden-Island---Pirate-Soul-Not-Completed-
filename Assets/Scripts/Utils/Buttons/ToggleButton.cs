using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : InteractableButton
{
    // панель для переключения
    [SerializeField]
    protected GameObject panel;

    // доп. настройки
    [SerializeField]
    private bool isPauseNeeded; // is needed only in battle mode
    [SerializeField]
    private bool isSmoothAnimated = true;

    protected override void Perform()
    {
        // get panel state
        bool state = panel.activeSelf;

        // turn on/off pause
        if (isPauseNeeded)
        {
            // panel still active but is about to be closed
            if (state)
            {
                // start counting
                CounterManager.instance.Count();
            }
            // panel is not active but is about to be open
            else
            {
                // stop counting(if it does)
                CounterManager.instance.StopCounting();

                // pause round
                GameManager.instance.PauseRound();
            } // if else
        } // if(pause)

        // animated open & close
        if (isSmoothAnimated)
        {
            // animated panel opening
            if (!panel.activeSelf)
            {
                // refresh animator(set all animation to theirs inital positions)
                if (isAnimated)
                    animator.Rebind();

                // refresh local scale of Main panel(it seems to me some lags come out form this)
                // that's why need to check whether it's already (1, 1) or not
                if (panel.transform.Find("Main").transform.localScale != new Vector3(1, 1))
                    panel.transform.Find("Main").transform.localScale = new Vector2(1, 1);

                // little scaling of panel from .95 to 1 
                iTween.ScaleFrom(panel.transform.Find("Main").gameObject, iTween.Hash(
                    "scale", new Vector3(.9f, .9f),
                    //"delay", .1f,
                    "time", .15f,
                    "easetype", iTween.EaseType.easeOutExpo,
                    "ignoretimescale", true
                ));

                // have to force assign otherwise it may causes some lag at the beginning
                panel.GetComponent<CanvasGroup>().alpha = 0;

                // increasing alpha changel on panel from .75 to 1
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", 0,
                    "to", 1,
                    //"delay", .1f,
                    "time", .15f,
                    "easetype", iTween.EaseType.easeOutExpo,
                    "onupdate", "UpdateCanvasGroupAlphaChanel",
                    "ignoretimescale", true
                ));

                // open panel
                TogglePanel();
            }
            // animated panel closing
            else
            {
                // little scaling of panel from 1 to .85
                iTween.ScaleTo(panel.transform.Find("Main").gameObject, iTween.Hash(
                    "scale", new Vector3(.85f, .85f),
                    "time", .15f,
                    "easetype", iTween.EaseType.easeInExpo,
                    "ignoretimescale", true
                ));

                // increasing alpha changel on panel from .75 to 1
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", 1,
                    "to", 0,
                    "time", .15f,
                    "easetype", iTween.EaseType.easeInExpo,
                    "ignoretimescale", true,
                    "onupdate", "UpdateCanvasGroupAlphaChanel",
                    "oncomplete", "TogglePanel"
                ));
            } // if else
        }
        // defaul open & close
        else
        {
            TogglePanel();
        }

        // get parent initial logic
        base.Perform();
    } // Perform

    #region SmoothAnimationUtils
    private void UpdateCanvasGroupAlphaChanel(float value)
    {
        panel.GetComponent<CanvasGroup>().alpha = value;
    }
    // open & close panel
    private void TogglePanel()
    {
        bool state = panel.activeSelf;
        if (isSmoothAnimated && !state)
        {
            panel.transform.Find("Main").transform.localScale = new Vector3(.9f, .9f);
        }

        panel.SetActive(state ? false : true);
    }
    #endregion
}
