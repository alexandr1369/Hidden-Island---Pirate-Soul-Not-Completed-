using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// переключение между секциями достижений
public class AchievementSelectionSwapper : MonoBehaviour
{
    #region Singleton
    public static AchievementSelectionSwapper instance;
    void Awake()
    {
        instance = this;
    }
    #endregion

    public int selectedPanel = 0; // текущая активная ячейка(для другого скрипта)

    public GameObject[] selections = new GameObject[3]; // 3 панели с картинками
    public GameObject[] panels = new GameObject[3]; // 3 панели с достижениями
    public Sprite[] normalImages = new Sprite[3]; // картинки не выбранных селекций
    public Sprite[] selectedImages = new Sprite[3]; // картинки выбранных селекций

    private List<float> xStartPanelLocalPos = new List<float>() { 957f, 1f, 160f };

    // скрывает все панели
    // выбирает только текущую
    public void SelectSelection(string selection)
    {
        // back to start position
        //AchievementsMover.instance.SetRectTransformX(startPanelX);

        // hide all panels
        SelectNone();

        // choose and show
        selectedPanel = (int)System.Enum.Parse(typeof(AchievementSelection), selection);
        Image img = selections[selectedPanel].GetComponent<Image>();
        img.sprite = selectedImages[selectedPanel];
        panels[selectedPanel].SetActive(true);

        // demo
        panels[selectedPanel].transform.localPosition = 
            new Vector2(xStartPanelLocalPos[selectedPanel], panels[selectedPanel].transform.localPosition.y);

        // load current wrap to scroller content
        GameObject.Find("Achievements/Main/AchivementsWrapMask").GetComponent<ScrollRect>().content =
            panels[selectedPanel].GetComponent<RectTransform>();
    }
    private void SelectNone()
    {
        for(int i = 0; i < selections.Length; i++)
        {
            Image img = selections[i].GetComponent<Image>();
            img.sprite = normalImages[i];
            panels[i].SetActive(false);
        }
    }
}
// types of selection
public enum AchievementSelection
{
    All,
    Battle,
    Others
}
