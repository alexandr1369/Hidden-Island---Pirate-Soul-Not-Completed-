using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterManager : MonoBehaviour
{
    #region Singleton
    public static CounterManager instance;
    public void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField]
    private GameObject panel; // counter panel

    [SerializeField]
    private Animator animator; // animator

    public void Count()
    {
        // toggle panel and animate counting
        bool _state = panel.activeSelf;
        if (!_state)
        {
            panel.SetActive(!_state);
            animator.SetBool("Counting", true);
        }
    }
    public void StopCounting()
    {
        if(panel.activeSelf)
            panel.SetActive(false);
    }
}