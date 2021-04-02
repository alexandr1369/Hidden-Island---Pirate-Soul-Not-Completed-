using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class Animal : Core
{
    public MobType mobType; // rank of mob 0-2

    protected override void Start()
    {
        // load animal's textures according to the current map
        LoadAnimal();

        // base init
        base.Start();

        // set mob's hat
        SetMobRank();
    }
    // begin dragon bones animation
    public override void BeginDbAnimation(string name)
    {
        if (name == "Dying" && isKilled)
        {
            // Kill core
            ScoreManager.instance.KillCore(mobType);
        }
        base.BeginDbAnimation(name);
    }
    protected override void CommitASuicide(int type)
    {
        // if mob has been missed - savedMob++
        if (type == 0)
        {
            ScoreManager.instance.SaveCore();
        }
        base.CommitASuicide(type);
    }

    private void LoadAnimal()
    {
        // get animal's name
        int slotId = GetSlotId(PlayerManager.instance.currentMap.animalType);

        // set body
        component.armature.GetSlot("Body").displayIndex = slotId;

        // set head
        component.armature.GetSlot("Head").displayIndex = slotId;
        
        // set tail
        component.armature.GetSlot("Tail").displayIndex = slotId;

        // set back paw
        component.armature.GetSlot("BackPaw").displayIndex = slotId;

        // set back paw 1
        component.armature.GetSlot("BackPaw1").displayIndex = slotId;
        
        // set front paw
        component.armature.GetSlot("FrontPaw").displayIndex = slotId;

        // set front paw 1
        component.armature.GetSlot("FrontPaw1").displayIndex = slotId;
                
        // set front wing
        component.armature.GetSlot("FrontWing").displayIndex = slotId;

        // set back wing
        component.armature.GetSlot("BackWing").displayIndex = slotId;
    }
    // putting on random hats
    private void SetMobRank()
    {
        int display = Random.Range(0, 3);
        component.armature.GetSlot("Hat").displayIndex = display;
        switch (display)
        {
            case 0: mobType = MobType.Common; break;
            case 1: mobType = MobType.Rare; break;
            case 2: mobType = MobType.Royal; break;
        }
    }
    // getting db slot id by AnimalType
    private int GetSlotId(AnimalType type)
    {
        return new Dictionary<AnimalType, int>
        {
            { AnimalType.Pug, 0 },
            { AnimalType.Husky, 1 }
        }[type];
    }
}
