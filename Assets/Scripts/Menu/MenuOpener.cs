using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOpener : MonoBehaviour
{
    public GameObject panel;

    // opening the menu toggle
    public void OpenMenu()
    {
        print("Opening");
        Animator animator = panel.GetComponent<Animator>();
        if (animator != null)
        {
            bool isOpen = animator.GetBool("Open");
            animator.SetBool("Open", !isOpen);
        }
    }
}
