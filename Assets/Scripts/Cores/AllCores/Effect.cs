using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class Effect : Core
{
    public ParticleSystem bgEffect; // back ground effect(ps)
    public EffectType effectType; // effect type

    protected override void Start()
    {
        // base init
        base.Start();

        // turn on background effect
        bgEffect.Play();
    }

    // begin dragon bones animation
    public override void BeginDbAnimation(string name)
    {
        if (name == "Dying" && isKilled)
        {
            // turn off effects(magnet & bg) smoothly
            DisableAllEffects();

            // turn current effect on
            EffectsManager.instance.ActivateEffect(effectType);

            // play effect sound
            string _path = "Sounds/";
            switch (effectType)
            {
                case EffectType.Boost: _path += "boost_effect"; break;
                case EffectType.Magnet: _path += "magnet_effect"; break;
                case EffectType.Storm: _path += "storm_effect"; break;
                case EffectType.Slowdown: _path += "slowdown_effect"; break;
                // add more...
            }

            AudioClip clip = Resources.Load<AudioClip>(_path);
            SoundManager.instance.PlaySingle(clip, true, 2.5f);
        }
        base.BeginDbAnimation(name);
    }
    // disable all effects
    protected override void DisableAllEffects()
    {
        base.DisableAllEffects();
        bgEffect.Stop();
    }
}
