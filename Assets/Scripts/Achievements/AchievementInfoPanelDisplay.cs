using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// загрузка доп. информации о достижении в отдельной вкладке
public class AchievementInfoPanelDisplay : MonoBehaviour
{
    #region Singleton
    public static AchievementInfoPanelDisplay instance;
    void Awake()
    {
        instance = this;
    }
    #endregion

    public Image leftPanel; // панель иконки достижения
    public Image rightPanel; // панель описания
    public Image namePanel; // панель названия достижения

    public Text infoText; // описание
    public Text progressText; // прогресс выполнения
    public Text coinAmount; // кол-во монеток(награда)
    public Text soulAmount; // кол-во душ(награда)

    public GameObject[] wraps; // вручную настроенные картиночки(иконка и название) в оболочках

    private Dictionary<AchievementRank, Color32> textColors; // цвет текста описания

    void Start()
    {
        textColors = GetTextColors();
    }
    public void SetInfoPanel(Achievement ach)
    {
        Sprite _leftPanel, _rightPanel, _namePanel;
        string _path = @"Textures\Achievements\", // path of ach resources
            _name = ach.goal.goalType.ToString(); // get name of achievement

        // check for rank validity
        AchievementRank achRank = ach.rank;
        if (achRank == AchievementRank.None) return;

        // get sprites
        _leftPanel = Resources.Load<Sprite>(_path + @"AchievementInfo\LeftPanel" + achRank.ToString());
        _rightPanel = Resources.Load<Sprite>(_path + @"AchievementInfo\RightPanel" + achRank.ToString());
        _namePanel = Resources.Load<Sprite>(_path + @"AchievementInfo\NamePanel" + achRank.ToString());

        // set sprites
        leftPanel.sprite = _leftPanel;
        rightPanel.sprite = _rightPanel;
        namePanel.sprite = _namePanel;
        // из-за дурацкого бага с Preverse Aspect в Image приходится делать через жопу обходным путем
        // (ручная установка)
        ToggleWrapByName(_name);

        // set texts
        infoText.text = ach.description;
        progressText.text = ach.goal.currentAmount + "/" + ach.goal.requiredAmount;
        coinAmount.text = ach.coinReward.ToString();
        soulAmount.text = ach.soulReward.ToString();

        // set text colors
        infoText.color = progressText.color = GetTextColors()[achRank];
    }
    public void ToggleWrapByName(string name)
    {
        foreach (GameObject wrap in wraps) {
            wrap.SetActive(false);
            if (wrap.name == name + "Wrap")
                wrap.SetActive(true);
        }
    }
    // Get text colors of description
    private Dictionary<AchievementRank, Color32> GetTextColors()
    {
        return new Dictionary<AchievementRank, Color32>
        {
            { AchievementRank.Common, new Color32(154, 172, 154, 255) },
            { AchievementRank.Rare, new Color32(160, 151, 170, 255) },
            { AchievementRank.Legend, new Color32(176, 144, 176, 255) },
            { AchievementRank.Epic, new Color32(197, 161, 133, 255) },
            { AchievementRank.Relic, new Color32(198, 145, 145, 255) }
        };
    } 

} // AchievementInfoPanelDisplay