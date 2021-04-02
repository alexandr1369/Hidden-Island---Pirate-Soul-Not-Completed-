using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementGradeSwitcher : MonoBehaviour
{
    public GameObject[] grades;

    void Start()
    {
        // спрятать все уровни достижения
        HideAllGrades();
        // показать одно активное сначала
        bool hasFound = false;
        foreach(GameObject grade in grades)
        {
            if (grade.GetComponent<AchievementDisplay>().achievement.isActive) { 
                grade.SetActive(true);
                hasFound = true;
                break;
            }
        }
        // если все уровни достижения полученны -> отобразить 5 грейд
        // TODO: затемнить его или как-то выделить, как полностью законченное достижение
        if (!hasFound)
            grades[grades.Length - 1].SetActive(true);
    }

    private void HideAllGrades()
    {
        foreach(GameObject grade in grades)
            grade.SetActive(false);
    }
}
