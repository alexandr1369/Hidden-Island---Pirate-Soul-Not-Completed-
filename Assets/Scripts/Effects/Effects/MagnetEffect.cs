using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetEffect : CustomEffect
{
    #region Singleton
    public static MagnetEffect instance;
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
        effectType = EffectType.Magnet;
    }
}
