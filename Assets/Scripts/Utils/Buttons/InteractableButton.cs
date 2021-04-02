using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Родительский класс анимированной кнопочки
public class InteractableButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    // states toggles
    public bool isAnimated = true;
    public bool isInteractable = true;
    public bool isClickSoundOn = true;

    protected Animator animator; // animator

    // click sounds
    protected AudioClip btnDownAudioClip;
    protected AudioClip btnUpAudioClip;

    // animation utils
    private bool isReady;
    private bool isPointerUp = true;
    protected bool isPointerUpon;
    protected bool isToggleNeeded;

    // move utils
    protected Vector2 downPosition;
    protected Vector2 upPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        btnUpAudioClip = Resources.Load<AudioClip>("Sounds/btn_up");
        btnDownAudioClip = Resources.Load<AudioClip>("Sounds/btn_down");
    }

    void Update()
    {
        if (isToggleNeeded && isPointerUp)
        {
            isToggleNeeded = false;
            animator.SetTrigger("Unpressed");
        } // if

        if (isReady && isPointerUp)
        {
            // check for interactivity
            if (!isInteractable)
                return;

            // override button logic by changing only this method
            Perform();

            // button releasing
            isReady = false;
        } // if
    } // Update

    /// <summary>
    /// Main logic of button work.
    /// </summary>
    protected virtual void Perform() { }

    #region OnPointerUtils

    public void OnPointerUp(PointerEventData eventData)
    {
        // get mouse up position
        upPosition = Input.mousePosition;

        // mouse release if it's still upon the button
        if (isPointerUpon)
            isPointerUp = true;

        // check whether button is animated or not
        if (isAnimated)
            animator.SetTrigger("PressedReverse");
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // mouse click
        isPointerUp = false;

        // check whether button in animated or not
        if (isAnimated)
        {
            // begin animation
            animator.SetTrigger("Pressed");
        }
        else
        {
            // run without animation
            isReady = true;
        } 

        // get mouse down position
        downPosition = Input.mousePosition;
    }
    public void OnPointerEnter(PointerEventData eventData) => isPointerUpon = true;
    public void OnPointerExit(PointerEventData eventData) => isPointerUpon = false;

    #endregion

    #region AnimatorUtils

    protected virtual void PressedStart() { }
    protected virtual void UnpressedStart() { }
    protected void Pressed()
    {
        if(btnDownAudioClip != null)
        {
            if(isClickSoundOn)
                SoundManager.instance.PlaySingle(btnDownAudioClip);
        }
        isToggleNeeded = true;
    }
    protected void Unpressed()
    {
        if (btnUpAudioClip != null)
        {
            if(isClickSoundOn)
                SoundManager.instance.PlaySingle(btnUpAudioClip);
        }
    }
    protected void SetPanel()
    {
        isReady = true;
    }

    #endregion
}
