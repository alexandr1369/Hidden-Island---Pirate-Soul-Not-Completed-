using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// отображение в мини формате достижений
public class AchievementDisplay : MonoBehaviour
{
    public GameObject achInfoPanel; // панель доп. информации о текущем достижении
    public Achievement achievement; // класс достижения

    public Image template; // задний фон
    public Image artwork; // элемент(иконка)

    private Dictionary<string, Color32> colorDict;
    private Dictionary<string, int> fontDict;

    public Text title; // заглавие

    void Start()
    {
        Init();   
    }
    private void Init()
    {
        colorDict = new Dictionary<string, Color32>()
        {
            { "gradeCommon", new Color32(65, 88, 65, 255) },
            { "gradeRare", new Color32(72, 61, 86, 255) },
            { "gradeLegend", new Color32(93, 53, 92, 255) },
            { "gradeEpic", new Color32(115, 74, 36, 255) },
            { "gradeRelic", new Color32(120, 54, 54, 255) }
        };
        fontDict = new Dictionary<string, int>()
        {
            { "Magnate", 39 },
            { "Hardcore", 38 },
            { "Discovery", 37 },
            { "Richman", 37 }
        };

        title.text = achievement.GetTitle();
        SetTextColorAndFont();

        template.sprite = achievement.template;
        artwork.sprite = achievement.artwork;
    }

    // set and open achievement info panel
    public void OpenAchievementInfoPanel()
    {
        // load acheivement information
        AchievementInfoPanelDisplay.instance.SetInfoPanel(achievement);
    }
    // some 'damn' settings
    private void SetTextColorAndFont()
    {
        // костыль, но только потому что я сразу не подумал о добавлении имени и ранга для достижения
        // и при чем ранг будет enum типа, после чего можно будет генирировать title = name + rank.ToString()
        string rank = achievement.rank.ToString();

        title.color = colorDict["grade" + rank];
        try
        {
            title.fontSize = fontDict[achievement.goal.goalType.ToString()];
        }
        catch
        {
            //print("DO nothing");
        }
    }
}
