using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowdownEffect : CustomEffect
{
    #region Singleton
    public static SlowdownEffect instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    protected override void Start()
    {
        // base init
        base.Start();

        // set effect type
        effectType = EffectType.Slowdown;
    }
    protected override void Update()
    {
        if (isActive)
        {
            // continue
            duration -= Time.deltaTime;

            // check for the end
            if (duration <= 0)
            {
                // turn off active effect wrap(borders) smoothly
                EffectsManager.instance.ToggleActiveWrapEffect(false);

                // disable effect
                isActive = false;

                // hide info panel
                infoPanel.SetActive(false);

                // hide begin magnet effect
                startAnimation.SetActive(false);

                // return to initial values
                ShipController.instance.SetVelocity(ShipController.instance.animVelocity + .5f);
                CoreManager.instance.SetVelocity(CoreManager.instance.animVelocity - .5f);
            } // if

            // check for ending 
            if (!isEnding)
            {
                // start ending animation
                if (duration <= 7f)
                {
                    isEnding = true;
                    mainAnimator.SetBool("Ending", true);
                } // if
            } // if
        } // if is active
    }
    public override void Activate()
    {
        // preventing multiply effect
        if (!isActive)
        {
            // increase ship speed
            ShipController.instance.SetVelocity(ShipController.instance.animVelocity - .5f);
            CoreManager.instance.SetVelocity(CoreManager.instance.animVelocity + .5f);
        }

        // base init
        base.Activate();
    }
}
