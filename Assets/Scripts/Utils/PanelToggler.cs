using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// переключение отображения панели
public class PanelToggler : MonoBehaviour
{
    public GameObject panel;

    public bool isSmoothAnimated = true;

    private CanvasGroup canvasGroup;

    public void Start()
    {
        canvasGroup = panel.GetComponent<CanvasGroup>();
    }

    public void TogglePanel()
    {
        // animated open & close
        if (isSmoothAnimated)
        {
            print("Opening");
            // animated panel opening
            if (!panel.activeSelf)
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

                // have to force assign otherwise it may causes some lag at the beginning
                canvasGroup.alpha = 0;

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

                // open panel
                TogglePanelHelper();
            }
            // animated panel closing
            else
            {
                print("Closing");
                // little scaling of panel from 1 to .85
                iTween.ScaleTo(panel.transform.Find("Main").gameObject, iTween.Hash(
                    "scale", new Vector3(.85f, .85f),
                    "time", .15f,
                    "easetype", iTween.EaseType.easeInExpo,
                    "ignoretimescale", true
                ));

                // decreasing alpha chanel on panel from 1 to 0
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", 1,
                    "to", 0,
                    "time", .15f,
                    "easetype", iTween.EaseType.easeInExpo,
                    "ignoretimescale", true,
                    "onupdate", "UpdateCanvasGroupAlphaChanel",
                    "oncomplete", "TogglePanelHelper"
                ));
            } // if else
        }
        // defaul open & close
        else
        {
            TogglePanelHelper();
        }
    }
    #region SmoothAnimationUtils
    // canvas group alpha chanel updating
    private void UpdateCanvasGroupAlphaChanel(float value)
    {
        canvasGroup.alpha = value;
    }
    // open & close panel
    private void TogglePanelHelper()
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
