using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingEffect : MonoBehaviour
{
    public GameObject fadingPrefab;

    public float spawTime = .1f;
    public float durationTime = 10f;

    private float duration;
    private float coolDown;

    private bool isActive;

    public void Start()
    {
        ToggleActivation();
    }
    public void Update()
    {
        if (isActive)
        {
            if(coolDown <= 0)
            {
                // spawn fade particle
                GameObject fadingParticle = Instantiate(fadingPrefab, transform.position, Quaternion.identity, GameObject.Find("BackUI").transform);
                fadingParticle.transform.localScale = transform.parent.transform.localScale;

                // set cooldown
                coolDown = spawTime;
            }
            else
                coolDown -= Time.deltaTime;

            if (duration > 0)
                duration -= Time.deltaTime;
        } // if (isActive)
    }

    public void ToggleActivation()
    {
        // activate
        isActive = isActive ? false : true;

        // set duration
        if(isActive)
            duration = durationTime;
    }
}
