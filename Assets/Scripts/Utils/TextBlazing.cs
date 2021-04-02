using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBlazing : MonoBehaviour
{
    public Material material;

    public void Start()
    {
        Animate();
    }
    // tmp material glow animation with iTween
    private void Animate()
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 0,
            "to", .65f,
            "time", 2,
            "easetype", iTween.EaseType.easeInOutQuad,
            "onupdate", "ChangeMaterialGlow"
        ));

        iTween.ValueTo(gameObject, iTween.Hash(
            "from", .65f,
            "to", 0,
            "time", 2,
            "delay", 2,
            "easetype", iTween.EaseType.easeOutQuad,
            "onupdate", "ChangeMaterialGlow",
            "oncomplete", "Animate"
        ));
    }
    // utils
    private void ChangeMaterialGlow(float value)
    {
        material.SetFloat(ShaderUtilities.ID_GlowPower, value);
    }
}
