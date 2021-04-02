using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using DragonBones;

public class BossHusky : Boss
{
    // idle animation toggle
    public bool IsIdle
    {
        get { return isIdle; }
        protected set { isIdle = value; }
    }
    private bool isIdle;

    public GameObject bossShadow; // boss shadow wrap
    public Animator bossShadowAnimator; // boss shadow animator

    public GameObject[] firstAttackItems; // 1st attack items prefabs
    public GameObject secondAttackItem; // 2nd attack item prefab

    public Canvas bossCanvas; // boss canvas(order in layer option)

    public FishingRod fishingRod; // fishing rod 

    private float idleBossShadowNormalizedTime; // boss shadow anim time for continuing after hit
    private List<float?> idleContinueTime; // boss anim time for continuing after hit
    private List<float> xPosList; // boss position for X

    protected override void Start()
    {
        base.Start();

        // fill default idles pause time in
        idleContinueTime = new List<float?>();
        idleContinueTime.Add(null);
        idleContinueTime.Add(null);

        // get boss positions on OX
        Vector2 _worldRes = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        float _part = _worldRes.x * 2 / 3;
        xPosList = new List<float>() { -1 * _part, 0, _part };
    }

    #region animations utils

    protected void SetCanvasOrderInLayer(int order) => bossCanvas.sortingOrder = order;
    protected void SetIsIdleState(int state)
    {
        // check for dying
        if (state == 1)
        {
            if (IsDying)
            {
                animator.SetFloat("IdleSpeed", 0);
                bossShadowAnimator.SetFloat("IdleSpeed", 0);
            }
        }

        // toggle idle state
        IsIdle = state == 0 ? false : true;
    }
    protected void SetBossShadowIdleAnimation() => bossShadowAnimator.SetTrigger("Idle");
    protected void SetRandomBossWrapXPosition()
    {
        // get random position on OX(check for dying -> center anim positioning)
        float _randomX = IsDying? 0 : xPosList[Random.Range(0, 3)];

        // set boss position
        transform.position = new Vector3(_randomX, transform.position.y, transform.position.z);

        // set boss shadow position
        Vector3 _bossShadowPosition = bossShadow.transform.position;
        bossShadow.transform.position = new Vector3(_randomX, _bossShadowPosition.y, _bossShadowPosition.z);
    }
    protected void ResetBossWrapXPosition()
    {
        // reset boss position
        transform.position = new Vector3(0, transform.position.y, transform.position.z);

        // reset boss shadow position
        Vector3 _bossShadowPosition = bossShadow.transform.position;
        bossShadow.transform.position = new Vector3(0, _bossShadowPosition.y, _bossShadowPosition.z);
    }
    protected void PlayDyingAnimation()
    {
        if (bossComponent.animation.lastAnimationName != "Dying")
            BeginDbAnimation("Dying");
    }
    protected void HideBossShadow() => bossShadowAnimator.SetTrigger("Dying");

    #endregion

