using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using EZCameraShake;

public class Chest : Effect
{
    public ParticleSystem coinsPs;
    public override void BeginDbAnimation(string name)
    {
        if (name == "Dying" && isKilled)
        {
            // camera slow motion
            SetTimeScale(.1f);

             iTween.ValueTo(gameObject, iTween.Hash(
                "from", .1,
                "to", 1,
                "time", .3f,
                "delay", .2f,
                "easetype", iTween.EaseType.easeOutExpo,
                "onupdate", "SetTimeScale"
            ));

            // play round finishing animation
            GameManager.instance.Invoke("PlayRoundFinishingAnimation", .5f);
        }
        base.BeginDbAnimation(name);
    }

    protected override void Start()
    {
        base.Start();

        // add db listener for frame event on db component
        component.AddDBEventListener(EventObject.FRAME_EVENT, OnFrameEventHandler);
    }
    protected override void Update()
    {
        base.Update();

        /// chest always has to magnetize
        // animated core position X(child)
        float childAnchoredPosX = GetComponent<RectTransform>().anchoredPosition.x;

        // show magnet effect
        if (childAnchoredPosX <= -Screen.width / 6 && !magnetEffect.isPlaying && !isKilled && !isBeingUnderStorm)
            ToggleEffect(EffectType.Magnet, true);

        // start magnetizing
        if (childAnchoredPosX <= -Screen.width / 4 && !isBeingMagnetized && !isKilled)
        {
            // start magnetizing
            StartCoroutine(MagnetizeToShip());

            // toggle magnet effect trigger
            isBeingMagnetized = true;
        }
    }
    protected override void OnFrameEventHandler(string type, EventObject eventObject)
    {
        base.OnFrameEventHandler(type, eventObject);

        switch (eventObject.name)
        {
            case "ChestDyingStart":
            {
                component.animation.timeScale = animator.speed = 1f;
            } break;
            case "ChestDyingMoneyJumping":
            {
                // start coin flow
                coinsPs.Play();

                // smooth stop coin flow
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", 5,
                    "to", 0,
                    "time", .8f,
                    "delay", 1f,
                    "easetype", iTween.EaseType.easeInOutExpo,
                    "onupdate", "SetCoinsRateOverTime"
                ));

                // smooth chest hiding
                iTween.ValueTo(gameObject, iTween.Hash(
                    "from", 1,
                    "to", 0,
                    "time", .8f,
                    "delay", 1f,
                    "easetype", iTween.EaseType.easeInOutExpo,
                    "onupdate", "SetCoreAlpha"
                ));
            } break;
            case "ChestDyingMoneyCameraShaking":
            {
                // shake the camera
                CameraShaker.Instance.ShakeOnce(.75f * ShipController.instance.animVelocity, 2f, .75f, 1f);
            } break;
        } // switch
    }

    // set simulation speed
    private void SetCoinsRateOverTime(float value)
    {
        ParticleSystem.EmissionModule emission = coinsPs.emission;
        emission.rateOverTime = value;
    }
    // set time scale
    private void SetTimeScale(float value) => Time.timeScale = value;
}
