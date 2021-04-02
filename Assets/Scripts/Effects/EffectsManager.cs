using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    #region Singleton
    public static EffectsManager instance;
    void Awake()
    {
        instance = this;
    }
    #endregion

    public BoostEffect speedBoostEffect;
    public MagnetEffect magnetEffect;
    public SlowdownEffect slowdownEffect;
    public StormEffect stormEffect;

    private void Start()
    {
        speedBoostEffect = BoostEffect.instance;
        magnetEffect = MagnetEffect.instance;
        slowdownEffect = SlowdownEffect.instance;
        stormEffect = StormEffect.instance;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            speedBoostEffect.Activate();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            magnetEffect.Activate();
        if (Input.GetKeyDown(KeyCode.Alpha3))
            slowdownEffect.Activate();
        if (Input.GetKeyDown(KeyCode.Alpha4))
            stormEffect.Activate();

        if (Input.GetKeyDown(KeyCode.Alpha5))
            stormEffect.isActive = false;
    }
    // activate effect
    public void ActivateEffect(EffectType type)
    {
        switch (type)
        {
            case EffectType.Boost: speedBoostEffect.Activate(); break;
            case EffectType.Magnet: magnetEffect.Activate(); break;
            case EffectType.Slowdown: slowdownEffect.Activate(); break;
            case EffectType.Storm: stormEffect.Activate(); break;
        }
    }
    // get gradient according to effect type
    public Gradient GetEffectColor(EffectType effectType)
    {
        Gradient gradient = new Gradient();
        switch (effectType)
        {
            case EffectType.Boost:
            {
                gradient.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(Color.white, 0),
                    new GradientColorKey(new Color32(250, 91, 235, 255), .33f),
                    new GradientColorKey(new Color32(78, 212, 73, 255), .66f),
                    new GradientColorKey(new Color32(213, 245, 10, 255), 1f)
                };
                gradient.alphaKeys = new GradientAlphaKey[]
                {
                new GradientAlphaKey(25f/255f, 0),
                new GradientAlphaKey(25f/255f, 1)
                };
            } break;
            case EffectType.Magnet:
            {
                gradient.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(Color.white, 0),
                    new GradientColorKey(new Color32(87, 154, 251, 255), .5f),
                    new GradientColorKey(new Color32(231, 44, 29, 255), 1f)
                };
                gradient.alphaKeys = new GradientAlphaKey[]
                {
                new GradientAlphaKey(25f/255f, 0),
                new GradientAlphaKey(25f/255f, 1)
                };
            } break;
            case EffectType.Slowdown:
            {
                gradient.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(new Color32(165, 165, 165, 255), 0),
                    new GradientColorKey(new Color32(186, 54, 64, 255), .5f),
                    new GradientColorKey(new Color32(96, 0, 8, 255), 1f),
                };
                gradient.alphaKeys = new GradientAlphaKey[]
                {
                new GradientAlphaKey(25f/255f, 0),
                new GradientAlphaKey(25f/255f, 1)
                };
            } break;
            case EffectType.Storm:
            {
                gradient.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(new Color32(200, 200, 200, 255), 0.25f),
                    new GradientColorKey(new Color32(128, 122, 41, 255), .5f),
                    new GradientColorKey(new Color32(50, 50, 50, 255), .75f),
                };
                gradient.alphaKeys = new GradientAlphaKey[]
                {
                new GradientAlphaKey(25f/255f, 0),
                new GradientAlphaKey(25f/255f, 1)
                };
            } break;
        }

        return gradient;
    }
    // toggle active wrap effect
    public void ToggleActiveWrapEffect(bool state, EffectType effectType = EffectType.Boost)
    {
        // get active effect wrap
        GameObject activeEffectWrap = GameObject.Find("ActiveEffectWrap");

        // check all part of active wrap effect
        foreach (ParticleSystem host in activeEffectWrap.GetComponentsInChildren<ParticleSystem>())
        {
            ParticleSystem.EmissionModule emissionModule = host.emission;
            if (state)
            {
                // set start color
                ParticleSystem.MainModule mainModule = host.main;
                mainModule.startColor = GetEffectColor(effectType);

                // set particles count
                emissionModule.rateOverTime = 50;
            }
            else
            {
                // set particles count
                if(!AreActiveMoreThanTwoEffect())
                    emissionModule.rateOverTime = 0;
            } // if else
        } // foreach
    }

    // are there any more than 2 effects
    private bool AreActiveMoreThanTwoEffect()
    {
        // add more in future...
        int _counter = 0;
        if (speedBoostEffect.isActive) _counter++;
        if (magnetEffect.isActive) _counter++;
        if (slowdownEffect.isActive) _counter++;
        if (stormEffect.isActive) _counter++;

        return _counter >= 2 ? true : false;
    }
}
public enum EffectType
{
    Boost,   // ускорение(b)
    Magnet,  // магнит(b)
    Slowdown, // замедление(db)
    Storm,   // шторм(db)
    //LifeUnit, // жизнь(b)
    //Chest, // сундук(b)
    //Tnt // Tnt(db)
}
