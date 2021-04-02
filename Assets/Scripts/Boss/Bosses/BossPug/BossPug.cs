using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class BossPug : Boss
{
    public GameObject firstAttackItem; // 1st attack item prefab
    public GameObject[] secondAttackItems; // 2st attack items prefabs

    public GameObject bossDyingEffectPrefab; // dying effect prefab

    public float timeBeforeAttack; // time before attack
    protected float attackCooldown; // attack cooldown

    protected float firstAttackPauseTime; // if boss is hit why attacking -> stop and save time, then continue

    protected override void Start()
    {
        base.Start();

        // set start attack cooldown
        attackCooldown = 6.5f;
    }
    protected void Update()
    {
        if (isAttacking || !IsActive || IsDying) return;

        // is ready for attack
        if (attackCooldown <= 0)
        {
            Attack();
        }
        // continue waiting
        else
        {
            attackCooldown -= Time.deltaTime;
        }
    }
    protected override void BossAppearing()
    {
        base.BossAppearing();

        // activate sound
        AudioClip _clip = Resources.Load<AudioClip>("Sounds/pug_laugh");
        SoundManager.instance.PlaySingle(_clip);
    }
    protected override void ToggleFirstAttack()
    {
        print("Toggling First Attack!");
        if (!isFirstAttackActive)
        {
            // activate attack
            isFirstAttackActive = true;

            // start spawning attack items
            StartCoroutine(StartSpawningFirstAttackItems(1.25f));
        }
        else
        {
            // deactivate attack
            isAttacking = false;
            isFirstAttackActive = false;

            // reset attack cooldown
            attackCooldown = timeBeforeAttack;
        }
    }
    protected override void ToggleSecondAttack()
    {
        print("Toggling Second Attack! " + isSecondAttackActive);
        if (!isSecondAttackActive)
        {
            // activate attack
            isSecondAttackActive = true;
        }
        else
        {
            // deactivate attack
            isAttacking = false;
            isSecondAttackActive = false;
            bossComponent.animation.timeScale = 1;
            BeginDbAnimationRandom("Idle1 Idle1 Idle2");

            // reset attack cooldown
            attackCooldown = timeBeforeAttack;
        }
    }

    #region attacks utility functions

    // 1st attack spawning items animation
    private IEnumerator StartSpawningFirstAttackItems(float cooldown)
    {
        // start dragon bones animation
        BeginDbAnimation("Attack1");

        // spawning items
        while (isFirstAttackActive)
        {
            // get all available positions
            List<float> xPosList = ShipController.instance.xPosList;

            // get 2 free positions(bone wont spawn a bone on this position)
            List<float> xFreePos = new List<float>
            {
                Random.Range(0, ShipController.instance.xPosList.Count),
                Random.Range(0, ShipController.instance.xPosList.Count)
            };

            // мб костыль, но это первое что пришло в голову, как получить 2 чисто рандомно из того же порядка, чтобы 2 != 1
            while (xFreePos[0] == xFreePos[1])
                xFreePos[1] = Random.Range(0, ShipController.instance.xPosList.Count);

            // spawn bone on selected position
            for (int i = 0; i < xPosList.Count; i++)
                if (!xFreePos.Contains(i))
                    Instantiate(firstAttackItem, new Vector3(xPosList[i], 0, 100), Quaternion.identity, GameObject.Find("BackUI").transform);

            // set spawning cooldown
            yield return new WaitForSeconds(cooldown);
        } // while
    }
    // spawn 2nd attack item(steak or chicken leg)
    // (float parameter in a space after anim name will scale db anim speed)
    protected void SpawnSecondAttack(string type)
    {
        if (isSecondAttackActive)
        {
            // scale animation speed(if exists)
            string[] @params = type.Split(' ');
            type = @params[0];
            float timeScale = 1;
            if (@params.Length == 2)
            {
                if (@params[1] == "1.5")
                    timeScale = 1.5f;
                else if (@params[1] == "2")
                    timeScale = 2f;

                bossComponent.animation.timeScale = timeScale;
            } // if

            // play boss second db attack animation
            BeginFadeInDbAnimation("Attack2", .02f);

            // play vomiting sound
            string _clipPath = "Sounds/" + (Random.Range(0, 2) == 0 ? "vomiting1" : "vomiting2");
            AudioClip _clip = Resources.Load<AudioClip>(_clipPath);
            SoundManager.instance.PlaySingle(_clip);

            // spawn an attack item
            StartCoroutine(SpawnShootingItem(type, .5f, timeScale));
        } // if   
    }
    private IEnumerator SpawnShootingItem(string type, float delay, float timeScale)
    {
        // wait before spawning
        yield return new WaitForSeconds(delay / timeScale);

        // spawn item
        type = type.ToLower();
        if (type == "steak")
        {
            // spawn steak
            GameObject steak = Instantiate(secondAttackItems[0], gameObject.transform.position,
                Quaternion.identity, GameObject.Find("BackUI").transform);

            // aim and shoot it
            Vector3 shipPos = ShipController.instance.shipPanel.position;
            Vector3 shootPos = new Vector3(shipPos.x * 2, shipPos.y * 2, shipPos.z * 2);
            steak.GetComponentInChildren<HomingAttack>().AimAndShoot(shootPos, timeScale);
        }
        else if (type == "chickenleg")
        {
            // spawn steak
            GameObject chickeLeg = Instantiate(secondAttackItems[1], gameObject.transform.position,
                Quaternion.identity, GameObject.Find("BackUI").transform);

            // aim and shoot it
            Vector3 shipPos = ShipController.instance.shipPanel.position;
            Vector3 shootPos = new Vector3(shipPos.x * 2, shipPos.y * 2, shipPos.z * 2);
            chickeLeg.GetComponentInChildren<HomingAttack>().AimAndShoot(shootPos, timeScale);
        }
    }

    #endregion

    // levitation animation toggler
    protected void ToggleLevitation()
    {
        bool state;
        if (animator.GetBool("Levitating"))
            state = false;
        else
            state = true;

        animator.SetBool("Levitating", state);
    }

    #region boss dying animation

    protected void StopDyingAnimation() => IsDying = false;
    protected IEnumerator PlayDyingAnimation()
    {
        // do animation while is dying(for N seconds)
        while (IsDying)
        {
            // get boss transform
            UnityEngine.Transform bossTransform = GameObject.Find("BossesPanel/BossPugPanel").transform;

            // spawn boss dying effect randomly
            // get spawn position randomly
            Vector3 spawnPosition =
                new Vector3(bossTransform.position.x + Random.Range(-1.5f, 1.5f),
                bossTransform.position.y + Random.Range(-1.5f, 1.5f),
                bossTransform.position.z);

            // spawn effect randomly
            GameObject dyingEffect = Instantiate(bossDyingEffectPrefab, spawnPosition, Quaternion.identity, bossTransform);

            // activate all particles
            foreach (ParticleSystem ps in dyingEffect.GetComponentsInChildren<ParticleSystem>())
                ps.Play();

            // wait for .2f second before playing db animation
            yield return new WaitForSeconds(.3f);
            dyingEffect.GetComponentInChildren<UnityArmatureComponent>().animation.Play("Idle", 1);

            // wait for .8f second before spawning another dying effect
            yield return new WaitForSeconds(.3f);
            Destroy(dyingEffect, 1f);
        }
    }

    #endregion

    public override void GetDamage(float damage)
    {
        // get damage
        if (damage > 0)
            health -= damage;

        // hitting
        if (health > 0)
        {
            // get health stage
            float _healthStage = startHealth / 5;

            // check for health check points
            if (health <= _healthStage * 4 && beatingStage == 0)
            {
                // change boss appearance
                bossComponent.armature.GetSlot("ClosedEyeHoverLeft").displayIndex = -1;
                bossComponent.armature.GetSlot("PugEyeBrokenHoverLeft").displayIndex = 0;

                // increase beating stage
                beatingStage++;
            }
            else if (health <= _healthStage * 3 && beatingStage == 1)
            {
                // change boss appearance
                bossComponent.armature.GetSlot("ClosedEyeHoverRight").displayIndex = -1;
                bossComponent.armature.GetSlot("PugEyeBrokenHoverRight").displayIndex = 0;

                // increase beating stage
                beatingStage++;
            }
            else if (health <= _healthStage * 2 && beatingStage == 2)
            {
                // change boss appearance
                bossComponent.armature.GetSlot("PugBandage").displayIndex = 0;

                // increase beating stage
                beatingStage++;
            }
            else if (health <= _healthStage && beatingStage == 3)
            {
                // change boss appearance
                bossComponent.armature.GetSlot("PugEarLeft").displayIndex = 1;

                // increase beating stage
                beatingStage++;
            } 

            // save current first attack animation time when hit
            if (isFirstAttackActive)
                firstAttackPauseTime = bossComponent.animation.lastAnimationState.currentTime +
                    bossComponent.animation.animations["Hit"].duration;

            // play hit animation
            BeginFadeInDbAnimation("Hit", .05f);

            // play getting hit damage
            string _filePath = Random.Range(0, 2) == 0 ? "Sounds/pug_hit1" : "Sounds/pug_hit2";
            AudioClip _clip = Resources.Load<AudioClip>(_filePath);
            SoundManager.instance.PlaySingle(_clip);
        }
        // dying
        else
        {
            // TODO: check if boss is killed while attacking
            // -> disable attacking
            if (isFirstAttackActive)
            {
                // disable attack
                isFirstAttackActive = false;

                // disable animator
                animator.enabled = false;
            }
            else if (isSecondAttackActive)
            {
                // disable attack
                isSecondAttackActive = false;

                // disable animator
                animator.enabled = false;
            }

            // toggle ship shooting(disable)
            ToggleShipShooting();

            // toggle dying flag
            IsDying = true;

            // play dying animation
            BeginDbAnimation("Dying");

            // turn dying animation on(effects)
            StartCoroutine(PlayDyingAnimation());

            // play crying sound
            AudioClip _clip = Resources.Load<AudioClip>("Sounds/pug_crying");
            SoundManager.instance.PlaySingle(_clip);

            // turn dying animation off in 3.5 secs
            Invoke("StopDyingAnimation", 3.5f);
        }
    }

    protected override void OnFrameEventHandler(string type, EventObject eventObject)
    {
        switch (eventObject.name)
        {
            case "PugHitStart": { if (isFirstAttackActive) bossComponent.animation.timeScale = 1; } break;
            case "PugHitEnd":
            {
                if (isFirstAttackActive)
                    bossComponent.animation.GotoAndPlayByTime("Attack1", firstAttackPauseTime, 1);
                else 
                    BeginDbAnimationRandom("Idle1 Idle1 Idle2");
            } break;
            case "PugIdleEnd":
            case "PugAttack1End": BeginDbAnimationRandom("Idle1 Idle1 Idle2"); break;
            case "PugAttack2End":
            {
                BeginDbAnimationRandom("Idle1 Idle1 Idle2");
                bossComponent.animation.timeScale = 1;
            } break;
            case "PugDyingFalling":
            {
                // disable animator(frezee current position)
                animator.enabled = false;

                // animate boss falling manually
                GameObject bossMainWrap = GameObject.Find("BossesPanel/BossPugPanel");
                print(bossMainWrap.name);
                iTween.MoveTo(bossMainWrap, iTween.Hash(
                    "y", bossMainWrap.transform.position.y - 12,
                    "time", 2f,
                    "easetype", iTween.EaseType.easeOutExpo
                ));
            } break;
            case "PugDyingEnd": GameManager.instance.PlayRoundFinishingAnimation(); break;
        }
    }
    protected override void OnSoundEventHandler(string type, EventObject eventObject)
    {
        switch (eventObject.name)
        {
            case "PugAttack1Start":
            {
                AudioClip _clip = Resources.Load<AudioClip>("Sounds/attack1");
                SoundManager.instance.PlaySingle(_clip);
            } break;
            case "PugDyingFalling":
            {
                AudioClip _clip = Resources.Load<AudioClip>("Sounds/fall2");
                SoundManager.instance.PlaySingle(_clip);
            } break;
            case "PugDyingEnd":
            {
                AudioClip _clip = Resources.Load<AudioClip>("Sounds/fart1");
                SoundManager.instance.PlaySingle(_clip);
            } break;
        }
    }
}