    protected string EditAnimIdByBeatingStage(string animName)
    {
        if (beatingStage <= 2) animName += "1";
        else if (beatingStage == 3) animName += "2";
        else if (beatingStage == 4) animName += "3";

        return animName;
    }
    public override void GetDamage(float damage)
    {
        // get damage
        if (damage > 0)
            health -= damage;

        // hitting
        if(health > 0)
        {
            // get health stage
            float _healthStage = startHealth / 5;

            // check for health check points
            if (health <= _healthStage * 4 && beatingStage == 0)
            {
                // change boss appearance
                bossComponent.armature.GetSlot("EyeScar").displayIndex = 0;

                // increase beating stage
                beatingStage++;
            }
            else if (health <= _healthStage * 3 && beatingStage == 1)
            {
                // change boss appearance
                bossComponent.armature.GetSlot("HuskyMouth").displayIndex = 1;

                // increase beating stage
                beatingStage++;
            }
            else if (health <= _healthStage * 2 && beatingStage == 2)
            {
                // change boss appearance
                bossComponent.armature.GetSlot("HuskyTailTop").displayIndex = 1;

                // increase beating stage
                beatingStage++;
            }
            else if (health <= _healthStage && beatingStage == 3)
            {
                // change boss appearance
                bossComponent.armature.GetSlot("HuskyTailBottom").displayIndex = 1;

                // increase beating stage
                beatingStage++;
            }

            // save current idle animation time when hit
            string _lastAnimationName = bossComponent.animation.lastAnimationName;
            if (_lastAnimationName.StartsWith("Idle"))
            {
                idleContinueTime[int.Parse(_lastAnimationName[4].ToString()) - 1] = bossComponent.animation.lastAnimationState.currentTime;

                // get 'idle' animation duration time and current 'idle' animation time before playing 'hit' animation
                float _currentIdleAnimationTime, _idleAnimationDuration;
                _idleAnimationDuration = bossShadowAnimator.GetCurrentAnimatorStateInfo(0).length;
                _currentIdleAnimationTime = _idleAnimationDuration * bossShadowAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                // calculate normalized time for continue 'idle' animatioafter 'hit' animation before playing 'hit' animation
                idleBossShadowNormalizedTime = _currentIdleAnimationTime / _idleAnimationDuration;

                // play hit boss shadow animation
                bossShadowAnimator.Play(EditAnimIdByBeatingStage("Hit"), 0);
            }

            // play hit animation
            BeginFadeInDbAnimation("Hit", .05f);

            // TODO: add another sound for this boss
            // play getting hit damage
            string _filePath = Random.Range(0, 2) == 0 ? "Sounds/pug_hit1" : "Sounds/pug_hit2";
            AudioClip _clip = Resources.Load<AudioClip>(_filePath);
            SoundManager.instance.PlaySingle(_clip);
        }
        // dying
        else
        {
            // toggle ship shooting(disable)
            ToggleShipShooting();

            // check for boss idle animation position(should be in the middle, if not => begin idle anim in the middle)
            if (bossWrap.transform.position.x != 0)
            {
                // hide current 'idle' animation and wait for another
                animator.Play("Idle", 0, .875f);

                // do the same for boss shadow 'idle' animation
                bossShadowAnimator.Play("Idle", 1, .875f);
            }
            else
            {
                // stop 'idle' animation
                animator.SetFloat("IdleSpeed", 0);

                // stop boss shadow idle animation
                bossShadowAnimator.SetFloat("IdleSpeed", 0);
            }

            // toggle dying flag
            IsDying = true;

            // play crying sound(TODO: make another sound for this boss)
            AudioClip _clip = Resources.Load<AudioClip>("Sounds/pug_crying");
            SoundManager.instance.PlaySingle(_clip);

            // show fishing rod somewhere and begin wating for player's clicking on it
            fishingRod.gameObject.SetActive(true);
            fishingRod.Activate();
        }
    }

    protected override void OnFrameEventHandler(string type, EventObject eventObject)
    {
        switch (eventObject.name)
        {
            case "HuskyIdle1Start": bossShadowAnimator.Play(EditAnimIdByBeatingStage("Idle1_"), 0, 0); break;
            case "HuskyIdle2Start": bossShadowAnimator.Play(EditAnimIdByBeatingStage("Idle2_"), 0, 0); break;
            case "HuskyIdleEnd":
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dying"))
                    BeginDbAnimation("Dying");
                else
                    BeginDbAnimationRandom("Idle1 Idle1 Idle2");
            } break;
            case "HuskyHitStart": if(beatingStage > 0) bossComponent.armature.GetSlot("EyeHoverScar").displayIndex = 0; break;
            case "HuskyHitEnd":
            {
                // toggle scar(it won't be seen but I leave it here)
                if (beatingStage > 0) bossComponent.armature.GetSlot("EyeHoverScar").displayIndex = -1;

                // continue idle animation
                int _idleNum = idleContinueTime[0] != null ? 1 : 2;
                bossComponent.animation.GotoAndPlayByTime("Idle" + _idleNum, (float)idleContinueTime[_idleNum - 1], 1);
                bossShadowAnimator.Play(EditAnimIdByBeatingStage("Idle" + _idleNum + "_"), 0, idleBossShadowNormalizedTime);

                // clear pauses's times
                idleContinueTime[0] = idleContinueTime[1] = null;
            } break;
            case "HuskyDyingEnd": StateManager.instance.Victory(); break;
        }
    }
}