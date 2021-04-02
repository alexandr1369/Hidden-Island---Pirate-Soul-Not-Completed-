using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AchievementGoal
{
    public GoalType goalType; // цель(вид достижения)
    
    public float requiredAmount; // нужное кол-во до выполнения
    public float currentAmount; // текуще кол-во(прогресс)

    public AchievementGoal(GoalType goalType, int requiredAmount)
    {
        this.goalType = goalType;
        this.requiredAmount = requiredAmount;
    }
    // check if the goal is reached
    public bool isReached()
    {
        return (requiredAmount <= currentAmount);
    }
}
// type of the goal
public enum GoalType
{
    Active,
    Boost,
    Death,
    Discovery,
    Hardcore,
    Kill,
    Magnate,
    Richman,
    Score,
    TheLark,
    TheOwl
}
