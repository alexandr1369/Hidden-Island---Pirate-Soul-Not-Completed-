using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class BossAttack : MonoBehaviour
{
    public UnityArmatureComponent component; // db animator
    public AudioClip[] dyingSounds; // dying sound
    public ParticleSystem ps; // particle system

    protected bool isDying;

    protected void Start()
    {
        component.AddEventListener(EventObject.FRAME_EVENT, OnFrameEventHandler);
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision) { }
    protected void CommitASuicide()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
    protected void OnFrameEventHandler(string type, EventObject eventObject)
    {
        if (eventObject.name == "DyingEnd")
        {
            // destroy bone object
            CommitASuicide();
        }
    }
}
