using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// само достижение
[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievement")]
public class Achievement : ScriptableObject
{
    public bool isActive; // is completed(false - yes)

    public AchievementRank rank; // ранг достижения(I-V)
    public string description; // описание 
    public int coinReward; // награда в монетках
    public int soulReward; // награда в душах

    public AchievementGoal goal; // цель

    public Sprite template; // картинка BG
    public Sprite artwork; // картинка Icon

    // complete the achievement
    public void Complete()
    {
        // сохранения прогресса излишне, т.к. прогресс будет храниться в уже созданных файлах ачивки
        isActive = false;
        // remove counter from list
        switch (goal.goalType)
        {
            case GoalType.Active: AchievementManager.instance.activeAchievements -= DoGoal; break;
            case GoalType.Boost: AchievementManager.instance.boostAchievements -= DoGoal; break;
            case GoalType.Death: AchievementManager.instance.deathAchievements -= DoGoal; break;
            case GoalType.Discovery: AchievementManager.instance.discoveryAchievements -= DoGoal; break;
            case GoalType.Hardcore: AchievementManager.instance.hardcoreAchievements -= DoGoal; break;
            case GoalType.Kill: AchievementManager.instance.killAchievements -= DoGoal; break;
            case GoalType.Magnate: AchievementManager.instance.magnateAchievements -= DoGoal; break;
            case GoalType.Richman: AchievementManager.instance.richmanAchievements -= DoGoal; break;
            case GoalType.Score: AchievementManager.instance.scoreAchievements -= DoGoal; break;
            case GoalType.TheLark: AchievementManager.instance.morningAchievements -= DoGoal; break;
            case GoalType.TheOwl: AchievementManager.instance.nightAchievements -= DoGoal; break;
            default: break;
        }
        // give the reward
        GiveReward();
        // notify in the corner about achixevement
        AchievementInformer.instance.SetAndShowNewAchievement(this);
        AchievementManager.instance.SendInfo($"{goal.goalType.ToString()} {GetRankToRomanNum()} compled!");
    }
    // get ach title to string
    public string GetTitle()
    {
        return $"{goal.goalType.ToString()} {GetRankToRomanNum()}";
    }
    // do achievement
    public void DoGoal(GoalType type)
    {
        // инкремент текущего кол-ва единиц выполненного достижения
        if (goal.goalType == GoalType.Hardcore) { 
            // HardcoreMode only has Time Achievement(stay aliving for N secs)
            // That's why it'll be increasing by deltatime
            goal.currentAmount -= Time.deltaTime;
        }
        else { 
            // The rest will be increasing by 1
            goal.currentAmount++;
        }
        // test write info
        AchievementManager.instance.SendInfo(goal.goalType.ToString() + ": " + goal.currentAmount + "/" + goal.requiredAmount);
        // проверка на завершение достижения
        if (goal.isReached()) Complete();
    }
    // выдача награды
    private void GiveReward()
    {
        if (coinReward > 0)
            PlayerManager.instance.AddCoins(coinReward);
        if (soulReward > 0)
            PlayerManager.instance.AddSouls(soulReward);
    }
    private string GetRankToRomanNum()
    {
        string romanNum;
        switch (rank)
        {
            case AchievementRank.Common: romanNum = "I"; break;
            case AchievementRank.Rare: romanNum = "II"; break;
            case AchievementRank.Legend: romanNum = "III"; break;
            case AchievementRank.Epic: romanNum = "IV"; break;
            case AchievementRank.Relic: romanNum = "V";  break;
            default: romanNum = string.Empty; break;
        }

        return romanNum;
    }
}
public enum AchievementRank
{
    Common,
    Rare,
    Legend,
    Epic,
    Relic,
    None
}