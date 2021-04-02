using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DefeatManager : MonoBehaviour
{
    public Transform chest; // chest transform
    private Vector3 chestStartPosition; // chest start position

    public Transform flag; // flag transform

    public TextMeshProUGUI text; // text of completion
    public TextMeshProUGUI textShadow; // text shadow of completion

    public Animator contentHoverAnimator; // animated of c.h.(when text is about it's mask)

    public ScrollRect scrollRect; // scroller of all content
    private float yContentStartPosition; // start top y of content of scroll rect

    private float _percent; // percent of game completion

    public void Start()
    {
        Init();
    }
    public void Update()
    {
        // check for content hover
        if (scrollRect.verticalNormalizedPosition < .99 && !contentHoverAnimator.GetBool("Showing"))
            contentHoverAnimator.SetBool("Showing", true);
        else if (scrollRect.verticalNormalizedPosition >= .99 && contentHoverAnimator.GetBool("Showing"))
            contentHoverAnimator.SetBool("Showing", false);
    }
    public void Init()
    {
        // text init
        text.text = textShadow.text = "Battle Completion - 0%";

        // prevent vertical scrolling
        PreventVerticalScrolling();

        // get start top y of content
        yContentStartPosition = scrollRect.content.transform.localPosition.y;
    }
    // move chest destroying all circles passing by
    public void MoveChest()
    {
        // get distance between flag and chest(the length of progress bar)
        float _distance = chest.position.x - flag.position.x;

        // get percent of match finishing
        _percent = Random.Range(0, 1f);
        //_percent = (float)ScoreManager.instance.totalDeadCores / PlayerManager.instance.currentMapStage.prefabAmount;

        // get x position of moving to
        float _moveToX = flag.position.x + _distance - _distance * _percent;

        // animate movement
        if (_percent == 0)
            MoveToCompletion();
        else
            iTween.MoveTo(chest.gameObject, iTween.Hash(
               "x", _moveToX,
               "time", 1.5f,
               "easetype", iTween.EaseType.easeInOutCubic,
               "ignoretimescale", true,
               "oncomplete", "MoveToCompletion",
               "oncompletetarget", gameObject
           ));
    }
    // move content of scrollRect to Completion Panel
    public void MoveToCompletion() => MoveTo("Completion", "AnimatedShowingPercentOfRoundFinishing", .75f); 
    // move content of scrollRect to Retive Button
    public void MoveToBtnRevive() => MoveTo("BtnRevive", "AllowVerticalScrolling", 1f);

    #region animation utils

    /// <summary>
    /// Moving ScrollRect content to it's parts inside, according to VerticalNormalizedPosition.
    /// </summary>
    /// <param name="name">Name of part inside content.</param>
    /// <param name="callback">Name of callback function(oncomplete).</param>
    /// <param name="time">Time of animation.</param>
    /// <param name="easeType">Easetype of animation.</param>
    private void MoveTo(string name, string callback = null, float time = .5f, iTween.EaseType easeType = iTween.EaseType.easeInOutBack)
    {
        // get transform of needed panel inside the content
        GameObject _panel = GameObject.Find("DefeatPanel/Main/InfoPanel/Mask/Content/" + name);
        Transform _panelTransform = _panel.transform;
        RectTransform _panelRectTransform = _panel.GetComponent<RectTransform>();

        // get top and bottom edges of mask(the visible part of content)
        float topY = GameObject.Find("InfoPanel/Mask").transform.position.y + GameObject.Find("InfoPanel/Mask").GetComponent<RectTransform>().rect.height / 2;
        float bottomY = GameObject.Find("InfoPanel/Mask").transform.position.y - GameObject.Find("InfoPanel/Mask").GetComponent<RectTransform>().rect.height / 2;

        // get top and bottom edges of required panel
        float panelTopY = _panelTransform.position.y + _panelRectTransform.rect.height / 2;
        float panelBottomY = _panelTransform.position.y - _panelRectTransform.rect.height / 2;

        // get direction of scroll rect moving (up/down) and move scoll rect according to calculated values
        float yLeakedAxisLength, verticalNormalizedPosition = 1f;
        bool moveRequired = false;
        if (panelTopY > topY)
        {
            // get vertical normalized position
            yLeakedAxisLength = Mathf.Abs(panelTopY - topY);
            verticalNormalizedPosition = Mathf.Abs((scrollRect.content.transform.localPosition.y - yLeakedAxisLength + yContentStartPosition) / (2 * yContentStartPosition));
            moveRequired = true;
        }
        else if(panelBottomY < bottomY)
        {
            // get vertical normalized position
            yLeakedAxisLength = Mathf.Abs(panelBottomY - bottomY);
            verticalNormalizedPosition = Mathf.Abs((scrollRect.content.transform.localPosition.y + yLeakedAxisLength + yContentStartPosition) / (2 * yContentStartPosition));
            moveRequired = true;
        }

        if (moveRequired)
        {
            // move scroll
            if (callback != null)
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", scrollRect.verticalNormalizedPosition,
                    "to", verticalNormalizedPosition,
                    "time", time,
                    "easetype", easeType,
                    "ignoretimescale", true,
                    "onupdate", "SetVerticalNormalizedPosition",
                    "oncomplete", callback
                ));
            else
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", scrollRect.verticalNormalizedPosition,
                    "to", verticalNormalizedPosition,
                    "time", time,
                    "easetype", easeType,
                    "ignoretimescale", true,
                    "onupdate", "SetVerticalNormalizedPosition"
                ));
        }   
    }
    private void PreventVerticalScrolling()
    {
        scrollRect.vertical = false;
    }
    private void AllowVerticalScrolling()
    {
        scrollRect.vertical = true;
    }
    private void SetVerticalNormalizedPosition(float value)
    {
        scrollRect.verticalNormalizedPosition = value;
    }

    #endregion

    #region animaed text counting utility

    private void AnimatedShowingPercentOfRoundFinishing()
    {
        // speed up the animation if completion percent is 0
        if (_percent == 0)
            MoveToBtnRevive();

        // animatedly showing pecent of round finishing
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 0f,
            "to", _percent * 100f,
            "time", 2f,
            "easetype", iTween.EaseType.easeInOutQuad,
            "ignoretimescale", true,
            "onupdate", "SetCompletionText",
            "oncomplete", "MoveToBtnRevive"
        ));
    }
    private void SetCompletionText(float value)
    {
        text.text = textShadow.text = $"Battle Completion - {value:,0}%";
    }

    #endregion

}
