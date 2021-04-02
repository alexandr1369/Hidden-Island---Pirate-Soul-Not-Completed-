using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class LifeUnit : MonoBehaviour
{
    public UnityArmatureComponent component; // db live unity

    public Animator animator; // animator

    public ParticleSystem[] effects; // bg particle system

    public void PlayDbAnimation(string name)
    {
        component.animation.Play(name, 1);
    }
    public void DisableParticleSystemEmission()
    {
        foreach(ParticleSystem ps in effects)
        {
            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rateOverTime = 0;
        }
    }
    public void CommitASuicide()
    {
        Destroy(gameObject);
    }
}
