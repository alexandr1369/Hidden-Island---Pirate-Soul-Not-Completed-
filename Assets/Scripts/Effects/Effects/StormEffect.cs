using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormEffect : CustomEffect
{
    #region Singleton
    public static StormEffect instance;
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
        effectType = EffectType.Storm;
    }
}
