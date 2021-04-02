using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class AchievementInformer : MonoBehaviour
{
    #region Singleton
    public static AchievementInformer instance;
    void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;
        DontDestroyOnLoad(instance);
    }
    #endregion

    public Image bg; // задний фон иконки
    public GameObject[] icons; // вручную настроенные картиночки(иконки)

    public Animator animator; // get component в start не успевает получить, из-за этого лучше так добавить вручную

    public void SetAndShowNewAchievement(Achievement ach)
    {
        // set right icon
        string achName = ach.goal.goalType.ToString();
        SetIconByName(achName);

        // set bg by rank
        // из-за костылей приходится выкручиваться только так(Trim)
        AchievementRank rank = ach.rank;
        Sprite bgSprite = Resources.Load<Sprite>(@"Textures\Achievements\Achievements\" + rank.ToString() + "InformerBg");
        bg.sprite = bgSprite;

        // play animation
        animator.SetTrigger("ShowNewAchievement");
    }
    private void SetIconByName(string name)
    {
        foreach (GameObject icon in icons) {
            icon.SetActive(false);
            if (icon.name == name + "Icon")
                icon.SetActive(true);
        } // foreach
    }
}
