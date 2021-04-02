using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class FishingRod : MonoBehaviour
{
    public UnityArmatureComponent fishingRodComponent; // fishing rod animator(db)

    public Animator animator; // animator

    public GameObject pullingBtn; // pulling button gameObject

    public Canvas fishingLineCanvas; // fishing line canvas(order in layer)
    public Canvas floatCanvas; // float canvas(order in layer)

    public void Activate()
    {
        // start listening to events
        fishingRodComponent.AddDBEventListener(EventObject.FRAME_EVENT, OnFrameEventHandler);

        // show fishing rod
        animator.SetTrigger("Appearing");
    }
    protected void BeginDbAnimation(string name)
    {
        // play db animation
        DragonBones.AnimationState lastAnimState = fishingRodComponent.animation.Play(name, 1);
    }
    public void SwapPullingAnimations()
    {
        // get info
        string _hostAnimName = fishingRodComponent.animation.lastAnimationName == "Pulling" ? "PullingReverse" : "Pulling";
        float _pauseTime = fishingRodComponent.animation.lastAnimationState.currentTime,
            _hostAnimDuration = fishingRodComponent.animation.animations[_hostAnimName].duration;

        // play animation
        fishingRodComponent.animation.GotoAndPlayByTime(_hostAnimName, _hostAnimDuration - _pauseTime, 1);
    }
    protected void OnFrameEventHandler(string type, EventObject eventObject)
    {
        switch (eventObject.name)
        {
            case "FishingRodIdleFloatGoingDown":
            {
                // set canvas order in layer to part of fishing rod which are to be hidden 'under the sea'
                fishingLineCanvas.sortingOrder = floatCanvas.sortingOrder = 0;
            } break;
            case "FishingRodIdlePullingButtonAppearing":
            {
                // show pulling buton
                pullingBtn.SetActive(true);
            } break;
            case "FishingRodPullingStart":
            {
                // set animator(db) time scale
                fishingRodComponent.animation.timeScale = .2f;
            } break;
            case "FishingRodPullingCheckPoint":
            case "FishingRodPullingReverseEnd": SwapPullingAnimations(); break;
            case "FishingRodPulledOutStart":
            {
                // start boss dying animation
                Boss _currentBoss = BossManager.instance.currentBoss;
                _currentBoss.animator.enabled = true;
                _currentBoss.animator.SetTrigger("Dying");
            } break;
            case "FishingRodPulledOutGoingUp":
            {
                // set canvas order 'back to default' in layer to part of fishing rod which are to be hidden 'under the sea'
                fishingLineCanvas.sortingOrder = floatCanvas.sortingOrder = 8;
            } break;
            case "FishingRodPulledOutDisappearing": animator.SetTrigger("Disappearing"); break;
        }
    }
}
