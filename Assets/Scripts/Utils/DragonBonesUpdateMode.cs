using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class DragonBonesUpdateMode : MonoBehaviour
{
    public UnityArmatureComponent component; // db animator

    // use only when it's smth like boss attack item
    // when player is loosing after boss attack item damage
    // game sets time scale to 0 and db animation freezes
    // to prevent this and calculate automatically without unfreezing back
    // set this field to true
    public bool isAutoChecked; // begins when time.timeScale == 0

    public bool isInUnscaledMode; // check for unscaled mode
    private float lastInterval; // last interval of real time

    public void Update()
    {
        // auto checking for unscaled mode
        if (isAutoChecked && !isInUnscaledMode && Time.timeScale == 0)
            isInUnscaledMode = true;

        if (!isInUnscaledMode)
            // get real time in this frame
            lastInterval = Time.realtimeSinceStartup;
        else
        {
            // send its own delta time
            float deltaTime = Time.realtimeSinceStartup - lastInterval;
            component.armature.AdvanceTime(deltaTime);

            lastInterval = Time.realtimeSinceStartup;
        }
    }
    public void ToggleUpdateMode()
    {
        if (isInUnscaledMode)
            isInUnscaledMode = false;
        else
            isInUnscaledMode = true;
    }
}


