using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent of all game effects.
/// </summary>
public class CustomEffect : MonoBehaviour
{
    public bool isActive; // is active now

    public GameObject infoPanel; // panel with info about current effect time left
    public GameObject startAnimation; // start animation obj

    public float durationTime; // duration time

    protected float duration; // duration time

    protected bool isEnding; // ending trigger
    protected Animator mainAnimator; // time elapsed animator

    protected EffectType effectType; // current effect type

    virtual protected void Start()
    {
        // can get from PlayerPrefs(в случае, если можно будет улучшать)
        // set duration time of effect
        durationTime = durationTime <= 0 ? 15f : durationTime;
        duration = durationTime;

        // get time elapsed animator
        mainAnimator = infoPanel.GetComponent<Animator>();
    }
    virtual protected void Update()
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
    virtual public void Activate()
    {
        // activate effect
        isActive = true;
        isEnding = false;

        // set duration
        duration = durationTime;

        // start time elapsing animation
        infoPanel.SetActive(true);
        mainAnimator.Play("EffectValueMoving", 0, 0);
        mainAnimator.SetFloat("Speed", 15f / duration);

        // stop ending animation if it's curently on
        if (mainAnimator.GetBool("Ending"))
        {
            mainAnimator.SetBool("Ending", false);
            mainAnimator.SetTrigger("EndingDenied");
        }

        // show begin magnet effect
        startAnimation.SetActive(true);
        if(startAnimation.GetComponentInChildren<Animator>() != null)
            startAnimation.GetComponentInChildren<Animator>().SetTrigger("Start");

        // turn on active effect wrap
        EffectsManager.instance.ToggleActiveWrapEffect(true, effectType);
    }
}
