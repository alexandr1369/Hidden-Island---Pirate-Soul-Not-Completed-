using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

/* TODO: Сделать так, чтобы все боссы управлялись одним скриптом,
   у всех было одинаковое кол-во анимаций, но реализованные по разному */

public class Boss : MonoBehaviour
{
    // death toggle(is used to check for boss attacks)
    public bool IsDying
    {
        get { return isDying; }
        set { isDying = value; }
    }
    private bool isDying;

    // activity toggle
    public bool IsActive {
        get { return isActive; }
        protected set { isActive = value; }
    }
    private bool isActive;

    // health
    public float health;
    protected float startHealth;
    protected int beatingStage;

    public Animator animator; // unity animator

    public UnityArmatureComponent bossComponent; // boss animator (db)

    public GameObject bossWrap; // boss wrap panel(hide/show)

    protected int lastAttackIndex; // last attack index(1 or 2)

    protected bool isAttacking; // attacking trigger

    // attacks flags
    protected bool isFirstAttackActive;
    protected bool isSecondAttackActive;

    protected virtual void Start()
    {
        // set health to 100
        if (health < 100)
            health = 100;

        // set start health
        startHealth = health;
    }

    #region boss activation functions

    public void Activate()
    {
        // active event listener
        bossComponent.AddDBEventListener(EventObject.FRAME_EVENT, OnFrameEventHandler);
        bossComponent.AddDBEventListener(EventObject.SOUND_EVENT, OnSoundEventHandler);

        // wait for activation
        StartCoroutine(WaitForEndOfSpawning());
    }
    protected IEnumerator WaitForEndOfSpawning()
    {
        // stop adding cores
        CoreManager.instance.StopAddingCores();

        // waiting
        while (true)
        {
            // TODO:
            // после начала добавления других боссов, которые будут в игре в вертикальном положении,
            // при котором не нужно будет кораблик по центру позиционировать перед началом боя,
            // разделять боссов на 2 основных вида(вертикальные и горизонтальные) и через if переключаться между ними 
            // в скрипте ниже + подправить логику управления корабликом после перехода в бой с боссом в ShipController.cs ===> GetSwipeDirection()

            // if the count of active cores is 0
            if (CoreManager.instance.allCores.Count == 0)
            {
                // toggle ship movement
                if (ShipController.instance.isMovementAllowed)
                    ToggleShipMovement();

                // move ship to the bottom center(3, 3)
                iTween.MoveTo(ShipController.instance.shipPanel.gameObject, iTween.Hash(
                    "position", new Vector3(ShipController.instance.xPosList[2],
                        ShipController.instance.yPosList[2],
                        ShipController.instance.shipPanel.position.z),
                    "time", 1.5f,
                    "easetype", iTween.EaseType.easeInOutQuad,
                    "oncomplete", "BossAppearing",
                    "oncompletetarget", gameObject
                ));

                // change position info
                ShipController.instance.yCurrentPos = 2;
                ShipController.instance.xCurrentPos = 2;

                // break waiting loop
                break;
            }

            // update while every frame
            yield return new WaitForEndOfFrame();
        } // while
    }
    protected virtual void BossAppearing()
    {
        // activate boss
        IsActive = true;

        // start animation chain
        animator.SetTrigger("Appearing");
    }

    #endregion

    #region ship utils

    protected void ToggleShipMovement()
    {
        ShipController.instance.isMovementAllowed = 
            ShipController.instance.isMovementAllowed ? false : true;
    }
    protected void ToggleShipShooting()
    {
        ShipController.instance.isShootingAllowed =
            ShipController.instance.isShootingAllowed ? false : true;
    }

    #endregion

    // boss attack(AI)
    protected void Attack()
    {
        // 0-1 -> attack2 | 2 = attack1
        if (lastAttackIndex == 2)
        {
            animator.SetTrigger("Attack1");
            lastAttackIndex = 1;
        }
        else
        {
            animator.SetTrigger("Attack2");
            lastAttackIndex = 2;
        }

        // set trigger
        isAttacking = true;
    }
    // first attack activation
    protected virtual void ToggleFirstAttack() { }
    // second attack activation
    protected virtual void ToggleSecondAttack() { }

    #region dragon bones utils

    protected void BeginDbAnimation(string name)
    {
        // play db animation
        DragonBones.AnimationState lastAnimState = bossComponent.animation.FadeIn(name, 0.05f, 1);
    }
    protected void BeginFadeInDbAnimation(string name, float fade)
    {
        DragonBones.AnimationState lastAnimState = bossComponent.animation.FadeIn(name, fade, 1);
    }
    protected void BeginDbAnimationRandom(string names)
    {
        // get all names
        string[] _names = names.Split(' ');

        // choose one of them
        string name = _names[Random.Range(0, _names.Length)];

        // play it
        bossComponent.animation.Play(name, 1);
    }

    #endregion

    // getting damage management
    public virtual void GetDamage(float damage) { }

    // dragon bones frame listener
    protected virtual void OnFrameEventHandler(string type, EventObject eventObject) { }
    // dragon bones sound listener
    protected virtual void OnSoundEventHandler(string type, EventObject eventObject) { }
}
