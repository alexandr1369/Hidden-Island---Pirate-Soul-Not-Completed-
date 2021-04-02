using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;
using EZCameraShake;

// class parent of all moving stuff(all beasts, effects)
public class Core : MonoBehaviour
{
    public AudioClip hit1; // hit sound1
    public AudioClip hit2; // hit sound2

    public UnityArmatureComponent component; // DB core

    public ParticleSystem magnetEffect; // magnet effect
    public ParticleSystem slowdownEffect; // slowdown effect
    public UnityArmatureComponent stormEffect; // DB storm(hiding cloud) effect 

    public CanvasGroup canvasGroupCore; // canvas group of core(alpha)

    public Animator animator; // animation's controller

    protected bool isKilled; // killing trigger
    protected bool isBeingMagnetized; // magnetizing trigger
    protected bool isBeingUnderStorm; // storming trigger

    protected virtual void Start()
    {
        Init();
    }
    protected void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.name == "ShipPanel" && !isKilled)
        {
            // set kill trigger
            isKilled = true;

            // show core if he's under storm
            // no need to turn isBeingUnderStorm trigger(the same is with magnet trigger)
            if (canvasGroupCore.alpha < 1)
                canvasGroupCore.alpha = 1;

            // fix position to current one
            transform.parent.position = transform.position;

            // turn hit sound on
            SoundManager.instance.RandomizeSfx(hit1, hit2);

            // set animations
            animator.SetTrigger("ExitLevitating");
            animator.SetTrigger("Dying");
        } // if
    }

    #region Utils
    protected virtual void Update()
    {
        // animated core position X(child)
        float childAnchoredPosX = GetComponent<RectTransform>().anchoredPosition.x;

        // effect enabling
        if (MagnetEffect.instance.isActive || magnetEffect.isPlaying)
        {
            // show magnet effect
            if (childAnchoredPosX <= -Screen.width / 6 && !magnetEffect.isPlaying && !isKilled && !isBeingUnderStorm)
                ToggleEffect(EffectType.Magnet, true);

            // start magnetizing
            if (childAnchoredPosX <= -Screen.width / 4 && !isBeingMagnetized && !isKilled)
            {
                ToggleEffect(EffectType.Magnet, true);

                // start magnetizing
                StartCoroutine(MagnetizeToShip());

                // toggle magnet effect trigger
                isBeingMagnetized = true;
            }
        }
        if (SlowdownEffect.instance.isActive)
        {
            if(!slowdownEffect.isPlaying && !isKilled && !isBeingUnderStorm)
                ToggleEffect(EffectType.Slowdown, true);
        }
        if (StormEffect.instance.isActive)
        {
            if (childAnchoredPosX <= Screen.width / 3 && !isBeingUnderStorm && !isKilled)
            {
                // toggle storm effect trigger
                isBeingUnderStorm = true;

                // toggle effects
                DisableAllEffects();

                // play hiding cloud animation
                stormEffect.animation.Play("Idle", 1);
            } 
        } 

        // effects diasbling 
        if (!SlowdownEffect.instance.isActive && slowdownEffect.isPlaying)
        {
            ToggleEffect(EffectType.Slowdown, false);
        }
        if (!StormEffect.instance.isActive && isBeingUnderStorm)
        {
            // toggle storm effect trigger
            isBeingUnderStorm = false;

            // toggle effects
            DisableAllEffects();

            // play hiding cloud animation
            stormEffect.animation.Play("Idle", 1);
        }
    }
    // set init settings
    protected void Init()
    {
        // get animator's component
        animator = GetComponent<Animator>();

        /* начальная анимация 
           не получилось вставить component.animation idle в начало animator idle через событие
           происходит какой-то конфликт при вызове второго Мопса, как будто component переопределяется
           буду вызывать отдельно тогда, не хочу разбираться в китайской фигне(DB) */

        // set start animations
        // animator's Idle start with animator loading(no need for setting)
        component.animation.Play("Idle");
        animator.SetTrigger("Levitating");

        // set start animation speed
        // setting in mob controller manager is a fault(works not correctly)
        ChangeAnimVelocity(CoreManager.instance.animVelocity);

        // ???
        component.CloseCombineMeshs();

        // add db listener for frame event on storm effect(hiding cloud)
        stormEffect.AddDBEventListener(EventObject.FRAME_EVENT, OnFrameEventHandler);
    }
    // изменение скорости движения и анимации животного
    public void ChangeAnimVelocity(float velocity)
    {
        component.animation.timeScale = animator.speed = velocity;
    }
    // begin dragon bones animation
    public virtual void BeginDbAnimation(string name)
    {
        component.animation.Play(name, 1);
        //component.animation.FadeIn(name, .2f, 1);
    }
    // toggle magnet fx
    public void ToggleEffect(EffectType effectType, bool state)
    {
        if (state)
        {
            if (effectType == EffectType.Magnet)
                magnetEffect.Play();
            else if (effectType == EffectType.Slowdown)
                slowdownEffect.Play();
        }
        else
        {
            if (effectType == EffectType.Magnet)
                magnetEffect.Stop();
            else if (effectType == EffectType.Slowdown)
                slowdownEffect.Stop();
        }
    }
    // disable all effects
    protected virtual void DisableAllEffects()
    {
        magnetEffect.Stop();
        slowdownEffect.Stop();
    }
    // begin magnetizing till die
    protected IEnumerator MagnetizeToShip()
    {
        while (!isKilled)
        {
            // parent rect transform for changing Y position
            RectTransform parentRectTransform = transform.parent.GetComponent<RectTransform>();

            // stop levitating
            if (!animator.GetBool("StopLevitating"))
                animator.SetTrigger("StopLevitating");

            // get ship Y anchored position 
            float shipAnchoredPosY = ShipController.instance.shipPanel.GetComponent<RectTransform>().anchoredPosition.y;

            // get mob Y anched position
            float parentAnchoredPosY = parentRectTransform.anchoredPosition.y;
            parentAnchoredPosY +=
                Time.deltaTime * (parentAnchoredPosY - shipAnchoredPosY < 0 ? 1 : -1) * 500 * CoreManager.instance.animVelocity;

            // *magnetizing*
            if (!IsApproximatelyEqual(shipAnchoredPosY, parentAnchoredPosY, 20))
            {
                parentRectTransform.anchoredPosition = new Vector2(parentRectTransform.anchoredPosition.x, parentAnchoredPosY);
            } // If

            // make invoking for every frame
            yield return null;
        } // while

        // reset trigger(but no need)
        isBeingMagnetized = false;
    }
    // destroy an object
    protected virtual void CommitASuicide(int type)
    {
        // remove core from list
        CoreManager.instance.RemoveCore(this);

        // destroy object
        Destroy(transform.parent.gameObject);
    }
    // check for approximately equaling
    protected bool IsApproximatelyEqual(float num1, float num2, float imprecision)
    {
        return Mathf.Abs(num1 - num2) <= imprecision;
    }
    protected virtual void OnFrameEventHandler(string type, EventObject eventObject)
    {
        switch (eventObject.name)
        {
            case "HidingCloudIdleHiding": {
                print("IdleHiding " + isBeingUnderStorm);
                // animated hiding core
                if (isBeingUnderStorm && !isKilled)
                {
                    iTween.ValueTo(gameObject, iTween.Hash(
                        "from", 1f,
                        "to", 0,
                        "time", .25f,
                        "easetype", iTween.EaseType.easeOutExpo,
                        "onupdate", "SetCoreAlpha"
                    ));
                }
                // animated showing core
                else
                {
                    print("turning alpha back!!!");
                    iTween.ValueTo(gameObject, iTween.Hash(
                        "from", 0,
                        "to", 1f,
                        "time", .25f,
                        "easetype", iTween.EaseType.easeOutExpo,
                        "onupdate", "SetCoreAlpha"
                    ));
                }
            } break;
        }
    }
    protected void SetCoreAlpha(float alpha) => canvasGroupCore.alpha = alpha;
    #endregion
}

