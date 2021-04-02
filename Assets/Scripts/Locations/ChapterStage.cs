using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AsyncLoadingButton))]
public class ChapterStage : MonoBehaviour
{
    public StageInfo stageInfo; // stage info of current location

    public List<GameObject> stateHovers; // state hover objects (signal color hover)
        
    public GameObject trailsEffect;  // trails effect (when state is 'Current')

    public void Start()
    {
        // set state color hover
        SetColorHover();

        if(stageInfo.stageState == StageState.Undiscovered)
        {
            GetComponent<AsyncLoadingButton>().isInteractable = false;
            GetComponent<AsyncLoadingButton>().isAnimated = false;
        }

        // check for 'Current' trails effect
        if (stageInfo.stageState == StageState.Current)
            trailsEffect.SetActive(true);
    }
    // color hover info state
    private void SetColorHover()
    {
        // hide all state hovers
        foreach (GameObject stateHover in stateHovers)
        {
            stateHover.SetActive(false);
        }

        // select one state hover and show 
        if(stageInfo.stageIndex != 5)
        {
            // for 1-4 stages
            switch (stageInfo.stageState)
            {
                case StageState.Current: stateHovers.Find(t => t.name == "Current").SetActive(true); break;
                case StageState.Undiscovered: stateHovers.Find(t => t.name == "Undiscovered").SetActive(true); break;
                case StageState.Completed: stateHovers.Find(t => t.name == "Completed").SetActive(true); break;
            }
        }
        else
        {
            // for 5th stage(BOSS)
            stateHovers.Find(t => t.name == "Boss").SetActive(true);
        }
    }
}

