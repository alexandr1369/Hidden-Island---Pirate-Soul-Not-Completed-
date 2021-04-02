
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingFieldToggler : MonoBehaviour
{
    public GameObject panel;
    public PlayingFieldTogglerType togglerType;

    private CanvasGroup canvasGroup;

    public void Start()
    {
        canvasGroup = panel.GetComponent<CanvasGroup>();
    }

    public void TogglePanel()
    {
        // animated panel opening
        if (canvasGroup.alpha == 0)
        {
            // refresh local scale of Main panel(it seems to me some lags come out form this)
            // that's why need to check whether it's already (1, 1) or not
            if (panel.transform.Find("Main").transform.localScale != new Vector3(1, 1))
                panel.transform.Find("Main").transform.localScale = new Vector2(1, 1);

            // little scaling of panel from .9 to 1 
            iTween.ScaleFrom(panel.transform.Find("Main").gameObject, iTween.Hash(
                "scale", new Vector3(.9f, .9f),
                //"delay", .1f,
                "time", .15f,
                "easetype", iTween.EaseType.easeOutExpo,
                "ignoretimescale", true
            ));

            // increasing alpha changel on panel from 0 to 1
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 0,
                "to", 1,
                //"delay", .1f,
                "time", .15f,
                "easetype", iTween.EaseType.easeOutExpo,
                "onupdate", "UpdateCanvasGroupAlphaChanel",
                "ignoretimescale", true
            ));

            // set local scale
            panel.transform.Find("Main").transform.localScale = new Vector3(.9f, .9f);

            ToggleAnimationsHelpher();
        }
        // animated panel closing
        else
        {
            // little scaling of panel from 1 to .85
            iTween.ScaleTo(panel.transform.Find("Main").gameObject, iTween.Hash(
                "scale", new Vector3(.85f, .85f),
                "time", .14f,
                "easetype", iTween.EaseType.easeInExpo,
                "ignoretimescale", true,
                "oncomplete", "ToggleAnimationsHelpher",
                "oncompletetarget", gameObject
            ));

            // decreasing alpha chanel on panel from 1 to 0
            iTween.ValueTo(gameObject, iTween.Hash(
                "from", 1,
                "to", 0,
                "time", .15f,
                "easetype", iTween.EaseType.easeInExpo,
                "ignoretimescale", true,
                "onupdate", "UpdateCanvasGroupAlphaChanel"
            ));
        } // if else
    }
    #region SmoothAnimationUtils
    // canvas group alpha chanel updating
    private void UpdateCanvasGroupAlphaChanel(float value)
    {
        canvasGroup.alpha = value;
    }
    // play all animations
    private void ToggleAnimationsHelpher()
    {
        if (canvasGroup.alpha == 0)
        {
            foreach (Animator animator in GetComponentsInChildren<Animator>())
                animator.SetTrigger("Appearing");
        }
        else
        {
            if(togglerType == PlayingFieldTogglerType.Defeat)
            {
                // load new defeat panel
                GameObject _defeatPanelPrefab = Resources.Load<GameObject>("Prefabs/PlayingField/DefeatPanel");
                GameObject _currentDefeatPanel = GameObject.Find("MainPanel/DefeatPanel");
                GameObject _newDefeatPanel = Instantiate(_defeatPanelPrefab, _currentDefeatPanel.transform.position, Quaternion.identity, transform.parent);
                _newDefeatPanel.name = "DefeatPanel";

                // remove old one
                Destroy(gameObject);
            }
            else
            {
                // load new defeat panel
                GameObject _victoryPanelPrefab = Resources.Load<GameObject>("Prefabs/PlayingField/VictoryPanel");
                GameObject _currentVictoryPanel = GameObject.Find("MainPanel/VictoryPanel");
                GameObject _newVictoryPanel = Instantiate(_victoryPanelPrefab, _currentVictoryPanel.transform.position, Quaternion.identity, transform.parent);
                _newVictoryPanel.name = "VictoryPanel";

                // remove old one
                Destroy(gameObject);
            }
        }
        
    }
    #endregion
}
public enum PlayingFieldTogglerType
{
    Victory,
    Defeat
}
