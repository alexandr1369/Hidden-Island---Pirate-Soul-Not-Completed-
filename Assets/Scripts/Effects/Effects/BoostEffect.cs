using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// increase speed effect
[System.Serializable]
public class BoostEffect : CustomEffect
{
    #region Singleton
    public static BoostEffect instance;
    protected void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject speedBoostEffectPrefab; // ship image prefab with animation

    private float spawnTime; // spawn time ratio
    private float cooldown; // cooldown between spawning

    protected override void Start()
    {
        // base init
        base.Start();

        // set effect type
        effectType = EffectType.Boost;

        // set spawn time(for spawning sprites)
        spawnTime = .075f;

        // load right sprite for echo effect
        speedBoostEffectPrefab.GetComponent<Image>().sprite = 
            Resources.Load<Sprite>("Textures/Effects/BoostEffect/" + PlayerManager.instance.currentShip.label);
    }
    private new void Update()
    {
        if (isActive)
        {
            // boost effect
            if (ShipController.instance.isMoving)
            {
                // if it's time to spawn a sprite
                if (cooldown <= 0)
                {
                    // spawn another prefab 
                    GameObject echoPartiple = 
                        Instantiate(speedBoostEffectPrefab, ShipController.instance.shipPanel.position, Quaternion.identity);

                    // destroy effect partiple in N sec
                    // (N has to be less than animation time not to be laggy)
                    Destroy(echoPartiple, .55f);

                    // set cooldown (time before doing another thing)
                    cooldown = spawnTime;
                }
                else
                {
                    // decrease cooldown by deltaTime
                    cooldown -= Time.deltaTime;
                } // if else
            } // if

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
                ShipController.instance.SetVelocity(ShipController.instance.animVelocity - .5f);
                CoreManager.instance.SetVelocity(CoreManager.instance.animVelocity + .5f);
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
    // activate effect
    public override void Activate()
    {
        // preventing multiply effect
        if (!isActive)
        {
            // increase ship speed
            ShipController.instance.SetVelocity(ShipController.instance.animVelocity + .5f);
            CoreManager.instance.SetVelocity(CoreManager.instance.animVelocity - .5f);
        }

        // base init
        base.Activate();
    }
}
