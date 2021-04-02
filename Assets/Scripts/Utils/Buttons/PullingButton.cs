using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PullingButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool isActive; // activity toggle

    public Sprite[] defaultSprites; // default icons for 'Idle' animations
    public Sprite[] pressedSprites; // press icons

    public Button btn; // button sript(for sprite swapping)
    public Animator animator; // animator

    public FishingRod fishingRod; // fishing rod animator (db)

    // holding button utils
    private bool holdingBtnToggle;
    private float holdingBtnTimer;

    public void ToggleActivity() => isActive = !isActive;
    private void Update()
    {
        if (holdingBtnToggle)
        {
            holdingBtnTimer += Time.deltaTime;
            if (holdingBtnTimer >= .2f)
                animator.SetFloat("Speed", .33f);
        }

    }
    protected void SetIconsAccordingToIdleAnimation(string animName)
    {
        // get id of sprites according to anim name
        int _id = animName == "Idle1" ? 0 : 1;

        // change current sprite
        btn.image.sprite = defaultSprites[_id];

        // set pressed sprite
        SpriteState _spriteState = new SpriteState();
        _spriteState.pressedSprite = pressedSprites[_id];
        btn.spriteState = _spriteState;
    }

    #region OnPointerUtils

    public void OnPointerUp(PointerEventData eventData)
    {
        // continue animation
        animator.SetFloat("Speed", 1f);

        // demo
        holdingBtnToggle = false;
        holdingBtnTimer = 0;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isActive) return;

        // start waiting for holding
        holdingBtnToggle = true;

        // add some particle effects of button click
        // ...

        // perform the fishing
        // check for right fishing rod moving
        string _animName = fishingRod.fishingRodComponent.animation.lastAnimationName;
        if(_animName == "PullingReverse")
            fishingRod.SwapPullingAnimations();

        // add +1/10 of the whole duration to current pulling animation time
        float _currentTime, _duration, _partInSeconds, _time;
        _currentTime = fishingRod.fishingRodComponent.animation.lastAnimationState.currentTime;
        _duration = fishingRod.fishingRodComponent.animation.lastAnimationState.totalTime;
        _partInSeconds = _duration / 30;
        _time = _currentTime + _partInSeconds;

        // check for ending of pulling
        if(_time >= _duration)
        {
            // set time scale to 1
            fishingRod.fishingRodComponent.animation.timeScale = 1f;

            // play 'pulled out' animation
            fishingRod.fishingRodComponent.animation.Play("PulledOut", 1);

            // deactivate and hide button
            animator.SetTrigger("Disappearing");
        }
        // continue pulling
        else
        {
            // set
            fishingRod.fishingRodComponent.animation.GotoAndPlayByTime("Pulling", _time, 1);

            // continue reverse pulling
            fishingRod.SwapPullingAnimations();
        }
    }

    #endregion
}
